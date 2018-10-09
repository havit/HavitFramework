using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Metadata
{
	/// <summary>
	/// Extension metody k IEntityType.
	/// </summary>
    public static class EntityTypeExtensions
    {
	    /// <summary>
	    /// Vrací true, pokud jde o systémovou entitu, tj. entitu zaregistrovanou HFW automaticky.
	    /// </summary>
	    public static bool IsSystemType(this IEntityType entityType)
	    {
			return entityType.ClrType == typeof(Havit.Data.EntityFrameworkCore.Model.DataSeedVersion);
		}

		internal static bool HasExactlyTwoPropertiesWhichAreAlsoForeignKeys(this IEntityType entityType)
		{
			return (entityType.GetProperties().Count() == 2) // třída má právě dvě (skalární) vlastnosti
				&& (entityType.GetProperties().All(item => item.IsForeignKey())); // všechny vlastnosti třídy jsou cizím klíčem
		}

		/// <summary>
		/// Vrací true, pokud je entita vztahovou entitou M:N vztahu.
		/// </summary>
		public static bool IsManyToManyEntity(this IEntityType entityType)
	    {
			// GetProperties neobsahuje vlastnosti z nadřazených tříd, v tomto scénáři to nevadí, dědičnost pro tabulky se dvěma sloupci primárního klíče neuvažujeme
			return !entityType.IsOwned()
				&& !entityType.IsQueryType
				&& (entityType.FindPrimaryKey()?.Properties.Count == 2) // třída má složený primární klíč ze svou vlastností
				&& HasExactlyTwoPropertiesWhichAreAlsoForeignKeys(entityType); // třída má právě dvě (skalární) vlastnosti a ty jsou i cizím klíčem
		}

		/// <summary>
		/// Vrací true, pokud jde o aplikační entitu - není systémová, nejde o QueryType a není Owned.
		/// </summary>
		/// <param name="entityType"></param>
		/// <returns></returns>
		internal static bool IsApplicationEntity(this IEntityType entityType)
		{
			return !entityType.IsQueryType
				&& !entityType.IsOwned()
				&& !entityType.IsSystemType();
		}
	}
}
