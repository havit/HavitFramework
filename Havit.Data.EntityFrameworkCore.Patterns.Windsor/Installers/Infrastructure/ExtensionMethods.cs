using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Registration.Lifestyle;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.EntityFrameworkCore.Patterns.Windsor.Installers.Infrastructure
{
	internal static class ExtensionMethods
	{
		internal static ComponentRegistration<object> ApplyLifestyle(this ComponentRegistration<object> componentRegistration, Func<LifestyleGroup<object>, ComponentRegistration<object>> lifestyleConfiguration)
		{
			return lifestyleConfiguration(componentRegistration.LifeStyle);
		}
	}
}
