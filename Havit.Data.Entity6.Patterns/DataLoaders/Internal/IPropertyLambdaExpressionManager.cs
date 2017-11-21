namespace Havit.Data.Entity.Patterns.DataLoaders.Internal
{
	public interface IPropertyLambdaExpressionManager
	{
		PropertyLambdaExpression<TEntity, TProperty> GetPropertyLambdaExpression<TEntity, TProperty>(string propertyName);
	}
}