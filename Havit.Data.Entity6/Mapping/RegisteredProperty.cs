using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Entity.Mapping.Internal
{
	[DebuggerDisplay("{PropertyName,nq}: {Nullable ? \"Nullable\" : \"Required\",nq}, {MaxLength != null ? \"MaxLength \" + MaxLength.ToString() : System.String.Empty, nq}")]
	public class RegisteredProperty
	{
		public string PropertyName { get; set; }
		public Type Type { get; set; }
		public bool? Nullable { get; set; }
		public int? MaxLength { get; set; }
		public StoreGeneratedPattern? StoreGeneratedPattern { get; set; }
		public bool IsStoreGeneratedIdentity { get; set; }

		public bool IsKeyProperty { get; set; }
		//public RegisteredProperty ForeignKey { get; set; }
		public object ForeignKey { get; set; } // int?, string,....

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
