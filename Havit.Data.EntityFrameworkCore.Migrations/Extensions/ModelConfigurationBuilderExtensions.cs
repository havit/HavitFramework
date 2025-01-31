using Havit.Data.EntityFrameworkCore.Migrations.Metadata.Conventions;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Migrations.Extensions;
/// <summary>
/// Havit.Data.EntityFrameworkCore.Migrations extension methods for <see cref="ModelConfigurationBuilder"/>.
/// </summary>
public static class ModelConfigurationBuilderExtensions
{
	/// <summary>
	/// Adds ModelExtensionRegistrationConvention to <paramref name="configurationBuilder"/>. This adds support for ModelExtenders
	/// </summary>
	/// <param name="configurationBuilder"></param>
	public static void AddModelExtensionRegistrationConvention(this ModelConfigurationBuilder configurationBuilder)
	{
		configurationBuilder.Conventions.Add((IServiceProvider serviceProvider) => new ModelExtensionRegistrationConvention(serviceProvider));
	}
}
