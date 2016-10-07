using System;
using System.Diagnostics;

namespace Havit.Data.Entity.CodeGenerator.Entity
{
	[DebuggerDisplay("{PropertyName,nq}: {Nullable ? \"Nullable\" : \"Required\",nq}, {MaxLength != null ? \"MaxLength \" + MaxLength.ToString() : System.String.Empty, nq}")]
	public class RegisteredProperty
	{
		public string PropertyName { get; set; }
		public Type Type { get; set; }
		public bool Nullable { get; set; }
		public int? MaxLength { get; set; }
	}
}