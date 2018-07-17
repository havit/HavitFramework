using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Havit.Business.CodeMigrations.ExtendedProperties
{
	internal static class ExtendedPropertiesAnnotationsHelper
	{
		private const string AnnotationMarker = "ExtendedProperty:";

		public static void UseSqlServerExtendedProperties(this DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.ReplaceService<IMigrationsAnnotationProvider, ExtendedPropertiesMigrationsAnnotationProvider>();
			optionsBuilder.ReplaceService<IMigrationsSqlGenerator, ExtendedPropertiesMigrationsSqlGenerator>();
		}

		public static void ForSqlServerExtendedProperties(this ModelBuilder modelBuilder)
		{
			foreach (var entityType in modelBuilder.Model.GetEntityTypes())
			{
				AddExtendedPropertyAnnotations(entityType, entityType.ClrType);
				foreach (var property in entityType.GetProperties())
				{
					AddExtendedPropertyAnnotations(property, property.PropertyInfo);
				}
			}
		}

		public static bool AnnotationsFilter(IAnnotation annotation) => annotation.Name.StartsWith(AnnotationMarker, StringComparison.Ordinal);

		public static string ParseAnnotationName(IAnnotation annotation) => AnnotationsFilter(annotation) ? annotation.Name.Substring(AnnotationMarker.Length) : null;

		internal static string BuildAnnotationName(ExtendedPropertyAttribute attribute) => $"{AnnotationMarker}{attribute.Name}";

		private static void AddExtendedPropertyAnnotations(IMutableAnnotatable annotatable, MemberInfo memberInfo)
		{
			var attributes = memberInfo.GetCustomAttributes(typeof(ExtendedPropertyAttribute), false).Cast<ExtendedPropertyAttribute>();
			foreach (var attribute in attributes)
			{
				annotatable.AddAnnotation(BuildAnnotationName(attribute), attribute.Value);
			}
		}
	}
}
