using Havit.Data.Patterns.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Havit.Data.Patterns.DataLoaders.Fakes
{
    /// <summary>
    /// Explicity data loader, který nic nedělá.
    /// Určeno pro použití v unit testech pro mock IDataLoaderu.
    /// </summary>
    [Fake]    
    public class FakeDataLoader : IDataLoader, IDataLoaderAsync
    {
        /// <summary>
        /// Contract: Načte vlastnosti objektů, pokud ještě nejsou načteny.        
        /// Implementace: Nic nedělá.
        /// </summary>
        public void Load<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] propertyPaths)
            where TEntity : class
        {
            // NOOP
        }

        /// <summary>
        /// Contract: Načte vlastnosti objektů, pokud ještě nejsou načteny.        
        /// Implementace: Nic nedělá.
        /// </summary>
        public void LoadAll<TEntity>(IEnumerable<TEntity> entities, params Expression<Func<TEntity, object>>[] propertyPaths)
            where TEntity : class
        {
            // NOOP
        }

        /// <summary>
        /// Contract: Načte vlastnosti objektů, pokud ještě nejsou načteny.        
        /// Implementace: Nic nedělá.
        /// </summary>
        public Task LoadAllAsync<TEntity>(IEnumerable<TEntity> entities, params Expression<Func<TEntity, object>>[] propertyPaths)
            where TEntity : class
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Contract: Načte vlastnosti objektů, pokud ještě nejsou načteny.        
        /// Implementace: Nic nedělá.
        /// </summary>
        public Task LoadAsync<TEntity>(TEntity entity, params Expression<Func<TEntity, object>>[] propertyPaths)
            where TEntity : class
        {
            return Task.CompletedTask;            
        }
    }
}
