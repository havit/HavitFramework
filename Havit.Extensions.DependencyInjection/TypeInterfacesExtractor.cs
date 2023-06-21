using Havit.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Extensions.DependencyInjection;

/// <summary>
/// Helpers for interfaces types to register the service to DI container.
/// </summary>
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
		if (implementationType.IsConstructedGenericType)
		{
			throw new InvalidOperationException("Close generic types are not supported.");
		}

		// MyService can be registered as IService, IMyService
		// MyService<T> can be registered as IService<T>, IMyService<T>
		// MyService<T1, T2> can be registered as IService<T1, T2>, IMyService<T1, T2>
		return implementationType
			.GetInterfaces()
			// MyService cannot be registered as IService<T>
			.WhereIf(!implementationType.IsGenericTypeDefinition, interfaceType => !interfaceType.IsGenericTypeDefinition)
			// MyService<T> cannot be registered as IService
			// MyService<T1> cannot be registered as IService<T1, T2>
			// MyService<T1> cannot be registered as IService<T, string>
			.WhereIf(implementationType.IsGenericTypeDefinition, interfaceType => interfaceType.IsConstructedGenericType
				&& Enumerable.SequenceEqual(
					implementationType.GetGenericArguments() /* closed generic type or the type parameters of a generic type definition */, 
					interfaceType.GenericTypeArguments /* generic type (generic class) arguments, arguments are of generic type (property IsGenericParameter is true */))
			.Where(interfaceType => implementationType.Name.Contains(GetInterfaceName(interfaceType)))
			// Generic types from Type.GetInterfaces have false IsGenericTypeDefinition!!!
			// Generic types from Type.GetInterfaces IsConstructedGenericType with generic type parameters!
			.Select(interfaceType => interfaceType.IsConstructedGenericType ? interfaceType.GetGenericTypeDefinition() : interfaceType)
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
