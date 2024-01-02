using System;
using System.Linq;
using System.Linq.Expressions;
using Havit.Diagnostics.Contracts;

namespace Havit.Linq.Expressions;

/// <summary>
/// Helper methods for working with Expression.
/// </summary>
public static class ExpressionExt
{
	/// <summary>
	/// Replaces one parameter with another in the expression. Used, for example, to replace mujObjekt => mujObjekt.Id with item => item.Id.
	/// </summary>
	/// <param name="expression">The expression in which the parameter replacement is performed.</param>
	/// <param name="sourceParameter">The parameter to be found and replaced.</param>
	/// <param name="targetParameter">The parameter with which the source parameter will be replaced.</param>
	public static Expression ReplaceParameter(Expression expression, ParameterExpression sourceParameter, ParameterExpression targetParameter)
	{
		return new ReplaceParameterVisitor(sourceParameter, targetParameter).Visit(expression);
	}

	/// <summary>
	/// Removes the Convert from the expression, if present. Replaces it only at the highest level.
	/// Used to replace item => (object)item.Id with item => item.Id.
	/// </summary>
	/// <param name="expression">The expression in which the Convert is searched.</param>
	public static Expression RemoveConvert(this Expression expression)
	{
		while ((expression.NodeType == ExpressionType.Convert) || (expression.NodeType == ExpressionType.ConvertChecked))
		{
			expression = ((UnaryExpression)expression).Operand;
		}
		return expression;
	}

	/// <summary>
	/// If we have a lambda expression (B b) => b.C, but we want to apply the expression tree to another object from which we only get B (for example, (A a) => a.B), it modifies the expression to this form.
	/// That is, for inputs
	/// expression: (B b) => b.C % 2 == 0
	/// substitution (A a) => a.B
	/// replaces the parameter 'b' in the expression with 'a.B'.
	/// returns (A a) => a.B.C % 2 == 0
	/// </summary>
	/// <typeparam name="TSource">The input parameter of the lambda expression in which the transformation takes place.</typeparam>
	/// <typeparam name="TTarget">The input parameter of the lambda expression after the transformation. </typeparam>
	/// <typeparam name="TResult">The type returned by the input (and output) lambda expression.</typeparam>
	/// <param name="expression">The expression in which the substitution takes place.</param>
	/// <param name="substitution">The expression used as a substitution in the expression.</param>
	public static Expression<Func<TTarget, TResult>> SubstituteParameter<TSource, TTarget, TResult>(Expression<Func<TSource, TResult>> expression, Expression<Func<TTarget, TSource>> substitution)
	{
		return (Expression<Func<TTarget, TResult>>)new SubstitutionVisitor<TSource, TTarget, TResult>(expression, substitution).Visit(expression);
	}

	/// <summary>
	/// Returns an expression corresponding to the condition AND between individual expressions.
	/// </summary>
	/// <param name="expressions">Conditions to be combined with the AND operator.</param>
	/// <returns>
	/// If expressions is null or contains only null values, returns null.
	/// Otherwise, combines the expressions and returns them joined by the AND condition.
	/// For example, for input: item => item.A, item => item.B, item => item.C returns item => item.A && item.B && item.C
	/// </returns>
	public static Expression<Func<T, bool>> AndAlso<T>(params Expression<Func<T, bool>>[] expressions)
	{
		if (expressions == null)
		{
			return null;
		}

		var notNullExpressions = expressions.Where(item => item != null).ToList();
		if (notNullExpressions.Count == 0)
		{
			return null;
		}

		Expression result = notNullExpressions[0].Body;
		ParameterExpression resultParameter = notNullExpressions[0].Parameters[0];

		for (int i = 1; i < notNullExpressions.Count; i++)
		{
			result = Expression.AndAlso(result, Havit.Linq.Expressions.ExpressionExt.ReplaceParameter(notNullExpressions[i].Body, notNullExpressions[i].Parameters[0], resultParameter));
		}

		return (Expression<Func<T, bool>>)Expression.Lambda(result, resultParameter);
	}

	/// <summary>
	/// Returns the name of the member (property) to which the expression accesses. For example, for "person => person.Age" returns "Age".
	/// Richer expressions than this trivial one are not supported.
	/// </summary>
	public static string GetMemberAccessMemberName(Expression expression)
	{
		Contract.Requires(expression is LambdaExpression);

		Expression expressionBody = ((LambdaExpression)expression).Body.RemoveConvert();

		if (expressionBody is MemberExpression)
		{
			MemberExpression memberExpression = (MemberExpression)expressionBody;
			if (memberExpression.Expression is System.Linq.Expressions.ParameterExpression)
			{
				return memberExpression.Member.Name;
			}
		}

		throw new InvalidOperationException($"Expression '{expression.ToString()}' is not supported.");
	}
}
