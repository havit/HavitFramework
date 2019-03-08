namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal
{
	// TODO JK: Přesun do jiného namespace? Už nejen dataloader, ale taky caching.

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