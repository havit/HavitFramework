using Havit.Data.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;

/// <inheritdoc />
public class DbEntityKeyAccessorStorageBuilder : IDbEntityKeyAccessorStorageBuilder
{
	private readonly IDbContext _dbContext;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public DbEntityKeyAccessorStorageBuilder(IDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	/// <inheritdoc />
	public IDbEntityKeyAccessorStorage Build()
	{
		// GetApplicationEntityTypes už vylučuje keyless entity, takže FindPrimaryKey() je vždy k dispozici.
		// Entity se shadow PK (klíč bez CLR PropertyInfo) nepodporujeme - accessor přes reflexi pro ně nelze sestavit. Selžeme srozumitelně při startu, ne až NRE daleko od příčiny.
		var entityTypesWithShadowKey = _dbContext.Model.GetApplicationEntityTypes()
			.Where(entityType => entityType.FindPrimaryKey().Properties.Any(property => property.PropertyInfo == null))
			.Select(entityType => entityType.ClrType.FullName)
			.ToList();
		if (entityTypesWithShadowKey.Count > 0)
		{
			throw new InvalidOperationException($"Shadow primary key (a key without a CLR property) is not supported by {nameof(DbEntityKeyAccessor)}. Affected entity types: {String.Join(", ", entityTypesWithShadowKey)}.");
		}

		return new DbEntityKeyAccessorStorage
		{
			Value = _dbContext.Model.GetApplicationEntityTypes()
				.ToFrozenDictionary(
					entityType => entityType.ClrType,
					entityType =>
					{
						var propertyInfos = entityType.FindPrimaryKey().Properties.Select(property => property.PropertyInfo).ToArray();
						return new DbEntityKeyAccessorItem
						{
							PropertyInfos = propertyInfos,
							PropertyNames = propertyInfos.Select(propertyInfo => propertyInfo.Name).ToArray()
						};
					})
		};
	}
}
