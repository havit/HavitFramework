using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections
{
	public class DbInjectionsExtension : IDbContextOptionsExtension
    {
        private ImmutableList<Type> annotationProviders = ImmutableList.Create<Type>();
        private ImmutableList<Type> sqlGenerators = ImmutableList.Create<Type>();
	    private bool consolidateStatementsForMigrationsAnnotationsForModel = true;

	    public string LogFragment => "";

	    public DbInjectionsExtension()
	    {
	    }

	    // NB: When adding new options, make sure to update the copy ctor below.

		protected DbInjectionsExtension(DbInjectionsExtension copyFrom)
	    {
		    annotationProviders = copyFrom.annotationProviders;
		    sqlGenerators = copyFrom.sqlGenerators;
		    consolidateStatementsForMigrationsAnnotationsForModel = copyFrom.consolidateStatementsForMigrationsAnnotationsForModel;
	    }

		protected virtual DbInjectionsExtension Clone() => new DbInjectionsExtension(this);

	    public bool ConsolidateStatementsForMigrationsAnnotationsForModel => consolidateStatementsForMigrationsAnnotationsForModel;

		public DbInjectionsExtension WithConsolidateStatementsForMigrationsAnnotationsForModel(bool consolidateStatementsForMigrationsAnnotationsForModel)
	    {
		    var clone = Clone();
		    clone.consolidateStatementsForMigrationsAnnotationsForModel = consolidateStatementsForMigrationsAnnotationsForModel;
		    return clone;
	    }

		public DbInjectionsExtension WithAnnotationProvider<T>()
            where T : IDbInjectionAnnotationProvider
	    {
			// clone with new IDbInjectionAnnotationProvider
			// https://github.com/aspnet/EntityFrameworkCore/issues/10559#issuecomment-351753702
		    // https://github.com/aspnet/EntityFrameworkCore/blob/779d43731773d59ecd5f899a6330105879263cf3/src/EFCore.InMemory/Infrastructure/Internal/InMemoryOptionsExtension.cs#L47
			var clone = Clone();
		    clone.annotationProviders = clone.annotationProviders.Add(typeof(T));
		    return clone;
	    }

	    public DbInjectionsExtension WithSqlGenerator<T>()
            where T : IDbInjectionSqlGenerator
		{
			// clone with new IDbInjectionSqlGenerator 
			var clone = Clone();
			clone.sqlGenerators = clone.sqlGenerators.Add(typeof(T));
			return clone;
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
	        if (ConsolidateStatementsForMigrationsAnnotationsForModel)
	        {
		        var serviceCharacteristics = EntityFrameworkRelationalServicesBuilder.RelationalServices[typeof(IMigrationsModelDiffer)];

				services.Add(ServiceDescriptor.Describe(typeof(IMigrationsModelDiffer), typeof(AlterDatabaseFixUpMigrationsModelDiffer), serviceCharacteristics.Lifetime));
			}

            return false;
        }

	    public long GetServiceProviderHashCode()
        {
            var hashCode = annotationProviders.Aggregate(358, (current, next) => current ^ next.GetHashCode());
	        hashCode = sqlGenerators.Aggregate(hashCode, (current, next) => current ^ next.GetHashCode());
	        return hashCode;
        }

	    public void Validate(IDbContextOptions options)
        {
            // no validations
        }
    }
}