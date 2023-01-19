using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Migrations.Metadata.Conventions
{
	/// <summary>
	/// Internal <see cref="IConventionSetPlugin"/> for registering <see cref="ModelExtensionRegistrationConvention"/>.
	/// </summary>
	public class ModelExtensionRegistrationConventionPlugin : IConventionSetPlugin
	{
		private readonly IModelExtensionsAssembly modelExtensionsAssembly;
		private readonly IModelExtensionAnnotationProvider modelExtensionAnnotationProvider;

		/// <summary>
		/// Constructor.
		/// </summary>
		public ModelExtensionRegistrationConventionPlugin(
			IModelExtensionsAssembly modelExtensionsAssembly,
			IModelExtensionAnnotationProvider modelExtensionAnnotationProvider)
		{
			this.modelExtensionsAssembly = modelExtensionsAssembly;
			this.modelExtensionAnnotationProvider = modelExtensionAnnotationProvider;
		}

		/// <inheritdoc />
		public ConventionSet ModifyConventions(ConventionSet conventionSet)
		{
			conventionSet.ModelFinalizingConventions.Add(new ModelExtensionRegistrationConvention(modelExtensionsAssembly, modelExtensionAnnotationProvider));

			return conventionSet;
		}
	}
}