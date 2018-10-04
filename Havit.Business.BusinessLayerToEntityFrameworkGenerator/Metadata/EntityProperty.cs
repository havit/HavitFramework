using System.Diagnostics;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Metadata
{
	[DebuggerDisplay("{TypeName} {Name} (Column: {Column.Name})")]
	public class EntityProperty
	{
		public string Name { get; set; }

		public Column Column { get; set; }

		public string TypeName { get; set; }
	}
}