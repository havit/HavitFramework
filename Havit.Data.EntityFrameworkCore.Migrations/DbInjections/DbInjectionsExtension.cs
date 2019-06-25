using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Havit.Data.EntityFrameworkCore.Migrations.DbInjections
{
    /// <summary>
    /// <see cref="IDbContextOptionsExtension"/> for configuring <see cref="IMigrationsAnnotationProvider"/>s and <see cref="IDbInjectionSqlGenerator"/>s.
    /// </summary>
	public class DbInjectionsExtension : IDbContextOptionsExtension
    {
        private ImmutableList<Type> annotationProviders = ImmutableList.Create<Type>();
        private ImmutableList<Type> sqlGenerators = ImmutableList.Create<Type>();
	    private bool consolidateStatementsForMigrationsAnnotationsForModel = true;

        /// <inheritdoc />
        public string LogFragment => "";

        /// <inheritdoc />
        public DbInjectionsExtension()
	    {
	    }

	    // NB: When adding new options, make sure to update the copy ctor below.

		/// <summary>
		/// Copy constructor.
		///
		/// <remarks>Pattern from original EF Core source.</remarks>
		/// </summary>
		protected DbInjectionsExtension(DbInjectionsExtension copyFrom)
	    {
		    annotationProviders = copyFrom.annotationProviders;
		    sqlGenerators = copyFrom.sqlGenerators;
		    consolidateStatementsForMigrationsAnnotationsForModel = copyFrom.consolidateStatementsForMigrationsAnnotationsForModel;
	    }

        /// <summary>
        /// Clones this <see cref="IDbContextOptionsExtension"/>.
        ///
        /// <remarks>Pattern from original EF Core source.</remarks>
        /// </summary>
        protected virtual DbInjectionsExtension Clone() => new DbInjectionsExtension(this);

        /// <summary>
        /// Specifies, whether generated code statements in migrations with annotations should be consolidated.
        /// 
        /// If enabled <see cref="AlterOperationsFixUpMigrationsModelDiffer"/> is used instead of original implementation of <see cref="IMigrationsModelDiffer"/>.
        /// </summary>
        public bool ConsolidateStatementsForMigrationsAnnotationsForModel => consolidateStatementsForMigrationsAnnotationsForModel;

        /// <summary>
        /// Consolidate generated code statements in migrations with annotations (e.g. AlterDatabase().Annotation().OldAnnotation()).
        ///
        /// Enables or disables <see cref="AlterOperationsFixUpMigrationsModelDiffer"/>.
        /// </summary>
        public DbInjectionsExtension WithConsolidateStatementsForMigrationsAnnotationsForModel(bool consolidateStatementsForMigrationsAnnotationsForModel)
	    {
		    var clone = Clone();
		    clone.consolidateStatementsForMigrationsAnnotationsForModel = consolidateStatementsForMigrationsAnnotationsForModel;
		    return clone;
	    }

        /// <summary>
        /// Registers <see cref="IDbInjectionAnnotationProvider"/> to use.
        /// </summary>
        /// <typeparam name="T">Implementation of <see cref="IDbInjectionAnnotationProvider"/> to register.</typeparam>
        /// <returns>A new instance of <see cref="DbInjectionsExtension"/> with option changed.</returns>
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

	    /// <summary>
	    /// Registers <see cref="IDbInjectionSqlGenerator"/> to use.
	    /// </summary>
	    /// <typeparam name="T">Implementation of <see cref="IDbInjectionSqlGenerator"/> to register.</typeparam>
	    /// <returns>A new instance of <see cref="DbInjectionsExtension"/> with option changed.</returns>
	    public DbInjectionsExtension WithSqlGenerator<T>()
            where T : IDbInjectionSqlGenerator
		{
			// clone with new IDbInjectionSqlGenerator 
			var clone = Clone();
			clone.sqlGenerators = clone.sqlGenerators.Add(typeof(T));
			return clone;
		}

        /// <inheritdoc />
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

				services.Add(ServiceDescriptor.Describe(typeof(IMigrationsModelDiffer), typeof(AlterOperationsFixUpMigrationsModelDiffer), serviceCharacteristics.Lifetime));
			}

            return false;
        }

        /// <inheritdoc />
        public long GetServiceProviderHashCode()
        {
            var hashCode = annotationProviders.Aggregate(358, (current, next) => current ^ next.GetHashCode());
	        hashCode = sqlGenerators.Aggregate(hashCode, (current, next) => current ^ next.GetHashCode());
	        return hashCode;
        }

        /// <inheritdoc />
        public void Validate(IDbContextOptions options)
        {
            // no validations
        }
    }
}