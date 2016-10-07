using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Havit.Data.Entity.CodeGenerator.Entity
{
	[DebuggerDisplay("{FullName,nq}")]
	public class RegisteredEntity
	{
		public string NamespaceName { get; set; }
		public string ClassName { get; set; }
		public string FullName { get; set; }
		public Type Type { get; set; }
		public bool HasEntryEnum { get; set; }
		public bool HasDatabaseGeneratedIdentity { get; set; }
		public List<RegisteredProperty> Properties { get; set; }
	}
}
