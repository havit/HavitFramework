using Havit.Extensions.DependencyInjection.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Havit.Extensions.DependencyInjection
{
	/// <summary>
	/// Service to register with lifetime and service type.
	/// </summary>
	[DebuggerDisplay("Type={Type}, ServiceAttribute = {ServiceAttribute}")]
	public class TypeServiceAttributeInfo
	{
		/// <summary>
		/// Service to register.
		/// </summary>
		public Type Type { get; set; }


		/// <summary>
		/// ServiceAttribute with service lifetime and service types.
		/// </summary>
		public ServiceAttribute ServiceAttribute { get; set; }
		
	}
}
