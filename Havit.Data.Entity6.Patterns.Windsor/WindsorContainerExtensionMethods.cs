using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Registration.Lifestyle;
using Castle.Windsor;
using Havit.Data.Entity.Patterns.QueryServices;
using Havit.Data.Entity.Patterns.Repositories;
using Havit.Data.Entity.Patterns.Windsor.Installers;
using Havit.Data.Patterns.Attributes;
using Havit.Data.Patterns.DataEntries;
using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.Localizations;
using Havit.Data.Patterns.Repositories;

namespace Havit.Data.Entity.Patterns.Windsor;

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
