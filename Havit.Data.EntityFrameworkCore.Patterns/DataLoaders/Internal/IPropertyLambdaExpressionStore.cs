namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal
{
	/// <summary>
	/// Úložiště PropertyLambdaExpression.
	/// </summary>
	public interface IPropertyLambdaExpressionStore
	{
		/// <summary>
		/// Vrací PropertyLambdaExpression, pokud v úložiští existuje.
		/// </summary>
		bool TryGet<TEntity, TProperty>(string propertyName, out PropertyLambdaExpression<TEntity, TProperty> result);

		/// <summary>
		/// Uloží PropertyLambdaExpression do úložiště.
		/// </summary>
		void Store<TEntity, TProperty>(string propertyName, PropertyLambdaExpression<TEntity, TProperty> propertyLambdaExpression);
	}
}