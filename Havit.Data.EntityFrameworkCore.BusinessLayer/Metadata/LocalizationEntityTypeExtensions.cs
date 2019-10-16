using System;
using System.Linq;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata
{
	/// <summary>
	/// Extensinsion metody k EntityType s fakty pro oblast lokalizací v Business Layer.
	/// </summary>
	public static class LocalizationEntityTypeExtensions
    {
		/// <summary>
		/// Seznam suffixů názvu tabulek indikující tabulku s lokalizacemi.
		/// </summary>
        private static readonly string[] LocalizationEntityNameSuffixes = { "Localization", "_Lang" };
		
		/// <summary>
		/// Název cizího klíče pro lokalizace.
		/// </summary>
        private const string LanguageForeignKeyPropertyName = "LanguageId";

		/// <summary>
		/// Vrací true, pokud jde o lokalizační (nikoliv lokalizovanou) tabulku.
		/// </summary>
        public static bool IsBusinessLayerLocalizationEntity(this IEntityType entityType)
        {
            Contract.Requires<ArgumentNullException>(entityType != null);

            return LocalizationEntityNameSuffixes
                .Any(localizationTableNameSuffix => entityType.Name.EndsWith(localizationTableNameSuffix) && (entityType.Name.Length > localizationTableNameSuffix.Length));
        }

		/// <summary>
		/// pro lokalizační tabulku vrací jí lokalizovanou tabulku.
		/// </summary>
        public static IEntityType GetBusinessLayerLocalizationParentEntityType(this IEntityType localizationEntity)
        {
            Contract.Requires<ArgumentNullException>(localizationEntity != null);

            string localizationEntityName = localizationEntity.Name;

            foreach (string localizationEntityNameSuffix in LocalizationEntityNameSuffixes)
            {
                if (localizationEntityName.EndsWith(localizationEntityNameSuffix))
                {
                    string parentName = localizationEntityName.Substring(0, localizationEntityName.Length - localizationEntityNameSuffix.Length);
                    IEntityType result = localizationEntity.Model.FindEntityType(parentName);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            return null;
        }

		/// <summary>
		/// Vrací sloupec definující jazyk lokalizace.
		/// </summary>
        public static IProperty GetBusinessLayerLanguageProperty(this IEntityType entityType)
        {
            Contract.Requires<ArgumentNullException>(entityType != null);

            return entityType.FindProperty(LanguageForeignKeyPropertyName);
        }
    }
}