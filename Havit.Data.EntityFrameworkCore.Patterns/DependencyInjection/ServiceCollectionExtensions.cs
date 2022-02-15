using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection
{
	/// <summary>
	/// Extension metody pro IServiceCollection. Pro získání installeru Havit.Data.Entity.Patterns a souvisejících služeb.
	/// </summary>
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// Vrátí installer pro Havit.Data.Entity.Patterns a souvisejících služeb.
		/// </summary>
		/// <param name="services">ServiceCollection.</param>
		/// <param name="componentRegistrationAction">Konfigurace registrace komponent.</param>
		public static ServiceCollectionEntityPatternsInstaller WithEntityPatternsInstaller(this IServiceCollection services, Action<ServiceCollectionComponentRegistrationOptions> componentRegistrationAction = null)
		{
			ServiceCollectionComponentRegistrationOptions componentRegistrationOptions = new ServiceCollectionComponentRegistrationOptions();
			componentRegistrationAction?.Invoke(componentRegistrationOptions);
			return new ServiceCollectionEntityPatternsInstaller(services, componentRegistrationOptions);
		}
	}
}
