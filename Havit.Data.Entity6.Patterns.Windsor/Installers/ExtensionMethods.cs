﻿using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Registration.Lifestyle;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.Entity.Patterns.Windsor.Installers;

internal static class ExtensionMethods
{
	internal static BasedOnDescriptor WithServiceConstructedInterface(this BasedOnDescriptor basedOnDescriptor, Type genericInterfaceType)
	{
		Contract.Requires<ArgumentNullException>(basedOnDescriptor != null, nameof(basedOnDescriptor));
		Contract.Requires<ArgumentNullException>(genericInterfaceType != null, nameof(genericInterfaceType));
		Contract.Requires<ArgumentException>(genericInterfaceType.IsInterface, nameof(genericInterfaceType));

		return basedOnDescriptor.WithService.Select((type, baseTypes) => GetClosedConstructedType(type, genericInterfaceType));
	}

	internal static ComponentRegistration<object> ApplyLifestyle(this ComponentRegistration<object> componentRegistration, Func<LifestyleGroup<object>, ComponentRegistration<object>> lifestyleConfiguration)
	{
		return lifestyleConfiguration(componentRegistration.LifeStyle);
	}

	internal static BasedOnDescriptor ApplyLifestyle(this BasedOnDescriptor basedOnDescriptor, Func<LifestyleGroup<object>, ComponentRegistration<object>> lifestyleConfiguration)
	{
		return basedOnDescriptor.Configure(componentRegistration => lifestyleConfiguration(componentRegistration.LifeStyle));
	}

	private static IEnumerable<Type> GetClosedConstructedType(Type registeredType, Type openConstructedInterfaceType)
	{
		Contract.Requires<ArgumentNullException>(registeredType != null, nameof(registeredType));

		// Terms: https://msdn.microsoft.com/en-us/library/system.type.isgenerictype.aspx

		Type[] interfaces = registeredType.GetInterfaces();
		Type closedInterfaceType = interfaces.Where(itype => itype.IsGenericType && itype.GetGenericTypeDefinition() == openConstructedInterfaceType).FirstOrDefault();
		if (closedInterfaceType != null)
		{
			yield return openConstructedInterfaceType.MakeGenericType(closedInterfaceType.GenericTypeArguments);
		}
	}
}
