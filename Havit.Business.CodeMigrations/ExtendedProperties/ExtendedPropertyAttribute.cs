using System;

namespace Havit.Business.CodeMigrations.ExtendedProperties
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
	public abstract class ExtendedPropertyAttribute : Attribute
	{
		public abstract string Name { get; }
		public abstract string Value { get; }
	}
}
