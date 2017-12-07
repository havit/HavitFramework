using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Havit.Data.Entity.Patterns.DataLoaders.Internal
{
	public class PropertyLoadSequenceResolverWithDeletedFilteringCollectionsSubstitution : PropertyLoadSequenceResolver
	{
		public override PropertyToLoad[] GetPropertiesToLoad<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertyPath)
		{
			PropertyToLoad[] propertiesToLoad = base.GetPropertiesToLoad(propertyPath);
			foreach (PropertyToLoad propertyToLoad in propertiesToLoad)
			{
				// pokud jde o kolekci
				// a existuje vlastnost s pojmenováním "WithDeleted" na konci
				// která obsahuje prvky stejného typu
				// pak provedeme substituci
				if (propertyToLoad.IsCollection)
				{
					string propertyNameWithDeleted = propertyToLoad.PropertyName + "WithDeleted";
					PropertyInfo propertyWithDeleted = propertyToLoad.SourceType.GetProperty(propertyNameWithDeleted);					
					if (propertyWithDeleted != null)
					{
						 Type enumerableType = typeof(IEnumerable<>).MakeGenericType(propertyToLoad.CollectionItemType);						
						if (enumerableType.IsAssignableFrom(propertyWithDeleted.PropertyType))
						{
							propertyToLoad.PropertyName = propertyNameWithDeleted;
							propertyToLoad.TargetType = propertyWithDeleted.PropertyType;
						}
					}
				}
			}
			return propertiesToLoad;
		}
	}
}