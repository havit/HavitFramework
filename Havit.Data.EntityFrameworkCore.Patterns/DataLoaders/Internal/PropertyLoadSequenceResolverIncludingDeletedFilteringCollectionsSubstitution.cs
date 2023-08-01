using System.Linq.Expressions;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;

/// <summary>
/// Poskytuje seznam vlastností k načtení.
/// Pokud má být načtena vlastnost X, která je kolekcí, a zároveň existuje kolekce XIncludingDeleted, pak X substituuje touto XIncludingDeleted.
/// </summary>
public class PropertyLoadSequenceResolverIncludingDeletedFilteringCollectionsSubstitution : PropertyLoadSequenceResolver
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
			// a existuje vlastnost s pojmenováním "IncludingDeleted" na konci
			// která obsahuje prvky stejného typu
			// pak provedeme substituci
			if (propertyToLoad.IsCollection)
			{
				string propertyNameIncludingDeleted = propertyToLoad.PropertyName + "IncludingDeleted";
				PropertyInfo propertyIncludingDeleted = propertyToLoad.SourceType.GetProperty(propertyNameIncludingDeleted);
				if (propertyIncludingDeleted != null)
				{
					Type enumerableType = typeof(IEnumerable<>).MakeGenericType(propertyToLoad.CollectionItemType);
					if (enumerableType.IsAssignableFrom(propertyIncludingDeleted.PropertyType))
					{
						propertyToLoad.PropertyName = propertyNameIncludingDeleted;
						propertyToLoad.TargetType = propertyIncludingDeleted.PropertyType;
					}
				}
			}
		}
		return propertiesToLoad;
	}
}