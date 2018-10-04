using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Reflection;

namespace Havit.Data.Entity.ModelConfiguration.Configuration
{
	internal static class PrimitivePropertyConfigurationExtensions
	{
		public static object GetAnnotation(this PrimitivePropertyConfiguration property, string annotationName)
		{
			// převzato z https://github.com/mj1856/EntityFramework.IndexingExtensions/blob/master/EntityFramework.IndexingExtensions/IndexingExtensions.cs
			var configuration = typeof(PrimitivePropertyConfiguration)
				.GetProperty("Configuration", BindingFlags.Instance | BindingFlags.NonPublic)
				.GetValue(property, null);

			var annotations = (IDictionary<string, object>)configuration.GetType()
				.GetProperty("Annotations", BindingFlags.Instance | BindingFlags.Public)
				.GetValue(configuration, null);

			object annotation;
			if (annotations.TryGetValue(IndexAnnotation.AnnotationName, out annotation))
			{
				return annotation;
			}
			return null;
		}
	}
}
