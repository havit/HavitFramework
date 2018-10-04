using System.Linq.Expressions;

namespace Havit.Linq.Expressions
{
	internal class ReplaceParameterVisitor : ExpressionVisitor
	{
		private readonly ParameterExpression fromParameter;
		private readonly ParameterExpression toParameter;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public ReplaceParameterVisitor(ParameterExpression fromParameter, ParameterExpression toParameter)
		{
			this.fromParameter = fromParameter;
			this.toParameter = toParameter;
		}

		/// <summary>
		/// Nahradí parametr.
		/// </summary>
		protected override Expression VisitParameter(ParameterExpression node)
		{
			return (node == fromParameter) ? toParameter : base.VisitParameter(node);
		}
	}
}