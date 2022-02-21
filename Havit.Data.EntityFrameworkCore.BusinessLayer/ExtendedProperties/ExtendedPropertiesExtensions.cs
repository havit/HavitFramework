using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Data.EntityFrameworkCore.Migrations.Infrastructure.ModelExtensions;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties
{
	public static class ExtendedPropertiesExtensions
	{
		public static void UseSqlServerExtendedProperties(this DbContextOptionsBuilder optionsBuilder)
		{
			Contract.Requires<ArgumentNullException>(optionsBuilder != null);

			IDbContextOptionsBuilderInfrastructure builder = optionsBuilder;

			builder.AddOrUpdateExtension(optionsBuilder.Options.FindExtension<CompositeMigrationsAnnotationProviderExtension>());
			builder.AddOrUpdateExtension(optionsBuilder.Options.FindExtension<CompositeRelationalAnnotationProviderExtension>()
				.WithAnnotationProvider<ExtendedPropertiesRelationalAnnotationProvider>());
			builder.AddOrUpdateExtension(optionsBuilder.Options.FindExtension<CompositeMigrationsSqlGeneratorExtension>()
				.WithGeneratorType<ExtendedPropertiesMigrationOperationSqlGenerator>());
		}

		public static IDictionary<string, string> GetExtendedProperties(this IReadOnlyAnnotatable annotatable)
		{
			Contract.Requires<ArgumentNullException>(annotatable != null);

			return annotatable.GetAnnotations().Where(ExtendedPropertiesAnnotationsHelper.IsExtendedPropertyAnnotation)
				.ToDictionary(ExtendedPropertiesAnnotationsHelper.ParseAnnotationName, a => (string)a.Value);
	    }

		public static string GetStringExtendedProperty(this IReadOnlyAnnotatable annotatable, string key)
		{
			GetExtendedProperties(annotatable).TryGetValue(key, out string value);
			return value;
		}

	    public static bool? GetBoolExtendedProperty(this IReadOnlyAnnotatable annotatable, string key)
	    {
	        if (!GetExtendedProperties(annotatable).TryGetValue(key, out string value))
	        {
	            return null;
	        }

	        if (value.ToLowerInvariant() == "true" || value == "1")
	        {
	            return true;
	        }

	        if (value.ToLowerInvariant() == "false" || value == "0")
	        {
	            return false;
	        }

	        throw new ArgumentException($"Unknown bool value \"{value}\" in extended property {key}.");
        }

		public static void AddExtendedProperties(this IMutableAnnotatable annotatable, IDictionary<string, string> extendedProperties)
		{
			ExtendedPropertiesAnnotationsHelper.AddExtendedPropertyAnnotations(annotatable, extendedProperties);
		}

		public static void AddExtendedProperties(this EntityTypeBuilder entityTypeBuilder, IDictionary<string, string> extendedProperties)
		{
			entityTypeBuilder.Metadata.AddExtendedProperties(extendedProperties);
		}

		public static void AddExtendedProperties(this PropertyBuilder propertyBuilder, IDictionary<string, string> extendedProperties)
		{
			propertyBuilder.Metadata.AddExtendedProperties(extendedProperties);
		}

		public static void AddExtendedProperties(this ModelBuilder modelBuilder, IDictionary<string, string> extendedProperties)
		{
			modelBuilder.Model.AddExtendedProperties(extendedProperties);
		}

		public static void AddExtendedProperties(this IConventionAnnotatable annotable, IDictionary<string, string> extendedProperties, bool fromDataAnnotation = false)
		{
			ExtendedPropertiesAnnotationsHelper.AddExtendedPropertyAnnotations(annotable, extendedProperties, fromDataAnnotation);
		}
	}
}
