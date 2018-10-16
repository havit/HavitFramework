using System;
using System.Collections.Generic;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Services.Caching;

namespace Havit.Data.EntityFrameworkCore.Patterns.Windsor.Installers.Caching
{
	/// <summary>
	/// Installer, která zaregistruje službu, která nic necachuje (NoCachingEntityCacheManager). 
	/// </summary>
	public sealed class NoCachingInstaller : IWindsorInstaller
	{
		/// <inheritdoc />
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(Component.For<IEntityCacheManager>().ImplementedBy<NoCachingEntityCacheManager>().LifestyleSingleton());
		}
	}
}
