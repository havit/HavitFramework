using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes.ExtendedProperties;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata
{
	/// <summary>
	/// Extension metody k IEntityType pro podporu faktů v Business Layer.
	/// </summary>
    public static class BusinessLayerEntityTypeExtensions
    {
		/// <summary>
		/// Vrátí sloupec reprezentující příznak smazání záznamu.
		/// </summary>
        public static IProperty GetBusinessLayerDeletedProperty(this IEntityType entityType)
        {
            Contract.Requires<ArgumentNullException>(entityType != null);

            return GetBusinessLayerNotIgnoredProperties(entityType).FirstOrDefault(p => (p.Name == "Deleted") && (p.ClrType == typeof(bool) || p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?)));
        }

		/// <summary>
		/// Vrátí neignorované sloupce (vlastnosti).
		/// </summary>
		public static IEnumerable<IProperty> GetBusinessLayerNotIgnoredProperties(this IEntityType entityType)
        {
            Contract.Requires<ArgumentNullException>(entityType != null);

            foreach (IProperty property in entityType.GetProperties())
            {
                if (!property.GetExtendedProperties().ContainsKey(IgnoredAttribute.ExtendedPropertyName))
                {
                    yield return property;
                }
            }
        }

		/// <summary>
		/// Indikuje, zda jde o entitu reprezentující vztah Many-to-Many.
		/// Podporuje extravaganci BL v tom smyslu, kdy můžeme mít v tabulce dva cizí klíče ve složeném primárním klíči a také několik dalších ignorovaných sloupců.
		/// </summary>
        public static bool IsBusinessLayerManyToManyEntity(this IEntityType entityType)
        {
            Contract.Requires<ArgumentNullException>(entityType != null);

            return GetBusinessLayerNotIgnoredProperties(entityType).Count(p => p.IsPrimaryKey() && p.IsForeignKey()) == 2;
        }
    }
}