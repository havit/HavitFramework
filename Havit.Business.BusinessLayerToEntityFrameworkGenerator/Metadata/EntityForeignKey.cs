using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Metadata
{
	public class EntityForeignKey
	{
		public string ForeignKeyPropertyName { get; set; }

		public string NavigationPropertyName { get; set; }

		public Column Column { get; set; }
	}
}