namespace Havit.Data.EntityFrameworkCore.Patterns.PropertyLambdaExpressions.Internal;

/// <summary>
/// Konstruuje PropertyLambdaExpression.
/// </summary>
public interface IPropertyLambdaExpressionBuilder
{
	/// <summary>
	/// Vrací PropertyLambdaExpression pro získání vlastnosti propertyName dané TEntity.
	/// </summary>
	PropertyLambdaExpression<TEntity, TProperty> Build<TEntity, TProperty>(string propertyName);
}