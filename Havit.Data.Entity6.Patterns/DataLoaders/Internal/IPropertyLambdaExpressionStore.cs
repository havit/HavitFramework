namespace Havit.Data.Entity.Patterns.DataLoaders.Internal
{
	internal interface IPropertyLambdaExpressionStore
	{
		bool TryGet<TEntity, TProperty>(string propertyName, out PropertyLambdaExpression<TEntity, TProperty> result);
		void Store<TEntity, TProperty>(string propertyName, PropertyLambdaExpression<TEntity, TProperty> propertyLambdaExpression);
	}
}