using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Linq.Expressions;

internal class SubstitutionVisitor<TSource, TTarget, TResult> : ExpressionVisitor
{
	private readonly Expression<Func<TSource, TResult>> expression;
	private readonly Expression<Func<TTarget, TSource>> substitution;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public SubstitutionVisitor(Expression<Func<TSource, TResult>> expression, Expression<Func<TTarget, TSource>> substitution)
	{
		this.expression = expression;
		this.substitution = substitution;
	}

	protected override Expression VisitLambda<T>(Expression<T> node)
	{
		// potřebujeme zamezit tomu, aby se navštívili node.Parameters!
		return Expression.Lambda(Visit(node.Body), substitution.Parameters);
	}

	/// <summary>
	/// Nahradí parametr.
	/// </summary>
	protected override Expression VisitParameter(ParameterExpression node)
	{
		return (node == expression.Parameters[0])
			? substitution.Body
			: base.VisitParameter(node);
	}
}
