namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal
{
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
}