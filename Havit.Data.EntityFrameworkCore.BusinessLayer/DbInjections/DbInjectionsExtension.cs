using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections
{
    public class DbInjectionsExtension : IDbContextOptionsExtension
    {
        private readonly List<Type> annotationProviders = new List<Type>();
        private readonly List<Type> sqlGenerators = new List<Type>();

	    public string LogFragment => "";

	    public DbInjectionsOptions Options { get; private set; } = new DbInjectionsOptions();

	    public DbInjectionsExtension WithAnnotationProvider<T>()
            where T : IDbInjectionAnnotationProvider
        {
            annotationProviders.Add(typeof(T));
            return this;
        }

	    public DbInjectionsExtension WithSqlGenerator<T>()
            where T : IDbInjectionSqlGenerator
        {
            sqlGenerators.Add(typeof(T));
            return this;
        }

	    public DbInjectionsExtension WithOptions(DbInjectionsOptions options)
	    {
			Contract.Requires<ArgumentNullException>(options != null);
		    Options = options;
		    return this;
	    }

	    public bool ApplyServices(IServiceCollection services)
        {
            var currentProviderTypes = annotationProviders.ToArray();
            CompositeDbInjectionAnnotationProvider AnnotationProviderFactory(IServiceProvider serviceProvider)
            {
                var providers = currentProviderTypes.Select(type => (IDbInjectionAnnotationProvider)serviceProvider.GetService(type)).ToArray();
                return new CompositeDbInjectionAnnotationProvider(providers);
            }
            var currentSqlGeneratorTypes = sqlGenerators.ToArray();
            DbInjectionSqlResolver DropSqlResolverFactory(IServiceProvider serviceProvider)
            {
                var generators = currentSqlGeneratorTypes.Select(type => (IDbInjectionSqlGenerator)serviceProvider.GetService(type)).ToArray();
                return new DbInjectionSqlResolver(generators);
            }

            services.Add(annotationProviders.ToArray().Select(t => ServiceDescriptor.Singleton(t, t)));
            services.Add(sqlGenerators.ToArray().Select(t => ServiceDescriptor.Singleton(t, t)));
            services.AddSingleton<IDbInjectionAnnotationProvider, CompositeDbInjectionAnnotationProvider>(AnnotationProviderFactory);
            services.AddSingleton<IDbInjectionSqlResolver, DbInjectionSqlResolver>(DropSqlResolverFactory);
	        if (Options.RemoveUnnecessaryStatementsForMigrationsAnnotationsForModel)
	        {
		        var serviceCharacteristics = EntityFrameworkRelationalServicesBuilder.RelationalServices[typeof(IMigrationsModelDiffer)];

				services.Add(ServiceDescriptor.Describe(typeof(IMigrationsModelDiffer), typeof(AlterDatabaseFixUpMigrationsModelDiffer), serviceCharacteristics.Lifetime));
			}

            return false;
        }

	    public long GetServiceProviderHashCode()
        {
            return annotationProviders.Aggregate(358, (current, next) => current ^ next.GetHashCode());
        }

	    public void Validate(IDbContextOptions options)
        {
            // no validations
        }
    }
}