using Castle.Windsor;
using Havit.Data.EntityFrameworkCore.Patterns.Windsor.Installers;

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
		/// <param name="componentRegistrationOptions">Konfigurace registrace komponent.</param>
		public static IEntityPatternsInstaller WithEntityPatternsInstaller(this IWindsorContainer container, ComponentRegistrationOptions componentRegistrationOptions)
		{
			return new EntityPatternsInstaller(container, componentRegistrationOptions);
		}
	}
}
