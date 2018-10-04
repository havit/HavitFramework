using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Metadata
{
	[DebuggerDisplay("{Name}, Properties: {Properties.Count}")]
	public class GeneratedModelClass
	{
		public string Name { get; set; }

	    public string Namespace { get; set; }

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
			return Properties.Where(prop => ForeignKeys.All(fk => fk.NavigationProperty.Name != prop.Name) && CollectionProperties.All(collectionProperty => collectionProperty.Name != prop.Name));
		}

		public EntityForeignKey GetForeignKeyForColumn(Column column)
		{
			return ForeignKeys.FirstOrDefault(fk => fk.Column == column);
		}
	}
}