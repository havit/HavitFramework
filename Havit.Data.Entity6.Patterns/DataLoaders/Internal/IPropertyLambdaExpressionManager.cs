namespace Havit.Data.Entity.Patterns.DataLoaders.Internal
{
	/// <summary>
	/// Poskytuje PropertyLambdaExpression.
	/// </summary>
	public interface IPropertyLambdaExpressionManager
	{
		/// <summary>
		/// Vrací PropertyLambdaExpression pro získání vlastnosti propertyName dané TEntity.
		/// </summary>
		PropertyLambdaExpression<TEntity, TProperty> GetPropertyLambdaExpression<TEntity, TProperty>(string propertyName);
	}
}