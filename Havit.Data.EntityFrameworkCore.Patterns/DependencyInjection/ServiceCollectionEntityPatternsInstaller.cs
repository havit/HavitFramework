using Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Infrastructure;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection
{
	/// <summary>
	/// Implementace <see cref="IEntityPatternsInstaller"/>u pro IServiceCollection.
	/// </summary>
	public class ServiceCollectionEntityPatternsInstaller : EntityPatternsInstallerBase<ServiceCollectionEntityPatternsInstaller>
	{
        private readonly IServiceCollection services;
        private readonly ComponentRegistrationOptions componentRegistrationOptions;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public ServiceCollectionEntityPatternsInstaller(IServiceCollection services, ComponentRegistrationOptions componentRegistrationOptions) : base(new ServiceCollectionServiceInstaller(services), componentRegistrationOptions)
		{
            this.services = services;
            this.componentRegistrationOptions = componentRegistrationOptions;
        }

		/// <summary>
		/// Registruje do DI containeru DbContext a IDbContextTransient.
		/// </summary>
		public ServiceCollectionEntityPatternsInstaller AddDbContext<TDbContext>(Action<DbContextOptionsBuilder> optionsAction = null)
			where TDbContext : Havit.Data.EntityFrameworkCore.DbContext, IDbContext
		{
			// na pořadí záleží
			services.AddDbContextFactory<TDbContext>(optionsAction);
			services.AddDbContext<IDbContext, TDbContext>(optionsAction);

			services.TryAddSingleton<IDbContextFactory, DbContextFactory<TDbContext>>();
			return this;
		}

		/// <summary>
		/// Registruje do DI containeru DbContext s DbContext poolingem. Dále registruje IDbContextTransient.
		/// </summary>
		public ServiceCollectionEntityPatternsInstaller AddDbContextPool<TDbContext>(Action<DbContextOptionsBuilder> optionsAction, int poolSize = DbContextPool<DbContext>.DefaultPoolSize)
			where TDbContext : Havit.Data.EntityFrameworkCore.DbContext, IDbContext
		{
			//Contract.Requires(componentRegistrationOptions.DbContextLifestyle == ServiceLifetime.Scoped);

			services.AddPooledDbContextFactory<TDbContext>(optionsAction, poolSize);
			services.AddDbContextPool<IDbContext, TDbContext>(optionsAction);			

			services.TryAddSingleton<IDbContextFactory, DbContextFactory<TDbContext>>();
			return this;
		}
	}
}
