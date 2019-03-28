using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal
{
    /// <inheritdoc />
    public class CollectionTargetTypeStore : ICollectionTargetTypeStore
    {
        private readonly Lazy<Dictionary<TypePropertyName, Type>> collectionTargetTypes;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        public CollectionTargetTypeStore(IDbContextFactory dbContextFactory)
        {
            collectionTargetTypes = GetLazyCollectionTargetTypesDictionary(dbContextFactory);
        }

        /// <inheritdoc />
        public Type GetCollectionTargetType(Type type, string propertyName)
        {
            if (collectionTargetTypes.Value.TryGetValue(new TypePropertyName(type, propertyName), out Type result))
            {
                return result;
            }
            else
            {
                throw new InvalidOperationException(String.Format("Target type of entity type {0} and property {1} not found.", type.FullName, propertyName));
            }
        }

        private Lazy<Dictionary<TypePropertyName, Type>> GetLazyCollectionTargetTypesDictionary(IDbContextFactory dbContextFactory)
        {
            return new Lazy<Dictionary<TypePropertyName, Type>>(() =>
            {
                Dictionary<TypePropertyName, Type> result = null;
                dbContextFactory.ExecuteAction(dbContext =>
                {
                    result = dbContext.Model.GetApplicationEntityTypes()
                    .SelectMany(entityType => entityType.GetNavigations())
                    .Where(navigation => navigation.IsCollection())
                    .ToDictionary(
                        navigation => new TypePropertyName(navigation.DeclaringEntityType.ClrType, navigation.Name),
                        navigation => navigation.GetTargetType().ClrType);
                });
                return result;
            }, LazyThreadSafetyMode.PublicationOnly);
        }


    }
}
