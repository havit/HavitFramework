using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Infrastructure
{
    internal class CompositeMigrationsAnnotationProviderExtension : IDbContextOptionsExtension
    {
        private readonly List<Type> providers = new List<Type>();

        public string LogFragment => "";

        public CompositeMigrationsAnnotationProviderExtension WithAnnotationProvider<T>()
            where T : IMigrationsAnnotationProvider
        {
            providers.Add(typeof(T));
            return this;
        }

        public bool ApplyServices(IServiceCollection services)
        {
            var currentProviderTypes = providers.ToArray();
            CompositeMigrationsAnnotationProvider Factory(IServiceProvider serviceProvider)
            {
                var providers = currentProviderTypes.Select(type => (IMigrationsAnnotationProvider)serviceProvider.GetService(type)).ToArray();
                return new CompositeMigrationsAnnotationProvider(serviceProvider.GetRequiredService<MigrationsAnnotationProviderDependencies>(), providers);
            }

            services.Add(currentProviderTypes.Select(t => ServiceDescriptor.Singleton(t, t)));
            services.Replace(ServiceDescriptor.Singleton<IMigrationsAnnotationProvider, CompositeMigrationsAnnotationProvider>(Factory));

            return false;
        }

        public long GetServiceProviderHashCode()
        {
            return providers.Aggregate(358, (current, next) => current ^ next.GetHashCode());
        }

        public void Validate(IDbContextOptions options)
        {
            // no validation
        }
    }
}