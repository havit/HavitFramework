using System;
using System.Linq.Expressions;

namespace Havit.Data.Entity.Patterns.DataLoaders.Internal
{
	public class PropertyLoadSequenceResolver : IPropertyLoadSequenceResolver
	{
		public virtual PropertyToLoad[] GetPropertiesToLoad<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertyPath)
			where TEntity : class
		{
			PropertiesSequenceExpressionVisitor visitor = new PropertiesSequenceExpressionVisitor();
			return visitor.GetPropertiesToLoad(propertyPath);
		}
	}
}