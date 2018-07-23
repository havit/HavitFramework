using System;
using System.Collections.Generic;

namespace Havit.Business.CodeMigrations.ExtendedProperties
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
	public abstract class ExtendedPropertiesAttribute : Attribute
	{
		public abstract IDictionary<string, string> ExtendedProperties { get; }
	}
}
