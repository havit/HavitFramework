using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Havit.Data.Entity.Mapping
{
	[DebuggerDisplay("{Type.FullName,nq}")]
	public class MappedEntity
	{
		public Type Type { get; set; }

		public List<MappedProperty> DeclaredProperties { get; set; }		
	}
}