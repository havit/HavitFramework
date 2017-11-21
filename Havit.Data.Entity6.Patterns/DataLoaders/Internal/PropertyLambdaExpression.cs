using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Entity.Patterns.DataLoaders.Internal
{
	public class PropertyLambdaExpression<TEntity, TProperty>
	{
		public Expression<Func<TEntity, TProperty>> LambdaExpression { get; }
		public Func<TEntity, TProperty> LambdaCompiled { get; }

		public PropertyLambdaExpression(Expression<Func<TEntity, TProperty>> lambdaExpression, Func<TEntity, TProperty> lambdaCompiled)
		{
			this.LambdaExpression = lambdaExpression;
			this.LambdaCompiled = lambdaCompiled;
		}
	}
}
