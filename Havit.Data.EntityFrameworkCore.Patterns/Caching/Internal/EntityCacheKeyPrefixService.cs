using System.Runtime.CompilerServices;
using Havit.Data.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <summary>
/// Služba pro poskytnutí prefixů stringových klíčů do cache.
/// Do klíče generuje názvy typu.
/// Pro distribuovanou invalidaci musí být klíče deterministické.
/// </summary>
public class EntityCacheKeyPrefixService : IEntityCacheKeyPrefixService
{
	private readonly IEntityCacheKeyPrefixStorage _entityCacheKeyPrefixStorage;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public EntityCacheKeyPrefixService(IEntityCacheKeyPrefixStorage entityCacheKeyPrefixStorage)
	{
		_entityCacheKeyPrefixStorage = entityCacheKeyPrefixStorage;
	}

	/// <inheritdoc />
	public string GetCacheKeyPrefix(Type type)
	{
		if (_entityCacheKeyPrefixStorage.Value.TryGetValue(type, out string result))
		{
			return result;
		}
		else
		{
			throw new InvalidOperationException(string.Format("Type {0} is not a supported type.", type.FullName));
		}
	}
}
