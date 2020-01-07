using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Infrastructure.Factories
{
	internal class BeforeCommitProcessorsFactory : IBeforeCommitProcessorsFactory
	{
		private readonly IServiceProvider serviceProvider;

		public BeforeCommitProcessorsFactory(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
		}

		public IEnumerable<IBeforeCommitProcessor<TEntity>> Create<TEntity>()
			where TEntity : class
		{
			return serviceProvider.GetRequiredService<IEnumerable<IBeforeCommitProcessor<TEntity>>>();
		}

		public void Release<TEntity>(IEnumerable<IBeforeCommitProcessor<TEntity>> services)
			where TEntity : class
		{
			// NOOP
		}
	}
}
