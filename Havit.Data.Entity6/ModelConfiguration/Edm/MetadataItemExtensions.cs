using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Reflection;
using Havit.Data.Entity.Annotations;

namespace Havit.Data.Entity.ModelConfiguration.Edm
{
	internal static class MetadataItemExtensions
	{
		public static object GetAnnotation(this MetadataItem property, string annotationName)
		{
			var annotations = (IEnumerable<MetadataProperty>)typeof(MetadataItem).GetProperty("Annotations", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(property);
			MetadataProperty metadataProperty = annotations.FirstOrDefault(item => item.Name == annotationName);
			return metadataProperty?.Value;
		}

		public static bool IsConventionSuppressed(this MetadataItem metadataItem, Type conventionType)
		{
			return SuppressConventionAnnotation.IsConventionSuppressed(metadataItem, conventionType);
		}
	}
}
