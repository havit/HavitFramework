using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Entity.Patterns.DataLoaders.Internal
{
	/// <summary>
	/// Konstruuje PropertyLambdaExpression.
	/// </summary>
	public class PropertyLambdaExpressionBuilder : IPropertyLambdaExpressionBuilder
	{
		/// <summary>
		/// Vrací PropertyLambdaExpression pro získání vlastnosti propertyName dané TEntity.
		/// </summary>
		public PropertyLambdaExpression<TEntity, TProperty> Build<TEntity, TProperty>(string propertyName)
		{
			ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "item");
			Expression<Func<TEntity, TProperty>> lambdaExpression = Expression.Lambda<Func<TEntity, TProperty>>(Expression.Property(parameter, propertyName), parameter);
			Func<TEntity, TProperty> lambdaCompiled = lambdaExpression.Compile();

			return new PropertyLambdaExpression<TEntity, TProperty>(lambdaExpression, lambdaCompiled);
		}
	}
}
