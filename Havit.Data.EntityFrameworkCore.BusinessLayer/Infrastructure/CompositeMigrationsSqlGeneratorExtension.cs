using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Infrastructure
{
	internal class CompositeMigrationsSqlGeneratorExtension : IDbContextOptionsExtension
	{
		private ImmutableList<Type> generatorTypes = ImmutableList.Create<Type>();

		public string LogFragment => "";

		public CompositeMigrationsSqlGeneratorExtension()
		{
		}

		protected CompositeMigrationsSqlGeneratorExtension(CompositeMigrationsSqlGeneratorExtension copyFrom)
		{
			generatorTypes = copyFrom.generatorTypes;
		}

		protected virtual CompositeMigrationsSqlGeneratorExtension Clone() => new CompositeMigrationsSqlGeneratorExtension(this);

		public CompositeMigrationsSqlGeneratorExtension WithGeneratorType<T>()
            where T : IMigrationOperationSqlGenerator
		{
			var clone = Clone();
			clone.generatorTypes = generatorTypes.Add(typeof(T));
			return clone;
		}

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

        public long GetServiceProviderHashCode()
        {
            return generatorTypes.Aggregate(358, (current, next) => current ^ next.GetHashCode());
        }

        public void Validate(IDbContextOptions options)
        {
            // no validation
        }
    }
}