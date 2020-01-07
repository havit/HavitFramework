using Havit.Data.Patterns.DataSources;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Infrastructure.Factories
{
	internal class DataSourceFactory<TEntity> : IDataSourceFactory<TEntity>
		where TEntity : class
	{
		private readonly IServiceProvider serviceProvider;

		public DataSourceFactory(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
		}

		public IDataSource<TEntity> Create()
		{
			return serviceProvider.GetRequiredService<IDataSource<TEntity>>();
		}

		public void Release(IDataSource<TEntity> service)
		{
			// NOOP
		}
	}
}
