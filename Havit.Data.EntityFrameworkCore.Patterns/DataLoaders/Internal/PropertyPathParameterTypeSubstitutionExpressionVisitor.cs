using System.Linq.Expressions;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;

/// <summary>
/// Přepisuje property path expression tak, že nahradí typ parametru lambdy jiným (konkrétním) typem.
/// Používá se pro podporu načítání vlastností entit, které jsou typovány interface.
/// Přístupy na členy původního parametru jsou remapovány na stejnojmenné public vlastnosti nového typu (předpokládá se implicitní implementace).
/// </summary>
internal class PropertyPathParameterTypeSubstitutionExpressionVisitor : ExpressionVisitor
{
	private ParameterExpression _originalParameter;
	private ParameterExpression _substitutedParameter;

	/// <summary>
	/// Vrátí lambda výraz odpovídající <paramref name="propertyPath" />, jehož parametr je typu <paramref name="parameterType" />.
	/// Návratový typ lambdy zůstává zachován.
	/// </summary>
	/// <exception cref="InvalidOperationException">Pokud některá z vlastností není na typu <paramref name="parameterType"/> dostupná jako stejnojmenná public vlastnost (např. explicitní implementace interface).</exception>
	public LambdaExpression SubstituteParameterType(LambdaExpression propertyPath, Type parameterType)
	{
		ArgumentNullException.ThrowIfNull(propertyPath);
		ArgumentNullException.ThrowIfNull(parameterType);

		_originalParameter = propertyPath.Parameters.Single();
		_substitutedParameter = Expression.Parameter(parameterType, _originalParameter.Name);

		Expression body = Visit(propertyPath.Body);

		return Expression.Lambda(typeof(Func<,>).MakeGenericType(parameterType, propertyPath.ReturnType), body, _substitutedParameter);
	}

	/// <inheritdoc />
	protected override Expression VisitParameter(ParameterExpression node)
	{
		return (node == _originalParameter)
			? _substitutedParameter
			: node;
	}

	/// <inheritdoc />
	protected override Expression VisitMember(MemberExpression node)
	{
		Expression expression = Visit(node.Expression);

		if (expression == node.Expression)
		{
			return node;
		}

		if (expression.Type == node.Expression.Type)
		{
			return node.Update(expression);
		}

		// Typ výrazu, na kterém k přístupu ke členu dochází, se změnil (typicky nahrazení parametru).
		// Musíme remapovat PropertyInfo na nový typ, jinak by dále zpracovávaný expression tree obsahoval DeclaringType původního (nemapovaného) typu.
		PropertyInfo substitutedProperty = expression.Type.GetProperty(node.Member.Name, BindingFlags.Public | BindingFlags.Instance);
		if (substitutedProperty == null)
		{
			throw new InvalidOperationException($"DataLoader cannot load property {node.Member.Name} declared on type {node.Member.DeclaringType.FullName} while there is no public property of the same name on type {expression.Type.FullName}. The property is required to be implemented implicitly (as a public property with the same name).");
		}

		return Expression.Property(expression, substitutedProperty);
	}
}
