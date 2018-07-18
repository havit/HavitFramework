using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Metadata
{
	public class GeneratedModelClass
	{
		public string Name { get; set; }

		public Table Table { get; set; }

		public List<EntityPrimaryKeyPart> PrimaryKeyParts { get; } = new List<EntityPrimaryKeyPart>();

		public List<EntityProperty> Properties { get; } = new List<EntityProperty>();

		public List<EntityCollectionProperty> CollectionProperties { get; } = new List<EntityCollectionProperty>();

		public List<EntityForeignKey> ForeignKeys { get; } = new List<EntityForeignKey>();

		public EntityPrimaryKeyPart GetPrimaryKeyPartFor(Column column)
		{
			return PrimaryKeyParts.FirstOrDefault(p => p.Property.Column == column);
		}

		public EntityProperty GetPropertyFor(Column column)
		{
			return Properties.FirstOrDefault(p => p.Column == column);
		}

		public IEnumerable<EntityProperty> GetColumnProperties()
		{
			return Properties.Where(prop => ForeignKeys.All(fk => fk.NavigationPropertyName != prop.Name) && CollectionProperties.All(collectionProperty => collectionProperty.Name != prop.Name));
		}
	}
}