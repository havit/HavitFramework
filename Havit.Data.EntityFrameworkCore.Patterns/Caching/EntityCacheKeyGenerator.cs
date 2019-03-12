using Havit.Data.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching
{
    /// <summary>
    /// Služba pro poskytnutí stringových klíčů do cache.
    /// Do klíče generuje názvy typu.
    /// Pro distribuovanou invalidaci musí být klíče deterministické.
    /// </summary>
    public class EntityCacheKeyGenerator : IEntityCacheKeyGenerator
	{
        private readonly Lazy<Dictionary<Type, string>> typeToCacheKeyCoreMapping;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="dbContextFactory"></param>
        public EntityCacheKeyGenerator(IDbContextFactory dbContextFactory)
        {
            typeToCacheKeyCoreMapping = GetLazyDictionary(dbContextFactory);
        }

		/// <inheritdoc />
		public string GetEntityCacheKey(Type entityType, object key)
		{			
			return GetValueFromDictionary(entityType) + key.ToString();
		}

		/// <inheritdoc />
		public string GetCollectionCacheKey(Type entityType, object key, string propertyName)
		{
			return GetValueFromDictionary(entityType) + key.ToString() + "|" + propertyName;
		}

		/// <inheritdoc />
		public string GetAllKeysCacheKey(Type entityType)
		{
			return GetValueFromDictionary(entityType) + "AllKeys";
		}

        private Lazy<Dictionary<Type, string>> GetLazyDictionary(IDbContextFactory dbContextFactory)
        {
            return new Lazy<Dictionary<Type, string>>(() =>
            {
                Dictionary<Type, string> result = null;
                dbContextFactory.ExecuteAction(dbContext =>
                {
                    var typesByName = dbContext
                        .Model
                        .GetApplicationEntityTypes(includeManyToManyEntities: false)
                        .Select(entityType => entityType.ClrType)
                        .GroupBy(type => type.Name)
                        .ToList();

                    var singleTypeOccurences = typesByName
                        .Where(group => group.Count() == 1) // tam, kde pod jménem máme jen jednu položku (>99%)
                        .Select(group => new { Type = group.Single(), CacheKeyCore = group.Key }); // použijeme jen název třídy (bez namespace)

                    var multipleTypeOccurences = typesByName
                            .Where(group => group.Count() > 1) // tam, kde máme pod jedním názvem více tříd v různých namespaces (<1%)
                            .SelectMany(group => group)
                            .Select(type => new { Type = type, CacheKeyCore = type.FullName }); // použijeme celý název třídy vč. namespace

                    result = singleTypeOccurences.Concat(multipleTypeOccurences)
                            .ToDictionary(item => item.Type, item => "EF|" + item.CacheKeyCore + "|");
                });

                return result;
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GetValueFromDictionary(Type type)
        {
            if (typeToCacheKeyCoreMapping.Value.TryGetValue(type, out string result))
            {
                return result;
            }
            else
            {
                throw new InvalidOperationException(String.Format("Type {0} is not a supported type.", type.FullName));
            }
        }

    }
}
