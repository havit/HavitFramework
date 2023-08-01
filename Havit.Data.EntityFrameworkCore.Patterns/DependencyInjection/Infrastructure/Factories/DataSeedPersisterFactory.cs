using Havit.Data.Patterns.DataSeeds;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Infrastructure.Factories;

internal class DataSeedPersisterFactory : IDataSeedPersisterFactory
{
	private readonly IServiceProvider serviceProvider;

	public DataSeedPersisterFactory(IServiceProvider serviceProvider)
	{
		this.serviceProvider = serviceProvider;
	}

	public IDataSeedPersister CreateService()
	{
		return serviceProvider.GetRequiredService<IDataSeedPersister>();
	}

	public void ReleaseService(IDataSeedPersister service)
	{
		// NOOP
	}
}
