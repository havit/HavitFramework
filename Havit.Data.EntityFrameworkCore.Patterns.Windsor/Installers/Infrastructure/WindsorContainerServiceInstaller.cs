using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Registration.Lifestyle;
using Castle.Windsor;
using Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.Windsor.Installers.Infrastructure
{
	/// <inheritdoc />
	public class WindsorContainerServiceInstaller : ServiceInstallerBase<Func<LifestyleGroup<object>, ComponentRegistration<object>>>
	{
		private readonly IWindsorContainer container;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public WindsorContainerServiceInstaller(IWindsorContainer container)
		{
			this.container = container;
		}

		/// <inheritdoc />
		protected override Func<LifestyleGroup<object>, ComponentRegistration<object>> SingletonLifetime => lf => lf.Singleton;

		/// <inheritdoc />
		protected override Func<LifestyleGroup<object>, ComponentRegistration<object>> TransientLifetime => lf => lf.Transient;

		/// <inheritdoc />
		public override void AddService(Type serviceType, Type implementationType, Func<LifestyleGroup<object>, ComponentRegistration<object>> lifetime)
		{
			if (serviceType == typeof(IDbContextTransient))
			{
				// DbContext registrujeme pod IDbContext a pod IDbContextTransient.
				// Proto musíme jednu z registrací pojmenovat, jinak dostaneme výjimku o dvojí registraci DbContextu (resp. výjimka říká, že komponenta s daným jménem již existuje a nechť druhou registraci nějak pojmenujeme).
				container.Register(Component.For(serviceType).ImplementedBy(implementationType).ApplyLifestyle(lifetime).Named(implementationType.Name + "_Transient"));
				return;
			}

			container.Register(Component.For(serviceType).ImplementedBy(implementationType).ApplyLifestyle(lifetime));
		}

		/// <inheritdoc />
		public override void AddServiceSingletonInstance(Type serviceType, object implementation)
		{
			container.Register(Component.For(serviceType).Instance(implementation).LifestyleSingleton());
		}

		/// <inheritdoc />
		protected override void AddMultipleServices(Type[] serviceTypes, Type implementationType, Func<LifestyleGroup<object>, ComponentRegistration<object>> lifetime)
		{
			container.Register(Component.For(serviceTypes.First()).Forward(serviceTypes.Skip(1)).ImplementedBy(implementationType).ApplyLifestyle(lifetime));			
		}

		/// <inheritdoc />
		public override void TryAddService(Type serviceType, Type implementationType, Func<LifestyleGroup<object>, ComponentRegistration<object>> lifetime)
		{
			if (!container.Kernel.HasComponent(serviceType))
			{
				AddService(serviceType, implementationType, lifetime);
			}
		}

		/// <inheritdoc />
		public override void TryAddFactory(Type factoryType)
		{
			if (!container.Kernel.HasComponent(factoryType))
			{
				container.Register(Component.For(factoryType).AsFactory());
			}
		}
    }
}
