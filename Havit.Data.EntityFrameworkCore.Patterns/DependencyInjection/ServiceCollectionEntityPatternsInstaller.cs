using Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection
{
	/// <summary>
	/// Implementace <see cref="IEntityPatternsInstaller"/>u pro IServiceCollection.
	/// </summary>
	internal class ServiceCollectionEntityPatternsInstaller : EntityPatternsInstallerBase<ServiceLifetime>
	{
		/// <summary>
		/// Konstruktor.
		/// </summary>
		public ServiceCollectionEntityPatternsInstaller(IServiceCollection serviceCollection, ServiceCollectionComponentRegistrationOptions componentRegistrationOptions) : base(new ServiceCollectionServiceInstaller(serviceCollection), componentRegistrationOptions)
		{
			// NOOP
		}
	}
}
