using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Registration.Lifestyle;
using Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.EntityFrameworkCore.Patterns.Windsor.Installers
{
	/// <summary>
	/// Nastavení registrace komponent installeru Havit.Data.Entity.Patterns a souvisejících služeb.
	/// </summary>
	public sealed class WindsorContainerComponentRegistrationOptions : ComponentRegistrationOptionsBase<Func<LifestyleGroup<object>, ComponentRegistration<object>>>
	{
		/// <summary>
		/// Konstruktor.
		/// </summary>
		internal WindsorContainerComponentRegistrationOptions()
		{

		}
	}
}
