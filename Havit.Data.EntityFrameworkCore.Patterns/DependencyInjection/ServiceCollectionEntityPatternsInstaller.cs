using Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Infrastructure;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection
{
	/// <summary>
	/// Implementace <see cref="IEntityPatternsInstaller"/>u pro IServiceCollection.
	/// </summary>
	public class ServiceCollectionEntityPatternsInstaller : EntityPatternsInstallerBase<ServiceLifetime, ServiceCollectionEntityPatternsInstaller>
	{
        private readonly IServiceCollection services;
        private readonly ServiceCollectionComponentRegistrationOptions componentRegistrationOptions;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public ServiceCollectionEntityPatternsInstaller(IServiceCollection services, ServiceCollectionComponentRegistrationOptions componentRegistrationOptions) : base(new ServiceCollectionServiceInstaller(services), componentRegistrationOptions)
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
			services.AddDbContext<IDbContext, TDbContext>(optionsAction, componentRegistrationOptions.DbContextLifestyle, componentRegistrationOptions.DbContextLifestyle);
			services.AddTransient(typeof(IDbContextTransient), typeof(TDbContext));
			return this;
		}

		/// <summary>
		/// Registruje do DI containeru DbContext s DbContext poolingem. Dále registruje IDbContextTransient.
		/// </summary>
		public ServiceCollectionEntityPatternsInstaller AddDbContextPool<TDbContext>(Action<DbContextOptionsBuilder> optionsAction, int poolSize = DbContextPool<DbContext>.DefaultPoolSize)
			where TDbContext : Havit.Data.EntityFrameworkCore.DbContext, IDbContext
		{
			Contract.Requires(componentRegistrationOptions.DbContextLifestyle == ServiceLifetime.Scoped);

			services.AddDbContextPool<IDbContext, TDbContext>(optionsAction, poolSize);
			services.AddTransient(typeof(IDbContextTransient), typeof(TDbContext));
			return this;
		}
	}
}
