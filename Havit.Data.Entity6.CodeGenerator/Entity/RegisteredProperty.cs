using System;

namespace Havit.Data.Entity.CodeGenerator.Entity
{
	public class RegisteredProperty
	{
		public string PropertyName { get; set; }
		public Type Type { get; set; }
		public bool Nullable { get; set; }
		public int? MaxLength { get; set; }
	}
}