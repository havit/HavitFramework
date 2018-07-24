using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Havit.Business.CodeMigrations.ExtendedProperties
{
	public static class ExtendedPropertiesExtensions
	{
		public static void UseSqlServerExtendedProperties(this DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.ReplaceService<IMigrationsAnnotationProvider, ExtendedPropertiesMigrationsAnnotationProvider>();
			optionsBuilder.ReplaceService<IMigrationsSqlGenerator, ExtendedPropertiesMigrationsSqlGenerator>();
		}

		public static void ForSqlServerExtendedPropertiesAttributes(this ModelBuilder modelBuilder)
		{
			foreach (var entityType in modelBuilder.Model.GetEntityTypes())
			{
				ExtendedPropertiesAnnotationsHelper.AddExtendedPropertyAnnotations(entityType, entityType.ClrType);
				foreach (var property in entityType.GetProperties())
				{
					ExtendedPropertiesAnnotationsHelper.AddExtendedPropertyAnnotations(property, property.PropertyInfo);
				}
			}
		}

		public static void AddExtendedProperties(this IMutableAnnotatable annotatable, IDictionary<string, string> extendedProperties)
		{
			foreach (var property in extendedProperties)
			{
				ExtendedPropertiesAnnotationsHelper.AddExtendedPropertyAnnotations(annotatable, extendedProperties);
			}
		}

		public static void AddExtendedProperties(this EntityTypeBuilder entityTypeBuilder, IDictionary<string, string> extendedProperties)
			=> entityTypeBuilder.Metadata.AddExtendedProperties(extendedProperties);

		public static void AddExtendedProperties(this PropertyBuilder propertyBuilder, IDictionary<string, string> extendedProperties)
			=> propertyBuilder.Metadata.AddExtendedProperties(extendedProperties);

		public static void AddExtendedProperties(this ModelBuilder modelBuilder, IDictionary<string, string> extendedProperties)
			=> modelBuilder.Model.AddExtendedProperties(extendedProperties);
	}
}
