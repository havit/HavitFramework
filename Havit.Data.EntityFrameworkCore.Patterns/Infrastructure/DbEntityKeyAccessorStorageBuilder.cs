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
		this._dbContext = dbContext;
	}

	/// <inheritdoc />
	public IDbEntityKeyAccessorStorage Build()
	{
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
