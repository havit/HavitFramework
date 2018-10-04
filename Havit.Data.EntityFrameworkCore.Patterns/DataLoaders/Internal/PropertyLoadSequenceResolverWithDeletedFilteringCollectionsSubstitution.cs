using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal
{
	/// <summary>
	/// Poskytuje seznam vlastností k načtení.
	/// Pokud má být načtena vlastnost X, která je kolekcí, a zároveň existuje kolekce XWithDeleted, pak X substituuje touto XWithDeleted.
	/// </summary>
	public class PropertyLoadSequenceResolverWithDeletedFilteringCollectionsSubstitution : PropertyLoadSequenceResolver
	{
		/// <summary>
		/// Vrací z expression tree seznam vlastností, které mají být DataLoaderem načteny.
		/// </summary>
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