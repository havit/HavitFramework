using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit
{
	/// <summary>
	/// Extension metody k Type.
	/// </summary>
	public static class TypeExtensions
	{
		/// <summary>
		/// Vrací true, pokud daný typ implementuje daný interface.
		/// </summary>
		public static bool ImplementsInterface(this Type type, Type interfaceType)
		{
			return type.GetInterfaces().Any(typeInterfaceType => typeInterfaceType == interfaceType);
		}
	}
}
