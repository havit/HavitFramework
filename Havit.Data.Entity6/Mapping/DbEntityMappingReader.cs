using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Reflection;

namespace Havit.Data.Entity.Mapping;

public class DbEntityMappingReader : IEntityMappingReader
{
	public List<MappedEntity> GetMappedEntities(DbContext dbContext)
	{
		var objectContext = ((IObjectContextAdapter)dbContext).ObjectContext;
		lock (objectContext)
		{
			List<EntityType> entityTypes = objectContext.MetadataWorkspace.GetItems<EntityType>(DataSpace.OSpace).ToList();
			return (from entityType in entityTypes
					let type = (Type)entityType.GetType().GetProperty("ClrType", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(entityType)
					orderby entityType.FullName
					select new MappedEntity
					{
						Type = type,
						DeclaredProperties = GetMappedProperties(objectContext, entityType, type)
					}).ToList();
		}
	}

	private List<MappedProperty> GetMappedProperties(ObjectContext objectContext, EntityType entityType, Type type)
	{
		return (from property in entityType.DeclaredProperties
				select new MappedProperty
				{
					PropertyName = property.Name,
					Type = property.IsPrimitiveType
						? ((PrimitiveType)property.TypeUsage.EdmType).ClrEquivalentType
						: null,
					Property = type.GetProperty(property.Name),
					IsInPrimaryKey = entityType.KeyProperties.Contains(property),
					IsNullable = property.Nullable,
					IsStoreGenerated = IsStoreGenerated(objectContext, entityType, property)
				}).ToList();
	}

	private bool IsStoreGenerated(ObjectContext objectContext, EntityType entityType, EdmProperty property)
	{
		EntitySet entitySet = objectContext.MetadataWorkspace
			.GetItems<EntityContainer>(DataSpace.SSpace)
			.First()
			.BaseEntitySets
			.OfType<EntitySet>()
			.Single(item => item.Name == entityType.Name);

		EdmProperty edmProperty = entitySet.ElementType.Properties.Single(item => item.Name == property.Name);
		return edmProperty.IsStoreGeneratedIdentity || edmProperty.IsStoreGeneratedComputed;
	}
}