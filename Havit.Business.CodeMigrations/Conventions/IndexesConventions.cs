using System.Collections.Generic;
using System.Linq;
using Havit.Business.CodeMigrations.ExtendedProperties;
using Havit.Business.CodeMigrations.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Business.CodeMigrations.Conventions
{
    public static class IndexesConventions
    {
        private const string GenerateIndexesPropertyName = "GenerateIndexes";

        public static void ApplyBusinessLayerIndexes(this ModelBuilder modelBuilder)
        {
            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes()
                .Where(e => e.GetBoolExtendedProperty(GenerateIndexesPropertyName) ?? true))
            {
	            RenameForeignKeyIndexes(entityType.GetForeignKeys());

                if (entityType.IsJoinEntity())
                {
					// TODO
                }
                else
                {
                    AddNormalTableIndexes(entityType);
                }
            }
        }

        private static void AddNormalTableIndexes(IMutableEntityType entityType)
        {
            IMutableProperty deletedProperty = entityType.GetDeletedProperty();

            foreach (IMutableProperty property in entityType.GetNotIgnoredProperties()
                .Where(e => e.GetBoolExtendedProperty(GenerateIndexesPropertyName) ?? true))
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

            if (entityType.IsLocalizationEntity())
            {
                IMutableProperty parentLocalizationProperty = entityType.FindProperty(entityType.GetLocalizationParentEntityType().FindPrimaryKey().Properties[0].Name);
                IMutableProperty languageProperty = entityType.GetLanguageProperty();

                ReplaceIndexPrefix(entityType.GetOrAddIndex(new[] { parentLocalizationProperty, languageProperty }));
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
    }
}