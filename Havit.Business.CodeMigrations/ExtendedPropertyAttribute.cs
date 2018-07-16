using System;

namespace Havit.Business.CodeMigrations
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
	public abstract class ExtendedPropertyAttribute : Attribute
	{ }
}
