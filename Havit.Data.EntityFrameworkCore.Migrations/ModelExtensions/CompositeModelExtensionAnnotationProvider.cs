using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions
{
	/// <summary>
	/// Composite implementation of <see cref="IModelExtensionAnnotationProvider"/>.
	/// </summary>
	public class CompositeModelExtensionAnnotationProvider : IModelExtensionAnnotationProvider
	{
		private readonly IEnumerable<IModelExtensionAnnotationProvider> providers;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public CompositeModelExtensionAnnotationProvider(IEnumerable<IModelExtensionAnnotationProvider> providers)
		{
			this.providers = providers;
		}

		/// <inheritdoc />
		public List<IAnnotation> GetAnnotations(IModelExtension dbAnnotation, MemberInfo memberInfo)
		{
			return providers.SelectMany(provider => provider.GetAnnotations(dbAnnotation, memberInfo)).ToList();
		}

		/// <inheritdoc />
		public List<IModelExtension> GetModelExtensions(List<IAnnotation> annotations)
		{
			return providers.SelectMany(provider => provider.GetModelExtensions(annotations)).ToList();
		}
	}
}