using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Havit.Diagnostics.Contracts;

namespace Havit.Business.TestExtensions
{
	/// <summary>
	/// Extension metody k BusinessObjectBase pro podporu psaní (unit) testů.
	/// </summary>
	public static class BusinessObjectBaseExtensions
	{
		/// <summary>
		/// Přepne objekt do disconnected stavu, pokud ještě není disconnected.
		/// </summary>
		public static void SetDisconnected(this BusinessObjectBase businessObject)
		{
			Contract.Requires<ArgumentNullException>(businessObject != null, nameof(businessObject));

			businessObject.SetDisconnected();
		}

		/// <summary>
		/// Nastaví hodnotu vlastnosti. Určeno zejména pro readonly objekty, které nemají veřejné settery vlastností.
		/// </summary>
		public static void SetProperty<TBusinessObject, TValue>(this TBusinessObject businessObject, Expression<Func<TBusinessObject, TValue>> propertyPath, TValue value)
			where TBusinessObject : BusinessObjectBase
		{
			Contract.Requires<InvalidOperationException>(businessObject.IsLoaded, "SetProperty nelze volat na ghost objektu. Objekt musí být nejprve přepnut do stavu Disconnected.");

			// vytvoříme přiřazení "propertyPath = value"
			// nejprve musíme TypedParameterExpression propertyPath vyměnit za businessObject
			Expression propertyToAssignExpression = new ReplaceParameterVisitor(propertyPath.Parameters[0], Expression.Constant(businessObject)).Visit(propertyPath.Body);
			Expression assignExpression = Expression.Assign(
				propertyToAssignExpression, // kam přiřazujeme
				Expression.Constant(value, typeof(TValue))); // co přiřazujeme

			var lambda = Expression.Lambda(assignExpression, null); // vyrobíme lambdu bez parametrů (jde kompilovat)
			var compiledLambda = lambda.Compile(); // zkompilujeme
			compiledLambda.DynamicInvoke(); // a vykonáme
		}

		/// <summary>
		/// Provádí náhradu parametru v Expression.
		/// </summary>
		internal class ReplaceParameterVisitor : ExpressionVisitor
		{
			private readonly ParameterExpression fromParameterExpression;
			private readonly Expression toExpression;

			/// <summary>
			/// Konstruktor.
			/// </summary>
			public ReplaceParameterVisitor(ParameterExpression fromParameterExpression, Expression toExpression)
			{
				this.fromParameterExpression = fromParameterExpression;
				this.toExpression = toExpression;
			}

			/// <summary>
			/// Nahradí parametr.
			/// </summary>
			protected override Expression VisitParameter(ParameterExpression node)
			{
				return (node == fromParameterExpression) ? toExpression : base.VisitParameter(node);
			}
		}
	}
}
