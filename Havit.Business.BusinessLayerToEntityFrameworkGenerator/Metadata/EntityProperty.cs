using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Metadata
{
	public class EntityProperty
	{
		public string Name { get; set; }

		public Column Column { get; set; }
	}
}