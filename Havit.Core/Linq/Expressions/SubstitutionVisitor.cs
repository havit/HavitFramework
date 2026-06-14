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
		// Only the top-level lambda gets its parameters replaced (we need to prevent node.Parameters from being visited).
		// Nested lambdas (e.g. inside Any(...)) must keep their own parameters, only their bodies are visited.
		if (node == (Expression)expression)
		{
			return Expression.Lambda(Visit(node.Body), substitution.Parameters);
		}
		return base.VisitLambda(node);
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
