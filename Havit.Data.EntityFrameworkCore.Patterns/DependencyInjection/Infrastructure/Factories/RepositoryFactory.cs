using Havit.Data.Patterns.DataSources;
using Havit.Data.Patterns.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Infrastructure.Factories
{
	internal class RepositoryFactory<TEntity> : IRepositoryFactory<TEntity>
		where TEntity : class
	{
		private readonly IServiceProvider serviceProvider;

		public RepositoryFactory(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
		}

		public IRepository<TEntity> Create()
		{
			return serviceProvider.GetRequiredService<IRepository<TEntity>>();
		}

		public void Release(IRepository<TEntity> service)
		{
			// NOOP
		}
	}
}
