using System.Collections.Immutable;
using System.Reflection;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;

/// <inheritdoc />
public class ModelExtensionsAssembly : IModelExtensionsAssembly
{
	private IReadOnlyCollection<TypeInfo> _modelExtenders;

	/// <summary>
	/// Constructor.
	/// </summary>
	public ModelExtensionsAssembly(
		ICurrentDbContext currentDbContext,
		IDbContextOptions dbContextOptions)
	{
		Contract.Requires<ArgumentNullException>(currentDbContext != null);
		Contract.Requires<ArgumentNullException>(dbContextOptions != null);

		Assembly = dbContextOptions.FindExtension<ModelExtensionsExtension>()?.ExtensionsAssembly ??
				   currentDbContext.Context.GetType().GetTypeInfo().Assembly;
	}

	/// <inheritdoc />
	public IReadOnlyCollection<TypeInfo> ModelExtenders
	{
		get
		{
			IReadOnlyCollection<TypeInfo> Create()
			{
				return Assembly != null ?
					Assembly.DefinedTypes.Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition && t.GetInterface(nameof(IModelExtender)) != null).ToImmutableArray() :
					ImmutableArray<TypeInfo>.Empty;
			}

			return _modelExtenders ??= Create();
		}
	}

	/// <inheritdoc />
	public Assembly Assembly { get; }

	/// <inheritdoc />
	public IModelExtender CreateModelExtender(TypeInfo modelExtenderClass)
	{
		Contract.Requires<ArgumentNullException>(modelExtenderClass != null);

		return (IModelExtender)Activator.CreateInstance(modelExtenderClass.AsType());
	}
}