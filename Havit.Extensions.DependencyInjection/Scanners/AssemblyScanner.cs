using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using Havit.Extensions.DependencyInjection.Abstractions;

namespace Havit.Extensions.DependencyInjection.Scanners
{
	/// <summary>
	/// Scans assembly for types to register.
	/// </summary>
	public static class AssemblyScanner
	{
		/// <summary>
		/// Scans assembly for classes with ServiceAttribute and the given profile.
		/// </summary>
		public static TypeServiceAttributeInfo[] GetTypesWithServiceAttribute(Assembly assembly, string profile)
		{
			return (from type in assembly.GetTypes()
					from serviceAttribute in type.GetCustomAttributes(typeof(ServiceAttribute), false).Cast<ServiceAttribute>()
					where (serviceAttribute.Profile == profile)
					select new TypeServiceAttributeInfo { Type = type, ServiceAttribute = serviceAttribute })
					.ToArray();
		}
	}
}
