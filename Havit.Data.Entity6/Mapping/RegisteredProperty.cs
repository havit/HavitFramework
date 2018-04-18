using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Entity.Mapping.Internal
{
	/// <summary>
	/// It is property of RegisteredEntity.
	/// </summary>
	[DebuggerDisplay("{PropertyName,nq}: {Nullable ? \"Nullable\" : \"Required\",nq}, {MaxLength != null ? \"MaxLength \" + MaxLength.ToString() : System.String.Empty, nq}")]
	public class RegisteredProperty
	{
		/// <summary>
		/// Name of property.
		/// </summary>
		public string PropertyName { get; set; }

		/// <summary>
		/// Type of property
		/// </summary>
		public Type Type { get; set; }
		
		/// <summary>
		/// Identifier that the property is nullable.
		/// </summary>
		public bool? Nullable { get; set; }

		/// <summary>
		/// Max length of property.
		/// </summary>
		public int? MaxLength { get; set; }

		/// <summary>
		/// StoreGeneratedPattern of property.
		/// </summary>
		public StoreGeneratedPattern? StoreGeneratedPattern { get; set; }

		/// <summary>
		/// Identifier that the property is generated identity.
		/// </summary>
		public bool IsStoreGeneratedIdentity { get; set; }

		/// <summary>
		/// Identifier that the property is key property.
		/// </summary>
		public bool IsKeyProperty { get; set; }

		/// <summary>
		/// Foreign key of registered property which is NavigationProperty
		/// </summary>
		//public RegisteredProperty ForeignKey { get; set; }
		public object ForeignKey { get; set; } // int?, string,....

		/// <summary>
		/// Foreign keys of registered property which is NavigationProperty
		/// </summary>
		public List<object> ForeignKeys { get; } = new List<object>();
		// Je obsazeno v StoreGeneratedPattern
		//public bool IsIdentity { get; set; }

		/// <summary>
		/// NavigationProperty - element provides a reference to the other end of association.
		/// </summary>
		public bool IsNavigationProperty { get; set; }
		//public RegisteredProperty NavigationProperty { get; internal set; }
	}
}
