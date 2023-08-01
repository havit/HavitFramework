using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Caching;

/// <summary>
/// Installer, která zaregistruje službu, která nic necachuje (NoCachingEntityCacheManager). 
/// </summary>
public sealed class NoCachingInstaller : ICachingInstaller
{
	/// <inheritdoc />
	public void Install(IServiceCollection services)
	{
		services.AddSingleton<IEntityCacheManager, NoCachingEntityCacheManager>();
	}
}
