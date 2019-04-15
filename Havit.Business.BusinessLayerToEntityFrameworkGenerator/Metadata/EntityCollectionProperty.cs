using Havit.Business.BusinessLayerGenerator.Helpers.Types;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Metadata
{
	public class EntityCollectionProperty
	{
		public string Name { get; set; }

		public CollectionProperty CollectionProperty { get; set; }

		public Table TargetTable => CollectionProperty.IsManyToMany ? CollectionProperty.JoinTable : CollectionProperty.TargetTable;

        public GeneratedModelClass TargetClass { get; set; }

        public string TargetClassFullName => $"{TargetClass.Namespace}.{TargetClass.Name}";
    }
}