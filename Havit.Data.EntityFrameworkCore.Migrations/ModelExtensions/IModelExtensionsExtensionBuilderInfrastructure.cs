using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions
{
	/// <summary>
	///     Explicitly implemented by <see cref="ModelExtensionsExtensionBuilder" /> and <see cref="ModelExtensionsExtensionBuilderBase"/> to hide
	///     methods that are used by Model Extension extension methods but not intended to be called by application
	///     developers.
	/// </summary>
	public interface IModelExtensionsExtensionBuilderInfrastructure
	{
		/// <summary>
		///     Gets the core options builder.
		/// </summary>
		DbContextOptionsBuilder OptionsBuilder { get; }
	}
}
