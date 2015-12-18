using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.Registration;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.Entity.Patterns.Windsor
{
	internal static class ExtensionMethods
	{
		internal static BasedOnDescriptor WithServiceConstructedInterface(this BasedOnDescriptor basedOnDescriptor, Type genericInterfaceType)
		{
			Contract.Requires(basedOnDescriptor != null);
			Contract.Requires(genericInterfaceType != null);
			Contract.Requires(genericInterfaceType.IsInterface);

			return basedOnDescriptor.WithService.Select((type, baseTypes) => GetClosedConstructedType(type, genericInterfaceType));
		}

		private static IEnumerable<Type> GetClosedConstructedType(Type registeredType, Type openConstructedInterfaceType)
		{
			Contract.Requires(registeredType != null);

			// Terms: https://msdn.microsoft.com/en-us/library/system.type.isgenerictype.aspx

			Type[] interfaces = registeredType.GetInterfaces();
			Type closedInterfaceType = interfaces.Where(itype => itype.IsGenericType && itype.GetGenericTypeDefinition() == openConstructedInterfaceType).FirstOrDefault();
			if (closedInterfaceType != null)
			{
				yield return openConstructedInterfaceType.MakeGenericType(closedInterfaceType.GenericTypeArguments);
			}
		}
	}
}
