﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit;

/// <summary>
/// Extension methods for Type.
/// </summary>
public static class TypeExtensions
{
	/// <summary>
	/// Returns true if the given type implements the specified interface.
	/// </summary>
	public static bool ImplementsInterface(this Type type, Type interfaceType)
	{
		return type.GetInterfaces().Any(typeInterfaceType => typeInterfaceType == interfaceType);
	}
}
