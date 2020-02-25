using System;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore.Infrastructure;

// ReSharper disable once CheckNamespace
// Správny namespace je Microsoft.EntityFrameworkCore! Konvencia Microsoftu.
namespace Microsoft.Extensions.DependencyInjection.Extensions
{
    /// <summary>
    /// Extension methods for adding and removing services to an <see cref="IServiceCollection" />.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Removes the first service in <see cref="IServiceCollection" /> of type <typeparamref name="TService"/>
        /// and adds <typeparamref name="TService"/> back using <typeparamref name="TImplementation"/> as implementation to the collection.
        /// Lifetime is used according to lifetimes specified in <see cref="EntityFrameworkServicesBuilder.CoreServices"/> or
        /// <see cref="EntityFrameworkRelationalServicesBuilder.RelationalServices"/>.
        /// </summary>
        /// <typeparam name="TService">The type of the service to replace.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation to use instead of original implementation.</typeparam>
        /// <param name="collection">The <see cref="IServiceCollection" />.</param>
        /// <returns>The <see cref="IServiceCollection" /> for chaining.</returns>
        public static IServiceCollection ReplaceCoreService<TService, TImplementation>(this IServiceCollection collection)
            where TService : class
            where TImplementation : class, TService
        {
            Contract.Assert<ArgumentNullException>(collection != null);

            if (!EntityFrameworkServicesBuilder.CoreServices.TryGetValue(typeof(TService), out var serviceCharacteristics) &&
                !EntityFrameworkRelationalServicesBuilder.RelationalServices.TryGetValue(typeof(TService), out serviceCharacteristics))
            {
                throw new ArgumentException($"Unknown Entity Framework service: {typeof(TService).FullName}");
            }

            var descriptor = ServiceDescriptor.Describe(typeof(TService), typeof(TImplementation), serviceCharacteristics.Lifetime);
            return collection.Replace(descriptor);
        }

        /// <summary>
        /// Removes the first service in <see cref="IServiceCollection" /> of type <typeparamref name="TService"/>
        /// and adds service of same type to the collection using the factory specified in <paramref name="implementationFactory" />.
        /// Lifetime is used according to lifetimes specified in <see cref="EntityFrameworkServicesBuilder.CoreServices"/> or
        /// <see cref="EntityFrameworkRelationalServicesBuilder.RelationalServices"/>.
        /// </summary>
        /// <typeparam name="TService">The type of the service to replace.</typeparam>
        /// <param name="collection">The <see cref="IServiceCollection" />.</param>
        /// <param name="implementationFactory">The factory that creates the service.</param>
        /// <returns>The <see cref="IServiceCollection" /> for chaining.</returns>
        public static IServiceCollection ReplaceCoreService<TService>(
            this IServiceCollection collection,
            Func<IServiceProvider, TService> implementationFactory)
            where TService : class
        {
            Contract.Assert<ArgumentNullException>(collection != null);

            if (!EntityFrameworkServicesBuilder.CoreServices.TryGetValue(typeof(TService), out var serviceCharacteristics) &&
                !EntityFrameworkRelationalServicesBuilder.RelationalServices.TryGetValue(typeof(TService), out serviceCharacteristics))
            {
                throw new ArgumentException($"Unknown Entity Framework service: {typeof(TService).FullName}");
            }

            var descriptor = ServiceDescriptor.Describe(typeof(TService), implementationFactory, serviceCharacteristics.Lifetime);
            return collection.Replace(descriptor);
        }
    }
}
