using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Havit.Data.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Metadata
{
    public static class EntityTypeExtensions
    {
	    /// <summary>
	    /// Vrací true, pokud jde o systémovou entitu, tj. entitu zaregistrovanou HFW automaticky.
	    /// </summary>
	    public static bool IsSystemEntity(this IEntityType entityType)
	    {
		    return entityType.ClrType == typeof(DataSeedVersion);
	    }

	    /// <summary>
	    /// Vrací true, pokud je entita vztahovou entitou M:N vztahu.
	    /// </summary>
	    public static bool IsManyToManyEntity(this IEntityType entityType)
	    {
		    // GetProperties neobsahuje vlastnosti z nadřazených tříd, v tomto scénáři to nevadí, dědičnost pro tabulky se dvěma sloupci primárního klíče neuvažujeme
		    return (entityType.FindPrimaryKey().Properties.Count == 2) // třída má složený primární klíč ze svou vlastností
		           && (entityType.GetProperties().Count() == 2) // třída má právě dvě (skalární) vlastnosti
		           && (entityType.GetProperties().All(item => item.IsForeignKey())); // všechny vlastnosti třídy jsou cizím klíčem
	    }
	}
}
