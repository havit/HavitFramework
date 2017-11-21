namespace Havit.Data.Entity.Patterns.DataLoaders.Internal
{
	/// <summary>
	/// Konstruuje PropertyLambdaExpression.
	/// </summary>
	internal interface IPropertyLambdaExpressionBuilder
	{
		/// <summary>
		/// Vrací PropertyLambdaExpression pro získání vlastnosti propertyName dané TEntity.
		/// </summary>
		PropertyLambdaExpression<TEntity, TProperty> Build<TEntity, TProperty>(string propertyName);
	}
}