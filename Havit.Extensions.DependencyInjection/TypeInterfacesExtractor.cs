using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Extensions.DependencyInjection
{
	public static class TypeInterfacesExtractor
	{
		/// <summary>
		/// Gets interfaces types to register the service to DI container.
		/// Gets all implemented interfaces "matching the service name".
		/// Matching names, means that the implementing class contains in its name the name of the interface (without the I on the front).
		/// ("matching names" source: https://github.com/castleproject/Windsor/blob/master/docs/registering-components-by-conventions.md)
		/// </summary>
		public static Type[] GetInterfacesToRegister(Type implementationType)
		{
			return implementationType
				.GetInterfaces()
				.Where(interfaceType => implementationType.Name.Contains(GetInterfaceName(interfaceType)))
				.ToArray();
		}


		/// <summary>
		/// Returns the name of the interface without leading I.
		/// The next letter must be uppercase otherwise the leading I is not trimmed.		
		/// method source: https://github.com/castleproject/Windsor/blob/35ebd6e5a77de289536b3862f366d6cbfad19ecc/src/Castle.Windsor/MicroKernel/Registration/ServiceDescriptor.cs
		/// </summary>
		private static string GetInterfaceName(Type @interface)
		{
			var name = @interface.Name;
			if ((name.Length > 1 && name[0] == 'I') && char.IsUpper(name[1]))
			{
				return name.Substring(1);
			}
			return name;
		}
	}
}
