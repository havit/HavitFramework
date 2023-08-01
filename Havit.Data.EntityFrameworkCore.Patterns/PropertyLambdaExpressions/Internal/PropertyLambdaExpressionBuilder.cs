using System.Linq.Expressions;

namespace Havit.Data.EntityFrameworkCore.Patterns.PropertyLambdaExpressions.Internal;

/// <summary>
/// Konstruuje PropertyLambdaExpression.
/// </summary>
public class PropertyLambdaExpressionBuilder : IPropertyLambdaExpressionBuilder
{
	/// <summary>
	/// Vrací PropertyLambdaExpression pro získání vlastnosti propertyName dané TEntity.
	/// </summary>
	public PropertyLambdaExpression<TEntity, TProperty> Build<TEntity, TProperty>(string propertyName)
	{
		ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "item");
		Expression<Func<TEntity, TProperty>> lambdaExpression = Expression.Lambda<Func<TEntity, TProperty>>(Expression.Property(parameter, propertyName), parameter);
		Func<TEntity, TProperty> lambdaCompiled = lambdaExpression.Compile();

		return new PropertyLambdaExpression<TEntity, TProperty>(lambdaExpression, lambdaCompiled);
	}
}
