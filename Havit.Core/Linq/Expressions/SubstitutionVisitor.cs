using System.Linq.Expressions;

namespace Havit.Linq.Expressions;

internal class SubstitutionVisitor<TSource, TTarget, TResult> : ExpressionVisitor
{
	private readonly Expression<Func<TSource, TResult>> expression;
	private readonly Expression<Func<TTarget, TSource>> substitution;

	/// <summary>
	/// Constructor.
	/// </summary>
	public SubstitutionVisitor(Expression<Func<TSource, TResult>> expression, Expression<Func<TTarget, TSource>> substitution)
	{
		this.expression = expression;
		this.substitution = substitution;
	}

	protected override Expression VisitLambda<T>(Expression<T> node)
	{
		// We need to prevent node.Parameters from being visited!
		return Expression.Lambda(Visit(node.Body), substitution.Parameters);
	}

	/// <summary>
	/// Replaces the parameter.
	/// </summary>
	protected override Expression VisitParameter(ParameterExpression node)
	{
		return (node == expression.Parameters[0])
			? substitution.Body
			: base.VisitParameter(node);
	}
}
