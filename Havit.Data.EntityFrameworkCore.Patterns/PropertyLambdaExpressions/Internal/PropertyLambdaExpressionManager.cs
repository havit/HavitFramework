namespace Havit.Data.EntityFrameworkCore.Patterns.PropertyLambdaExpressions.Internal
{
	/// <summary>
	/// Poskytuje PropertyLambdaExpression.
	/// </summary>
	public class PropertyLambdaExpressionManager : IPropertyLambdaExpressionManager
	{
		private readonly IPropertyLambdaExpressionStore _propertyLambdaExpressionStore;
		private readonly IPropertyLambdaExpressionBuilder _propertyLambdaExpressionBuilder;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public PropertyLambdaExpressionManager(IPropertyLambdaExpressionStore propertyLambdaExpressionStore, IPropertyLambdaExpressionBuilder propertyLambdaExpressionBuilder)
		{
			this._propertyLambdaExpressionStore = propertyLambdaExpressionStore;
			this._propertyLambdaExpressionBuilder = propertyLambdaExpressionBuilder;
		}

		/// <summary>
		/// Vrací PropertyLambdaExpression pro získání vlastnosti propertyName dané TEntity.
		/// </summary>
		public PropertyLambdaExpression<TEntity, TProperty> GetPropertyLambdaExpression<TEntity, TProperty>(string propertyName)
		{
			if (_propertyLambdaExpressionStore.TryGet<TEntity, TProperty>(propertyName, out var result))
			{
				return result;
			}

			result = _propertyLambdaExpressionBuilder.Build<TEntity, TProperty>(propertyName);
			_propertyLambdaExpressionStore.Store<TEntity, TProperty>(propertyName, result);

			return result;
		}
	}
}
