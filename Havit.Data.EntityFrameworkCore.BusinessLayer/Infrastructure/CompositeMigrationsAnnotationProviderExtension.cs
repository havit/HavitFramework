using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Infrastructure
{
	internal class CompositeMigrationsAnnotationProviderExtension : IDbContextOptionsExtension
    {
        private ImmutableList<Type> providers = ImmutableList.Create<Type>();

        public string LogFragment => "";

	    public CompositeMigrationsAnnotationProviderExtension()
	    {
	    }

	    protected CompositeMigrationsAnnotationProviderExtension(CompositeMigrationsAnnotationProviderExtension copyFrom)
	    {
		    providers = copyFrom.providers;
	    }

	    protected virtual CompositeMigrationsAnnotationProviderExtension Clone() => new CompositeMigrationsAnnotationProviderExtension(this);

	    public CompositeMigrationsAnnotationProviderExtension WithAnnotationProvider<T>()
            where T : IMigrationsAnnotationProvider
	    {
		    var clone = Clone();
		    clone.providers = providers.Add(typeof(T));
			return clone;
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