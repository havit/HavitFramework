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
    public static class EntityTypeExtensions
    {
        public static IMutableProperty GetBusinessLayerDeletedProperty(this IEntityType entityType)
        {
            Contract.Requires<ArgumentNullException>(entityType != null);

            return GetBusinessLayerNotIgnoredProperties(entityType).FirstOrDefault(p => (p.Name == "Deleted") && (p.ClrType == typeof(bool) || p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?)));
        }

        public static IEnumerable<IMutableProperty> GetBusinessLayerNotIgnoredProperties(this IEntityType entityType)
        {
            Contract.Requires<ArgumentNullException>(entityType != null);

            foreach (IMutableProperty property in entityType.GetProperties())
            {
                if (!property.GetExtendedProperties().ContainsKey(IgnoredAttribute.ExtendedPropertyName))
                {
                    yield return property;
                }
            }
        }

        public static bool IsBusinessLayerManyToManyEntity(this IEntityType entityType)
        {
            Contract.Requires<ArgumentNullException>(entityType != null);

            return GetBusinessLayerNotIgnoredProperties(entityType).Count(p => p.IsPrimaryKey() && p.IsForeignKey()) == 2;
        }
    }
}