using System;
using System.Linq;
using System.Linq.Expressions;
using Havit.Diagnostics.Contracts;

namespace Havit.Linq.Expressions;

/// <summary>
/// Pomocné metody pro práci s Expression.
/// </summary>
public static class ExpressionExt
{
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

	/// <summary>
	/// Pokud máme lambda výraz (B b) => b.C, ale chceme expression tree aplikovat nad jiný objekt, ze kterého B teprve získáme (např. (A a) => a.B, pak pak upraví výraz do této podoby.
	/// Tj. pro vstupy
	/// expression: (B b) => b.C % 2 == 0
	/// substitution (A a) => a.B
	/// provede náhradu parametru &quot;b&quot; v expression za &quot;a.B&quot;.
	/// vrací (A a) => a.B.C % 2 == 0
	/// </summary>
	/// <typeparam name="TSource">Vstupní parametr lambda výrazu, ve kterém dojde k transformaci.</typeparam>
	/// <typeparam name="TTarget">Vstupní parametr lambda výrazu po transformaci. </typeparam>
	/// <typeparam name="TResult">Typ, která vrací vstupní (a výstupní) lambda výraz.</typeparam>
	/// <param name="expression">Výraz, ve kterém dojde k substituci.</param>
	/// <param name="substitution">Výraz, který je použit jako substituce v expression.</param>
	public static Expression<Func<TTarget, TResult>> SubstituteParameter<TSource, TTarget, TResult>(Expression<Func<TSource, TResult>> expression, Expression<Func<TTarget, TSource>> substitution)
	{
		return (Expression<Func<TTarget, TResult>>)new SubstitutionVisitor<TSource, TTarget, TResult>(expression, substitution).Visit(expression);
	}

	/// <summary>
	/// Vrátí výraz odpovídající podmínce AND mezi jednotlivými expressions.
	/// </summary>
	/// <param name="expressions">Podmínky ke spojení operátorem AND.</param>
	/// <returns>
	/// Pokud je expressions null nebo obsahuje jen null hodnoty, vrací null.
	/// Jinak zkombinuje výrazy a vrátí je spojené podmínkou AND.
	/// Např. pro vstup: item => item.A, item => item.B, item => item.C vrátí item => item.A &amp;&amp; item.B &amp;&amp; item.C
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
	/// Vrací název membera (property), ke které je v expression přistupováno. Tj. pro "person => person.Age" vrací "Age".
	/// Nejsou podporovány bohatší expression, než takto triviální.
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
