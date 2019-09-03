using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Havit.Data.EntityFrameworkCore.Migrations.Infrastructure
{
    /// <summary>
    /// <see cref="IDbContextOptionsExtension"/> for configuring <see cref="CompositeMigrationsSqlGenerator"/>.
    /// </summary>
    public class CompositeMigrationsSqlGeneratorExtension : IDbContextOptionsExtension
	{
		private ImmutableList<Type> generatorTypes = ImmutableList.Create<Type>();

        /// <inheritdoc />
        public string LogFragment => "";

        /// <inheritdoc />
        public CompositeMigrationsSqlGeneratorExtension()
		{
		}

        /// <inheritdoc />
        protected CompositeMigrationsSqlGeneratorExtension(CompositeMigrationsSqlGeneratorExtension copyFrom)
		{
			generatorTypes = copyFrom.generatorTypes;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        protected virtual CompositeMigrationsSqlGeneratorExtension Clone() => new CompositeMigrationsSqlGeneratorExtension(this);

        /// <summary>
        /// Registers specified type of <see cref="IMigrationOperationSqlGenerator"/>.
        /// </summary>
        /// <typeparam name="T">Implementation of <see cref="IMigrationOperationSqlGenerator"/></typeparam>
        /// <returns>Clone of <see cref="CompositeMigrationsSqlGeneratorExtension"/>.</returns>
        public CompositeMigrationsSqlGeneratorExtension WithGeneratorType<T>()
            where T : IMigrationOperationSqlGenerator
		{
			var clone = Clone();
			clone.generatorTypes = generatorTypes.Add(typeof(T));
			return clone;
		}

        /// <inheritdoc />
        public bool ApplyServices(IServiceCollection services)
        {
            var currentProviderTypes = generatorTypes.ToArray();
            CompositeMigrationsSqlGenerator Factory(IServiceProvider serviceProvider)
            {
                var generators = currentProviderTypes.Select(type => (IMigrationOperationSqlGenerator)serviceProvider.GetService(type)).ToArray();
                return new CompositeMigrationsSqlGenerator(serviceProvider.GetService<MigrationsSqlGeneratorDependencies>(), serviceProvider.GetService<IMigrationsAnnotationProvider>(), generators);
            }

            services.Add(currentProviderTypes.Select(t => ServiceDescriptor.Singleton(t, t)));
            services.Replace(ServiceDescriptor.Singleton<IMigrationsSqlGenerator, CompositeMigrationsSqlGenerator>(Factory));

            return false;
        }

        /// <inheritdoc />
        public long GetServiceProviderHashCode()
        {
            return generatorTypes.Aggregate(358, (current, next) => current ^ next.GetHashCode());
        }

        /// <inheritdoc />
        public void Validate(IDbContextOptions options)
        {
            // no validation
        }
    }
}