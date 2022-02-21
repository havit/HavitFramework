using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Data.EntityFrameworkCore.Metadata.Conventions
{
	/// <summary>
	/// Builder konfigurace použití konvencí ve frameworku.
	/// </summary>
	public class FrameworkConventionSetOptionsBuilder
    {
        private readonly DbContextOptionsBuilder optionsBuilder;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="optionsBuilder"></param>
        public FrameworkConventionSetOptionsBuilder(DbContextOptionsBuilder optionsBuilder)
        {
            this.optionsBuilder = optionsBuilder;
        }

        /// <summary>
        /// Nastaví, zda se má použít CascadeDeleteToRestrictConvention.
        /// </summary>
        public FrameworkConventionSetOptionsBuilder UseCascadeDeleteToRestrictConvention(bool enabled)
        {
            WithOption(e => e.WithCascadeDeleteToRestrictConvention(enabled));
            return this;
        }

        /// <summary>
        /// Nastaví, zda se má použít CacheAttributeToAnnotationConvention.
        /// </summary>
        public FrameworkConventionSetOptionsBuilder UseCacheAttributeToAnnotationConvention(bool enabled)
        {
            WithOption(e => e.WithCacheAttributeToAnnotationConvention(enabled));
            return this;
        }

        /// <summary>
        /// Nastaví, zda se má použít DataTypeAttributeConvention.
        /// </summary>
        public FrameworkConventionSetOptionsBuilder UseDataTypeAttributeConvention(bool enabled)
        {
            WithOption(e => e.WithDataTypeAttributeConvention(enabled));
            return this;
        }

        /// <summary>
        /// Nastaví, zda se má použít StringPropertiesDefaultValueConvention.
        /// </summary>
        public FrameworkConventionSetOptionsBuilder UseStringPropertiesDefaultValueConvention(bool enabled)
        {
            WithOption(e => e.WithStringPropertiesDefaultValueConvention(enabled));
            return this;
        }

        /// <summary>
        /// Nastaví, zda se má použít ManyToManyEntityKeyDiscoveryConvention.
        /// </summary>
        public FrameworkConventionSetOptionsBuilder UseManyToManyEntityKeyDiscoveryConvention(bool enabled)
        {
            WithOption(e => e.WithManyToManyEntityKeyDiscoveryConvention(enabled));
            return this;
        }

        private void WithOption(Func<FrameworkConventionSetOptionsExtension, FrameworkConventionSetOptionsExtension> withFunc)
        {
            var frameworkConventionSetOptionsExtension = optionsBuilder.Options.FindExtension<FrameworkConventionSetOptionsExtension>() ?? new FrameworkConventionSetOptionsExtension();                            
            frameworkConventionSetOptionsExtension = withFunc(frameworkConventionSetOptionsExtension);
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(frameworkConventionSetOptionsExtension);
        }
    }
}