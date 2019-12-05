using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Havit.Data.EntityFrameworkCore.Migrations.TestHelpers.Fakes
{
    public class FakeModelExtensionsAssemblyExtension : IDbContextOptionsExtension
    {
        private readonly IEnumerable<TypeInfo> modelExtenders;
        private DbContextOptionsExtensionInfo info;

        /// <inheritdoc />
        public DbContextOptionsExtensionInfo Info => info ??= new ExtensionInfo(this);

        public FakeModelExtensionsAssemblyExtension(IEnumerable<TypeInfo> modelExtenders)
        {
            Contract.Requires<ArgumentNullException>(modelExtenders != null);

            this.modelExtenders = modelExtenders;
        }

        public void ApplyServices(IServiceCollection services)
        {
            services.Replace(ServiceDescriptor.Scoped(typeof(IModelExtensionsAssembly), p => new FakeModelExtensionsAssembly(modelExtenders)));
        }

        public void Validate(IDbContextOptions options)
        {
            // NOOP
        }

        private class ExtensionInfo : DbContextOptionsExtensionInfo
        {
            private string logFragment;

            public ExtensionInfo(IDbContextOptionsExtension extension)
                : base(extension)
            {
            }

            private new FakeModelExtensionsAssemblyExtension Extension => (FakeModelExtensionsAssemblyExtension)base.Extension;

            public override bool IsDatabaseProvider => false;

            public override string LogFragment => logFragment ??= "using fake ModelExtensionsAssembly ";

            public override long GetServiceProviderHashCode() => Extension.modelExtenders.Aggregate(0, (value, type) => (value * 397) ^ type.GetHashCode());

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            {
                //debugInfo["ConventionType:" + nameof(Extension.ConventionType)] = (Extension.ConventionType.GetHashCode()).ToString(CultureInfo.InvariantCulture);
            }
        }

    }
}