using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal
{
    /// <summary>
    /// Poskytuje seznam kolekcí referencující danou entitu.
    /// </summary>
	/// <remarks>
	/// Revize použití s ohledem na https://github.com/volosoft/castle-windsor-ms-adapter/issues/32:
	/// DbContext je registrován scoped, proto se této factory popsaná issue týká.
	/// Z DbContextu jen čteme metadata (ta jsou pro každý DbContext stejná), issue tedy nemá žádný dopad.
	/// </remarks>	
    public class ReferencingCollectionsStore : IReferencingCollectionsStore
    {
        private Lazy<Dictionary<Type, List<ReferencingCollection>>> referencingCollections;
        
        /// <summary>
        /// Konstruktor.
        /// </summary>
        public ReferencingCollectionsStore(IDbContextFactory dbContextFactory)
        {
            referencingCollections = GetLazyDictionary(dbContextFactory);
        }

        /// <inheritdoc />
        public List<ReferencingCollection> GetReferencingCollections(Type type)
        {
            if (referencingCollections.Value.TryGetValue(type, out var result))
            {
                return result;
            }
            else
            {
                throw new InvalidOperationException(String.Format("Type {0} is not a supported type.", type.FullName));
            }
        }

        /// <summary>
        /// Vrací Dictionary s referujícími kolekcemi pro všechny registrované EntityTypes.
        /// </summary>
        public Lazy<Dictionary<Type, List<ReferencingCollection>>> GetLazyDictionary(IDbContextFactory dbContextFactory)
        {
            return new Lazy<Dictionary<Type, List<ReferencingCollection>>>(() =>
            {
                Dictionary<Type, List<ReferencingCollection>> result = null;
                dbContextFactory.ExecuteAction(dbContext =>
                {
                    result = dbContext.Model
                    .GetEntityTypes() // získáme entity types
                                      // z každého EntityType vezmeme ClrType a připojíme ReferencingCollections (klidně prázdný seznam)
                    .Select(entityType => new
                    {
                        ClrType = entityType.ClrType,
                        // abychom vzali referencující kolekce, vezmeme cizí klíče a z nich "opačný směr", tj. kolekci (neřešíme vazbu 1:1)
                        ReferencingCollections = entityType.GetForeignKeys().Where(item => item.PrincipalToDependent != null).Select(foreignKey =>
                    {
                        var property = foreignKey.Properties.Single();
                        return new ReferencingCollection
                        {
                            EntityType = foreignKey.PrincipalEntityType.ClrType,
                            GetForeignKeyValue = (dbContext2, entity) => dbContext2.GetEntry(entity, suppressDetectChanges: true).CurrentValues[property],
                            CollectionPropertyName = foreignKey.PrincipalToDependent.Name
                        };
                    })
                        .ToList()
                    })
                    .ToDictionary(item => item.ClrType, item => item.ReferencingCollections);
                });

                return result;
            });
        }
    }
}
