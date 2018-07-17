using Havit.Business.BusinessLayerGenerator.Helpers.Types;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Metadata
{
	public class EntityCollectionProperty
	{
		public string Name { get; set; }

		public CollectionProperty CollectionProperty { get; set; }
	}
}