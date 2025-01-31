namespace Havit.Data.Entity.Mapping;

public interface IEntityMappingReader
{
	List<MappedEntity> GetMappedEntities(DbContext dbContext);
}
