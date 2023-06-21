using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Reflection;
using Havit.Data.Entity.Annotations;
using Havit.Data.Entity.ModelConfiguration.Configuration;

namespace Havit.Data.Entity.ModelConfiguration;

/// <summary>
/// Extension metody k třídě EntityTypeConfiguration.
/// </summary>
public static class EntityTypeConfigurationExtensions
{
	/// <summary>
	/// Zajistí vytvoření indexu s daným jménem na daných vlastnostech (sloupcích).
	/// </summary>
	public static void HasIndex<TEntity>(this EntityTypeConfiguration<TEntity> configuration, string indexName, params PrimitivePropertyConfiguration[] properties)
		where TEntity : class
	{
		HasIndexInternal(indexName, properties, false);
	}

	/// <summary>
	/// Zajistí vytvoření unikátního indexu s daným jménem na daných vlastnostech (sloupcích).
	/// </summary>
	public static void HasUniqueIndex<TEntity>(this EntityTypeConfiguration<TEntity> configuration, string indexName, params PrimitivePropertyConfiguration[] properties)
		where TEntity : class
	{
		HasIndexInternal(indexName, properties, true);						
	}

	private static void HasIndexInternal(string indexName, PrimitivePropertyConfiguration[] properties, bool isUnique)
	{
		for (int i = 0; i < properties.Length; i++)
		{				
			IndexAttribute newIndex = new IndexAttribute(indexName, i + 1) { IsUnique = isUnique };

			IndexAnnotation indexAnnotation = (IndexAnnotation)properties[i].GetAnnotation(IndexAnnotation.AnnotationName);
			if (indexAnnotation != null)
			{
				var indexes = indexAnnotation.Indexes.ToList();
				indexes.Add(newIndex);
				indexAnnotation = new IndexAnnotation(indexes);
			}
			else
			{
				indexAnnotation = new IndexAnnotation(newIndex);
			}

			properties[i].HasColumnAnnotation(IndexAnnotation.AnnotationName, indexAnnotation);
		}
	}

	/// <summary>
	/// Přidá ke konfiguraci anotaci indikující požadavek na nepoužití určité konvence.
	/// </summary>
	public static void HasSuppressedConvention<TEntity>(this EntityTypeConfiguration<TEntity> configuration, Type conventionType)
		where TEntity : class
	{
		SuppressConventionAnnotation suppressConventionAnnotation = (SuppressConventionAnnotation)configuration.GetAnnotation("HasSuppressedConvention");
		if (suppressConventionAnnotation != null)
		{
			suppressConventionAnnotation.AddSupressedConvention(conventionType);	
		}
		else
		{
			suppressConventionAnnotation = new SuppressConventionAnnotation();
			suppressConventionAnnotation.AddSupressedConvention(conventionType);
			configuration.HasTableAnnotation("HasSuppressedConvention", suppressConventionAnnotation);
		}
	}

	internal static object GetAnnotation<TEntity>(this EntityTypeConfiguration<TEntity> entityTypeConfiguration, string annotationName)
		where TEntity : class
	{
		var configuration = entityTypeConfiguration.GetType()
			.GetProperty("Configuration", BindingFlags.Instance | BindingFlags.NonPublic)
			.GetValue(entityTypeConfiguration, null);

		var annotations = (IDictionary<string, object>)configuration.GetType()
			.GetProperty("Annotations", BindingFlags.Instance | BindingFlags.Public)
			.GetValue(configuration, null);

		object annotation;
		if (annotations.TryGetValue(annotationName, out annotation))
		{
			return annotation;
		}
		return null;
	}
}