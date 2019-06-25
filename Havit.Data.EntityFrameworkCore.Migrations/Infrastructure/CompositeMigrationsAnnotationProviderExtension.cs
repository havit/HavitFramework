﻿using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Havit.Data.EntityFrameworkCore.Migrations.Infrastructure
{
    /// <summary>
    /// <see cref="IDbContextOptionsExtension"/> for configuring <see cref="CompositeMigrationsAnnotationProvider"/>.
    /// </summary>
    public class CompositeMigrationsAnnotationProviderExtension : IDbContextOptionsExtension
    {
        private ImmutableList<Type> providers = ImmutableList.Create<Type>();

        /// <inheritdoc />
        public string LogFragment => "";

        /// <inheritdoc />
        public CompositeMigrationsAnnotationProviderExtension()
	    {
	    }

        /// <inheritdoc />
        protected CompositeMigrationsAnnotationProviderExtension(CompositeMigrationsAnnotationProviderExtension copyFrom)
	    {
		    providers = copyFrom.providers;
	    }

	    /// <summary>
	    /// Clones this instance.
	    /// </summary>
	    protected virtual CompositeMigrationsAnnotationProviderExtension Clone() => new CompositeMigrationsAnnotationProviderExtension(this);

	    /// <summary>
	    /// Registers specified type of <see cref="IMigrationsAnnotationProvider"/>.
	    /// </summary>
	    /// <typeparam name="T">Implementation of <see cref="IMigrationsAnnotationProvider"/></typeparam>
	    /// <returns>Clone of <see cref="CompositeMigrationsAnnotationProviderExtension"/>.</returns>
	    public CompositeMigrationsAnnotationProviderExtension WithAnnotationProvider<T>()
            where T : IMigrationsAnnotationProvider
	    {
		    var clone = Clone();
		    clone.providers = providers.Add(typeof(T));
			return clone;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public long GetServiceProviderHashCode()
        {
            return providers.Aggregate(358, (current, next) => current ^ next.GetHashCode());
        }

        /// <inheritdoc />
        public void Validate(IDbContextOptions options)
        {
            // no validation
        }
    }
}