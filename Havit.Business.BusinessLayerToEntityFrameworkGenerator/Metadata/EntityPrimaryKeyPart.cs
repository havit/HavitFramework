using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Metadata
{
	public class EntityPrimaryKeyPart
	{
		public string PropertyName { get; set; }

		public Column Column { get; set; }
	}
}