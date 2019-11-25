using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Havit.Extensions.DependencyInjection
{
	/// <summary>
	/// Service type to register with lifetime.
	/// </summary>
	[DebuggerDisplay("Type={Type}, Attribute = {Attribute}")]
	public class TypeAttributeInfo
	{
		/// <summary>
		/// Service type to register.
		/// </summary>
		public Type Type { get; set; }

		/// <summary>
		/// Liftime of the service.
		/// </summary>
		public ServiceLifetime Lifetime { get; set; }
	}
}
