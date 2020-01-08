using Castle.Windsor;
using Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection;
using Havit.Data.EntityFrameworkCore.Patterns.Windsor.Installers;
using System;

namespace Havit.Data.EntityFrameworkCore.Patterns.Windsor
{
	/// <summary>
	/// Extension metody pro IWindsorContainer. Pro získání installeru Havit.Data.Entity.Patterns a souvisejících služeb.
	/// </summary>
	public static class WindsorContainerExtensionMethods
	{
		/// <summary>
		/// Vrátí installer pro Havit.Data.Entity.Patterns a souvisejících služeb.
		/// </summary>
		/// <param name="container">Windsor container.</param>
		/// <param name="componentRegistrationAction">Konfigurace registrace komponent.</param>
		public static IEntityPatternsInstaller WithEntityPatternsInstaller(this IWindsorContainer container, Action<WindsorContainerComponentRegistrationOptions> componentRegistrationAction = null)
		{
			var componentRegistrationOptions = new WindsorContainerComponentRegistrationOptions();
			componentRegistrationAction?.Invoke(componentRegistrationOptions);
			return new WindsorContainerEntityPatternsInstaller(container, componentRegistrationOptions);
		}
	}
}
