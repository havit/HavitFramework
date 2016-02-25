using System;
using System.Collections.Generic;

namespace Havit.Data.Entity.CodeGenerator.Entity
{
	public class RegisteredEntity
	{
		public string NamespaceName { get; set; }
		public string ClassName { get; set; }
		public string FullName { get; set; }
		public Type Type { get; set; }
		public List<RegisteredProperty> Properties { get; set; }
	}
}
