using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;
using Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Infrastructure;
using Havit.Data.Patterns.DataLoaders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DependencyInjection.Infrastructure
{
    [TestClass]
    public class ServiceCollectionServiceInstallerTests
    {
        [TestMethod]
        public void ServiceCollectionServiceInstaller_AddService_AllowsAddMultipleServiceForSingleServiceType()
        {
            // Arrange
            ServiceCollection services = new ServiceCollection();
            ServiceCollectionServiceInstaller serviceCollectionServiceInstaller = new ServiceCollectionServiceInstaller(services);

            // Act
            serviceCollectionServiceInstaller.AddServiceTransient<IDataLoader, DbDataLoader>();
            serviceCollectionServiceInstaller.AddServiceTransient<IDataLoader, DbDataLoaderWithLoadedPropertiesMemory>();

            // Assert
            Assert.AreEqual(2, services.Count);
        }

        [TestMethod]
        public void ServiceCollectionServiceInstaller_TryAddService_DoesNotAddMultipleServicesForSingleServiceType()
        {
            // Arrange
            ServiceCollection services = new ServiceCollection();
            ServiceCollectionServiceInstaller serviceCollectionServiceInstaller = new ServiceCollectionServiceInstaller(services);

            // Act
            serviceCollectionServiceInstaller.TryAddServiceTransient<IDataLoader, DbDataLoader>();
            serviceCollectionServiceInstaller.TryAddServiceTransient<IDataLoader, DbDataLoaderWithLoadedPropertiesMemory>();

            // Assert
            Assert.AreEqual(1, services.Count);
            Assert.AreEqual(typeof(DbDataLoader), services.Single().ImplementationType); // first wins
        }

        [TestMethod]
        public void ServiceCollectionServiceInstaller_TryAddService_AddMultipleServicesForMultipleServiceType()
        {
            // Arrange
            ServiceCollection services = new ServiceCollection();
            ServiceCollectionServiceInstaller serviceCollectionServiceInstaller = new ServiceCollectionServiceInstaller(services);

            // Act
            serviceCollectionServiceInstaller.TryAddServiceTransient<IDataLoader, DbDataLoader>();
            serviceCollectionServiceInstaller.TryAddServiceTransient<IEntityCacheManager, EntityCacheManager>();

            // Assert
            Assert.AreEqual(2, services.Count);
        }

    }
}
