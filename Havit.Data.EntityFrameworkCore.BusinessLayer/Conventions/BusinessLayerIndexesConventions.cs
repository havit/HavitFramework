using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes.Metadata;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata;
using Havit.Data.EntityFrameworkCore.Conventions;
using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Conventions
{
	/// <summary>
	/// Konvencia pre vytvorenie indexov, ktoré definuje BusinessLayerGenerator. Premenuje taktiež všetky existujúce indexy, aby mali prefix "FKX_".
	/// 
	/// Konvencie:
	/// <list>
	/// <item>každý stĺpec s FK v tabuľke (+ Deleted stĺpec ak existuje)</item>
	/// <item>na kolekciách, resp. tabuľke s FK definuje index pre stĺpec určený Sorting extended property (Sorting ext. prop. je v entite s kolekciou)</item>
	/// <item>v lokalizačných tabuľkách (XyLocalization) unikátny index pre ParentId a LanguageId FK</item>
	/// <item>v tabuľke Langauge na UICulture property (ak existuje)</item>
	/// </list>
	/// 
	/// <remarks>EF Core inteligentne zahadzuje redundantné indexy, takže zostanú len tie, ktoré majú význam.</remarks>
	/// </summary>
	public class BusinessLayerIndexesConventions : IModelConvention
    {
	    /// <inheritdoc />
	    public void Apply(ModelBuilder modelBuilder)
        {
            foreach (IMutableEntityType entityType in modelBuilder.Model.GetApplicationEntityTypes())
            {
				RenameForeignKeyIndexes(entityType.GetForeignKeys());

	            if (ShouldGenerateIndexes(entityType.ClrType))
	            {
		            AddTableIndexes(entityType);
	            }
            }
        }

	    private static void AddTableIndexes(IMutableEntityType entityType)
        {
            IMutableProperty deletedProperty = entityType.GetDeletedProperty();

            foreach (IMutableProperty property in entityType.GetNotIgnoredProperties()
                .Where(p => ShouldGenerateIndexes(p.PropertyInfo)))
            {
                if (property.IsPrimaryKey() || !property.IsForeignKey())
                {
                    continue;
                }

                IMutableIndex index;
                if (deletedProperty != null)
                {
                    index = entityType.GetOrAddIndex(new[] { property, deletedProperty });
                }
                else
                {
                    index = entityType.GetOrAddIndex(property);
                }

                ReplaceIndexPrefix(index);
            }

	        CreateCollectionOrderIndex(entityType);

			if (entityType.IsLocalizationEntity())
			{
				IMutableEntityType parentEntity = entityType.GetLocalizationParentEntityType();
				IMutableProperty parentLocalizationProperty = entityType.GetForeignKeys().FirstOrDefault(fk => fk.PrincipalEntityType == parentEntity)?.Properties?[0];

                IMutableProperty languageProperty = entityType.GetLanguageProperty();

				IMutableIndex index = entityType.GetOrAddIndex(new[] { parentLocalizationProperty, languageProperty });
				index.IsUnique = true;
				ReplaceIndexPrefix(index);
            }

            if (entityType.IsLanguageEntity())
            {
                IMutableProperty uiCultureProperty = entityType.GetUICultureProperty();
                if (uiCultureProperty != null)
                {
                    ReplaceIndexPrefix(entityType.GetOrAddIndex(uiCultureProperty));
                }
            }
        }

	    private static void CreateCollectionOrderIndex(IMutableEntityType entityType)
	    {
		    IMutableProperty deletedProperty = entityType.GetDeletedProperty();

			// Find matching navigations using foreign keys
			// We need navigations, since we want to extract Order By properties from Collection attribute (Sorting property)
		    var collectionsIntoEntity = entityType.GetForeignKeys()
			    .Select(fk => fk.PrincipalEntityType.GetNavigations().FirstOrDefault(n => n.ForeignKey == fk))
			    .Where(n => n != null)
			    .ToArray();

			foreach (IMutableNavigation navigation in collectionsIntoEntity)
		    {
			    string sorting = new CollectionAttributeAccessor(navigation).Sorting;
			    if (string.IsNullOrEmpty(sorting))
			    {
				    continue;
			    }

			    List<string> orderByProperties = Regex.Matches(sorting, "(^|[^{])({([^{}]*)}|\\[([^\\[\\]]*)\\])")
				    .Cast<Match>()
				    .Where(m => m.Success && m.Groups[4].Success)
				    .Select(m => m.Groups[4].Value)
				    .ToList();

			    if (orderByProperties.Count == 0)
			    {
				    continue;
			    }

			    orderByProperties.Remove(navigation.ForeignKey.Properties[0].Name);
			    orderByProperties.Insert(0, navigation.ForeignKey.Properties[0].Name);

			    List<IMutableProperty> indexProperties = orderByProperties.Select(entityType.FindProperty).ToList();

			    if (deletedProperty != null)
			    {
				    indexProperties.Add(deletedProperty);
			    }

			    entityType.GetOrAddIndex(indexProperties);
		    }
	    }

	    private static void RenameForeignKeyIndexes(IEnumerable<IMutableForeignKey> foreignKeys)
	    {
		    foreach (IMutableIndex index in foreignKeys
			    .Select(k => k.DeclaringEntityType.FindIndex(k.Properties))
			    .Where(index => index != null))
		    {
		        ReplaceIndexPrefix(index);
		    }
	    }

	    private static void ReplaceIndexPrefix(IMutableIndex index)
        {
            if (index.Relational().Name.StartsWith("IX_"))
            {
                index.Relational().Name = "FKX_" + index.Relational().Name.Substring(3);
            }
        }

	    private static bool ShouldGenerateIndexes(MemberInfo memberInfo) => memberInfo.GetCustomAttribute<GenerateIndexesAttribute>()?.GenerateIndexes ?? true;
    }
}