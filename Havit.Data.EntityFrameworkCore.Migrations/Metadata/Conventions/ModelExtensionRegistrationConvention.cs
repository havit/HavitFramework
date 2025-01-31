using System.Reflection;
using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Havit.Data.EntityFrameworkCore.Migrations.Metadata.Conventions;

/// <summary>
/// Internal convention of Model Extensions feature. Discovers available <see cref="IModelExtender"/>s in configured assembly (see <see cref="IModelExtensionsAssembly"/>).
/// </summary>
/// <remarks>
/// Not intended to be used by application code.
/// </remarks>
public class ModelExtensionRegistrationConvention : IModelFinalizingConvention
{
	private readonly IModelExtensionsAssembly _modelExtensionsAssembly;
	private readonly IModelExtensionAnnotationProvider _modelExtensionAnnotationProvider;

	/// <summary>
	/// Constructor
	/// </summary>
	public ModelExtensionRegistrationConvention(IServiceProvider serviceProvider)
	{
		//Contract.Requires<ArgumentNullException>(modelExtensionsAssembly != null);

		this._modelExtensionsAssembly = (IModelExtensionsAssembly)serviceProvider.GetService(typeof(IModelExtensionsAssembly));
		this._modelExtensionAnnotationProvider = (IModelExtensionAnnotationProvider)serviceProvider.GetService(typeof(IModelExtensionAnnotationProvider));
	}

	/// <inheritdoc />
	public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
	{
		foreach (TypeInfo modelExtenderClass in _modelExtensionsAssembly.ModelExtenders)
		{
			IModelExtender extender = _modelExtensionsAssembly.CreateModelExtender(modelExtenderClass);

			IEnumerable<MethodInfo> publicMethods = modelExtenderClass.GetMethods(BindingFlags.Instance | BindingFlags.Public)
				.Where(m => typeof(IModelExtension).IsAssignableFrom(m.ReturnType));

			foreach (MethodInfo method in publicMethods)
			{
				var modelExtension = (IModelExtension)method.Invoke(extender, new object[0]);

				List<IAnnotation> annotations = _modelExtensionAnnotationProvider.GetAnnotations(modelExtension, method);

				annotations.ForEach(a => modelBuilder.HasAnnotation(a.Name, a.Value, false));
			}
		}
	}
}
