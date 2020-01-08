using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Infrastructure.Factories
{
	internal class DbContextFactory : IDbContextFactory
	{
		private readonly IServiceProvider serviceProvider;

		public DbContextFactory(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
		}

		public IDbContext CreateService()
		{
			return serviceProvider.GetRequiredService<IDbContext>();
		}

		public void ReleaseService(IDbContext service)
		{
			// NOOP
		}
	}
}
