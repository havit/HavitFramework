using System;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore.Infrastructure;

// ReSharper disable once CheckNamespace
// Správny namespace je Microsoft.EntityFrameworkCore! Konvencia Microsoftu.
namespace Microsoft.Extensions.DependencyInjection.Extensions
{
    /// <summary>
    /// Extension methods for adding and removing services to an <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Removes the first service in <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> with the same service type
        /// as <paramref name="descriptor" /> and adds <paramref name="descriptor" /> to the collection.
        /// </summary>
        /// <param name="collection">The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.</param>
        /// <param name="descriptor">The <see cref="T:Microsoft.Extensions.DependencyInjection.ServiceDescriptor" /> to replace with.</param>
        /// <returns>The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> for chaining.</returns>
        public static IServiceCollection ReplaceCoreService<TService, TImplementation>(this IServiceCollection collection)
        {
            Contract.Assert<ArgumentNullException>(collection != null);
            
            if (!EntityFrameworkServicesBuilder.CoreServices.TryGetValue(typeof(TService), out var serviceCharacteristics))
            {
                throw new ArgumentException($"Unknown Entity Framework service: {typeof(TService).FullName}");
            }

            var descriptor = ServiceDescriptor.Describe(typeof(TService), typeof(TImplementation), serviceCharacteristics.Lifetime);
            return collection.Replace(descriptor);
        }
    }
}
