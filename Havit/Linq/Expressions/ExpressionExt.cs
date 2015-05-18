using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Linq.Expressions
{
	/// <summary>
	/// Pomocné metody pro práci s Expression.
	/// </summary>
	public static class ExpressionExt
	{
		#region ReplaceParameter
		/// <summary>
		/// Vymění v expression jeden parameter druhým. Slouží například k náhradě na mujObjekt => mujObjekt.Id na item => item.Id.
		/// </summary>
		/// <param name="expression">Expression, ve kterém je prováděna náhrada parametru.</param>
		/// <param name="sourceParameter">Parametr, který má být nalezen a nahrazen.</param>
		/// <param name="targetParameter">Parametr, kterým bude sourceParametr nahrazen.</param>
		public static Expression ReplaceParameter(Expression expression, ParameterExpression sourceParameter, ParameterExpression targetParameter)
		{
			return new ReplaceParameterVisitor(sourceParameter, targetParameter).Visit(expression);
		}
		#endregion

		#region RemoveConvert
		/// <summary>
		/// Odstraní z expression Convert, pokud je přítomen. Nahrazuje jej jen na nejvyšší úrovni.
		/// Slouží k náhradě item => (object)item.Id na item => item.Id.
		/// </summary>
		/// <param name="expression">Expression, ve které se Convert vyhledává.</param>
		public static Expression RemoveConvert(this Expression expression)
		{
			while ((expression.NodeType == ExpressionType.Convert) || (expression.NodeType == ExpressionType.ConvertChecked))
			{
				expression = ((UnaryExpression)expression).Operand;
			}
			return expression;
		}
		#endregion
	}
}
