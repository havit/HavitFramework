using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Entity.Mapping.Internal
{
	[DebuggerDisplay("{FullName,nq}")]
	public class RegisteredEntity
	{
		public string NamespaceName { get; set; }
		public string ClassName { get; set; }
		public string FullName { get; set; }
		public Type Type { get; set; }
		public bool HasEntryEnum { get; set; }

		// Premisteno na uroven RegisteredProperty.StoreGeneratedPattern
		public bool HasDatabaseGeneratedIdentity { get; set; } // TODO: TW: RegisteredProperty.StoreGeneratedPattern zatím nefunguje, vracíme se tedy k HasDatabaseGeneratedIdentity, pak je třeba dořešit
		public List<RegisteredProperty> Properties { get; set; }
		public List<RegisteredProperty> PrimaryKeys { get; set; }


	}
}
