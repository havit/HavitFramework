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
    public class ReferencingCollectionsService : IReferencingCollectionsService
    {
        private readonly IReferencingCollectionsStorage referencingCollectionsStorage;
        private readonly IDbContext dbContext;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        public ReferencingCollectionsService(IReferencingCollectionsStorage referencingCollectionsStorage, IDbContext dbContext)
        {
            this.referencingCollectionsStorage = referencingCollectionsStorage;
            this.dbContext = dbContext;
        }

        /// <inheritdoc />
        public List<ReferencingCollection> GetReferencingCollections(Type type)
        {
            if (referencingCollectionsStorage.Value == null)
            {
                lock (referencingCollectionsStorage)
                {
                    if (referencingCollectionsStorage.Value == null)
                    {
                        referencingCollectionsStorage.Value = dbContext.Model
                            .GetEntityTypes() // získáme entity types
                            // z každého EntityType vezmeme ClrType a připojíme ReferencingCollections (klidně prázdný seznam)
                            .Select(entityType => new
                            {
                                ClrType = entityType.ClrType,
                                // abychom vzali referencující kolekce, vezmeme cizí klíče a z nich "opačný směr", tj. kolekci (neřešíme vazbu 1:1)
                                ReferencingCollections = entityType.GetForeignKeys()
                                    .Where(item => (item.PrincipalToDependent != null)
                                        && (item.PrincipalToDependent is Microsoft.EntityFrameworkCore.Metadata.Internal.Navigation)
                                        && (((Microsoft.EntityFrameworkCore.Metadata.Internal.Navigation)item.PrincipalToDependent).CollectionAccessor != null))
                                    .Select(foreignKey =>
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
                    }
                }
            }

            if (referencingCollectionsStorage.Value.TryGetValue(type, out var result))
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
