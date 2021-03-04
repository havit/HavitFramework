using System.Linq;
using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions
{
    /// <summary>
    /// Adds indexes into entities, which are used in Business Layer sorted collections.
    ///
    /// <para>
    /// For example: in Master, Child scenario, Master has collection List of Child with Collection(Sorting = "Order") attribute.
    /// Child has FK property declared (pointing to Master PK) and Order property representing order of Child objects.
    /// This convention will try to create index (MasterId, Count).
    /// </para>
    /// </summary>
    /// <remarks>
    /// Implementation as <see cref="IModelFinalizedConvention"/> is necessary, because there's no convention for changing annotation of <see cref="INavigation"/>.
    /// </remarks>
    public class CollectionOrderIndexConvention : IModelFinalizingConvention
    {
        /// <inheritdoc />
        public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
        {
            foreach (var navigation in modelBuilder.Metadata
                .GetApplicationEntityTypes()
                .SelectMany(e => e.GetNavigations()))
            {
                var conventionNavigation = (IConventionNavigation)navigation;

                CreateIndex(conventionNavigation);
            }
        }

        private static void CreateIndex(IConventionNavigation navigation)
        {
            var orderByProperties = new CollectionAttributeAccessor(navigation).ParseSortingProperties();
            if (orderByProperties.Count == 0)
            {
                return;
            }

            var entityType = navigation.ForeignKey.DeclaringEntityType;

            // move PK column to the front of index properties
            orderByProperties.Remove(navigation.ForeignKey.Properties[0].Name);
            orderByProperties.Insert(0, navigation.ForeignKey.Properties[0].Name);

            var indexProperties = orderByProperties.Select(entityType.FindProperty).ToList();

            if (entityType.GetBusinessLayerDeletedProperty() is IConventionProperty deletedProperty)
            {
                indexProperties.Add(deletedProperty);
            }

            if (entityType.FindIndex(indexProperties) == null)
            {
                entityType.AddIndex(indexProperties);
            }
        }
    }
}