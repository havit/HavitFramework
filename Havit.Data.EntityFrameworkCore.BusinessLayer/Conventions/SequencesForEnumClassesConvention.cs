using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Havit.Business.CodeMigrations.ExtendedProperties.Attributes;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Havit.Business.CodeMigrations.Conventions
{
	public static class SequencesForEnumClassesConvention
	{
		public static void UseSequencesForEnumClasses(this ModelBuilder modelBuilder, Func<string, string> namingConvention = null)
		{
			Contract.Requires<ArgumentNullException>(modelBuilder != null);

			namingConvention = namingConvention ?? (entityTypeName => $"sq_{entityTypeName}");

			IEnumerable<IMutableEntityType> enumClassEntities = modelBuilder.Model.GetEntityTypes().Where(e => e.ClrType.GetCustomAttributes<EnumClassAttribute>().Any());

			foreach (IMutableEntityType entityType in enumClassEntities)
			{
				var pkProperty = entityType.FindPrimaryKey().Properties[0];

				string sequenceName = namingConvention(entityType.ShortName());
				modelBuilder.HasSequence(pkProperty.ClrType, sequenceName);

				pkProperty.Relational().DefaultValueSql = $"NEXT VALUE FOR {sequenceName}";
			}
		}
	}
}