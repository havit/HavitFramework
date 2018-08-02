using System.Collections.Generic;
using System.Linq;
using Havit.Business.CodeMigrations.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Havit.Business.CodeMigrations.ExtendedProperties.ExtendedPropertiesAnnotationsHelper;

namespace Havit.Business.CodeMigrations.ExtendedProperties
{
	public static class ExtendedPropertiesExtensions
	{
		public static void UseSqlServerExtendedProperties(this DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.Options.GetExtension<CompositeMigrationsAnnotationProviderExtension>().WithAnnotationProvider<ExtendedPropertiesMigrationsAnnotationProvider>();
			optionsBuilder.Options.GetExtension<CompositeMigrationsSqlGeneratorExtension>().WithGeneratorType<ExtendedPropertiesMigrationsSqlGenerator>();
		}

		public static void ForSqlServerExtendedPropertiesAttributes(this ModelBuilder modelBuilder)
		{
			foreach (var entityType in modelBuilder.Model.GetEntityTypes())
			{
				AddExtendedPropertyAnnotations(entityType, entityType.ClrType);
				foreach (var property in entityType.GetProperties().Where(x => !x.IsShadowProperty))
				{
					AddExtendedPropertyAnnotations(property, property.PropertyInfo);
				}
				foreach (var navigation in entityType.GetNavigations())
				{
					AddExtendedPropertyAnnotations(entityType, navigation.PropertyInfo);
				}
			}
		}

		public static void AddExtendedProperties(this IMutableAnnotatable annotatable, IDictionary<string, string> extendedProperties)
		{
			AddExtendedPropertyAnnotations(annotatable, extendedProperties);
		}

		public static void AddExtendedProperties(this EntityTypeBuilder entityTypeBuilder, IDictionary<string, string> extendedProperties)
			=> entityTypeBuilder.Metadata.AddExtendedProperties(extendedProperties);

		public static void AddExtendedProperties(this PropertyBuilder propertyBuilder, IDictionary<string, string> extendedProperties)
			=> propertyBuilder.Metadata.AddExtendedProperties(extendedProperties);

		public static void AddExtendedProperties(this ModelBuilder modelBuilder, IDictionary<string, string> extendedProperties)
			=> modelBuilder.Model.AddExtendedProperties(extendedProperties);
	}
}
