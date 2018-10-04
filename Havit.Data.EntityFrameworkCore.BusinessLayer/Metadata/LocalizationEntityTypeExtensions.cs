using System;
using System.Linq;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata
{
    public static class LocalizationEntityTypeExtensions
    {
        public static readonly string[] LocalizationEntityNameSuffixes = { "Localization", "_Lang" };
        public const string LanguageForeignKeyPropertyName = "LanguageId";

        public static bool IsLocalizationEntity(this IMutableEntityType entityType)
        {
            Contract.Requires<ArgumentNullException>(entityType != null);

            return LocalizationEntityNameSuffixes
                .Any(localizationTableNameSuffix => entityType.Name.EndsWith(localizationTableNameSuffix) && (entityType.Name.Length > localizationTableNameSuffix.Length));
        }

        public static IMutableEntityType GetLocalizationParentEntityType(this IMutableEntityType localizationEntity)
        {
            Contract.Requires<ArgumentNullException>(localizationEntity != null);

            string localizationEntityName = localizationEntity.Name;

            foreach (string localizationEntityNameSuffix in LocalizationEntityNameSuffixes)
            {
                if (localizationEntityName.EndsWith(localizationEntityNameSuffix))
                {
                    string parentName = localizationEntityName.Substring(0, localizationEntityName.Length - localizationEntityNameSuffix.Length);
                    IMutableEntityType result = localizationEntity.Model.FindEntityType(parentName);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            return null;
        }

        public static IMutableProperty GetLanguageProperty(this IMutableEntityType entityType)
        {
            Contract.Requires<ArgumentNullException>(entityType != null);

            return entityType.FindProperty(LanguageForeignKeyPropertyName);
        }
    }
}