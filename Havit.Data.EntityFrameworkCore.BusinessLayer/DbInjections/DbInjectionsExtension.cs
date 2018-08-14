using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections
{
    public class DbInjectionsExtension : IDbContextOptionsExtension
    {
        private readonly List<Type> annotationProviders = new List<Type>();
        private readonly List<Type> dropSqlGenerators = new List<Type>();

        public string LogFragment => "";

        public DbInjectionsExtension WithAnnotationProvider<T>()
            where T : IDbInjectionAnnotationProvider
        {
            annotationProviders.Add(typeof(T));
            return this;
        }

        public DbInjectionsExtension WithDropSqlGenerator<T>()
            where T : IDbInjectionDropSqlGenerator
        {
            dropSqlGenerators.Add(typeof(T));
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
            var currentDropSqlGeneratorTypes = dropSqlGenerators.ToArray();
            DbInjectionDropSqlResolver DropSqlResolverFactory(IServiceProvider serviceProvider)
            {
                var generators = currentDropSqlGeneratorTypes.Select(type => (IDbInjectionDropSqlGenerator)serviceProvider.GetService(type)).ToArray();
                return new DbInjectionDropSqlResolver(generators);
            }

            services.Add(annotationProviders.ToArray().Select(t => ServiceDescriptor.Singleton(t, t)));
            services.Add(dropSqlGenerators.ToArray().Select(t => ServiceDescriptor.Singleton(t, t)));
            services.AddSingleton<IDbInjectionAnnotationProvider, CompositeDbInjectionAnnotationProvider>(AnnotationProviderFactory);
            services.AddSingleton<IDbInjectionDropSqlResolver, DbInjectionDropSqlResolver>(DropSqlResolverFactory);

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