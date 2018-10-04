using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Entity.Mapping.Internal
{
	/// <summary>
	/// Registered entity from ObjectContext
	/// </summary>
	[DebuggerDisplay("{FullName,nq}")]
	public class RegisteredEntity
	{
		/// <summary>
		/// Namespace in which registered entity is.
		/// </summary>
		public string NamespaceName { get; set; }

		/// <summary>
		/// Name of class in which registered entity is.
		/// </summary>
		public string ClassName { get; set; }

		/// <summary>
		/// Full name of registered entity.
		/// </summary>
		public string FullName { get; set; }
		
		/// <summary>
		/// Type of registered entity
		/// </summary>
		public Type Type { get; set; }
		
		/// <summary>
		/// Identifier if registered entity is enum value.
		/// </summary>
		public bool HasEntryEnum { get; set; }

		// Premisteno na uroven RegisteredProperty.StoreGeneratedPattern

		/// <summary>
		/// Identifier if registered entity has primary key.
		/// </summary>
		public bool HasDatabaseGeneratedIdentity { get; set; } // TODO: TW: RegisteredProperty.StoreGeneratedPattern zatím nefunguje, vracíme se tedy k HasDatabaseGeneratedIdentity, pak je třeba dořešit
		
		/// <summary>
		/// Properties of registered entity.
		/// </summary>
		public List<RegisteredProperty> Properties { get; set; }

		/// <summary>
		/// Primary keys of registered entity.
		/// </summary>
		public List<RegisteredProperty> PrimaryKeys { get; set; }
	}
}
