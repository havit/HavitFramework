using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Metadata
{
	public class EntityForeignKey
	{
		public EntityProperty ForeignKeyProperty { get; set; }

		public EntityProperty NavigationProperty { get; set; }

		public Column Column { get; set; }
	}
}