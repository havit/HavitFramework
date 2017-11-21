namespace Havit.Data.Entity.Patterns.DataLoaders.Internal
{
	internal interface IPropertyLambdaExpressionBuilder
	{
		PropertyLambdaExpression<TEntity, TProperty> Build<TEntity, TProperty>(string propertyName);
	}
}