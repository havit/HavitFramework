using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.Conventions;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Conventions
{
	public class SequencesForEnumClassesConvention : IModelConvention
	{
		private readonly Func<string, string> namingConvention;

		public SequencesForEnumClassesConvention(Func<string, string> namingConvention = null)
		{
			this.namingConvention = namingConvention;
		}

		public void Apply(ModelBuilder modelBuilder)
		{
			Contract.Requires<ArgumentNullException>(modelBuilder != null);

			var currentNamingConvention = namingConvention ?? (entityTypeName => $"sq_{entityTypeName}");

			IEnumerable<IMutableEntityType> enumClassEntities = modelBuilder.Model.GetEntityTypes().Where(e => e.ClrType.GetCustomAttributes<EnumClassAttribute>().Any());

			foreach (IMutableEntityType entityType in enumClassEntities)
			{
				var pkProperty = entityType.FindPrimaryKey().Properties[0];

				string sequenceName = currentNamingConvention(entityType.ShortName());
				modelBuilder.HasSequence(pkProperty.ClrType, sequenceName);

				pkProperty.Relational().DefaultValueSql = $"NEXT VALUE FOR {sequenceName}";
			}
		}
	}
}