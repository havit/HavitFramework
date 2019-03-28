using System;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal
{
    /// <summary>
    /// Poskytuje cílový typ kolekce dané entity, přesněji typ, který je v kolekci.
    /// </summary>
    public interface ICollectionTargetTypeStore
    {
        /// <summary>
        /// Poskytuje cílový typ kolekce dané entity, přesněji typ, který je v kolekci. Pro vlastnost typu List&lt;Role&gt; vrací typ Role.
        /// </summary>
        Type GetCollectionTargetType(Type entityType, string propertyName);
    }
}