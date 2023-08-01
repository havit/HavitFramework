using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;

/// <summary>
/// A service representing an assembly containing Model Extenders.
/// </summary>
/// <remarks>
/// Modeled after IMigrationsAssembly in EF Core code base.
/// </remarks>
public interface IModelExtensionsAssembly
{
	/// <summary>
	/// A collection of <see cref="TypeInfo"/> of the classes that represent model extenders.
	/// </summary>
	IReadOnlyCollection<TypeInfo> ModelExtenders { get; }

	/// <summary>
	/// The assembly that contains model extenders.
	/// </summary>
	Assembly Assembly { get; }

	/// <summary>
	/// Creates an instance of the model extender class.
	/// </summary>
	/// <param name="modelExtenderClass">The <see cref="TypeInfo"/> for the model extender class, as obtained from the <see cref="ModelExtenders" /> collection.</param>
	/// <returns>The model extender instance.</returns>
	IModelExtender CreateModelExtender(TypeInfo modelExtenderClass);
}