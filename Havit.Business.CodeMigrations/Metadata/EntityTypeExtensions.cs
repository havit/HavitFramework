using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Business.CodeMigrations.ExtendedProperties;
using Havit.Business.CodeMigrations.ExtendedProperties.Attributes;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Business.CodeMigrations.Metadata
{
    public static class EntityTypeExtensions
    {
        public static IMutableProperty GetDeletedProperty(this IMutableEntityType entityType)
        {
            Contract.Requires<ArgumentNullException>(entityType != null);

            return GetNotIgnoredProperties(entityType).FirstOrDefault(p => (p.Name == "Deleted") && (p.ClrType == typeof(bool) || p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?)));
        }

        public static IEnumerable<IMutableProperty> GetNotIgnoredProperties(this IMutableEntityType entityType)
        {
            Contract.Requires<ArgumentNullException>(entityType != null);

            foreach (IMutableProperty property in entityType.GetProperties())
            {
                if (!property.GetExtendedProperties().ContainsKey(IgnoredAttribute.PropertyName))
                {
                    yield return property;
                }
            }
        }

        public static bool IsJoinEntity(this IMutableEntityType entityType)
        {
            Contract.Requires<ArgumentNullException>(entityType != null);

            return GetNotIgnoredProperties(entityType).Count(p => p.IsPrimaryKey() && p.IsForeignKey()) == 2;
        }
    }
}