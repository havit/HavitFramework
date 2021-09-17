using Castle.Windsor;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;
using Havit.Data.EntityFrameworkCore.Patterns.Windsor.Installers.Infrastructure;
using Havit.Data.Patterns.DataLoaders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Patterns.Windsor.Tests.Infrastructure
{
    [TestClass]
    public class WindsorContainerServiceInstallerTests
    {
        [TestMethod]
        public void ServiceCollectionServiceInstaller_AddService_AllowsAddMultipleServiceForSingleServiceType()
        {
            // Arrange
            WindsorContainer windsorContainer = new WindsorContainer();
            WindsorContainerServiceInstaller windsorContainerServiceInstaller = new WindsorContainerServiceInstaller(windsorContainer);

            // Act
            windsorContainerServiceInstaller.AddServiceTransient<IDataLoader, DbDataLoader>();
            windsorContainerServiceInstaller.AddServiceTransient<IDataLoader, DbDataLoaderWithLoadedPropertiesMemory>();

            // Assert
            Assert.AreEqual(2, windsorContainer.Kernel.GetAssignableHandlers(typeof(object)).Count());
        }

        [TestMethod]
        public void ServiceCollectionServiceInstaller_TryAddService_DoesNotAddMultipleServicesForSingleServiceType()
        {
            // Arrange
            WindsorContainer windsorContainer = new WindsorContainer();
            WindsorContainerServiceInstaller windsorContainerServiceInstaller = new WindsorContainerServiceInstaller(windsorContainer);

            // Act
            windsorContainerServiceInstaller.TryAddServiceTransient<IDataLoader, DbDataLoader>();
            windsorContainerServiceInstaller.TryAddServiceTransient<IDataLoader, DbDataLoaderWithLoadedPropertiesMemory>();

            // Assert
            Assert.AreEqual(1, windsorContainer.Kernel.GetAssignableHandlers(typeof(object)).Count());
        }

        [TestMethod]
        public void ServiceCollectionServiceInstaller_TryAddService_AddMultipleServicesForMultipleServiceType()
        {
            // Arrange
            WindsorContainer windsorContainer = new WindsorContainer();
            WindsorContainerServiceInstaller windsorContainerServiceInstaller = new WindsorContainerServiceInstaller(windsorContainer);

            // Act
            windsorContainerServiceInstaller.TryAddServiceTransient<IDataLoader, DbDataLoader>();
            windsorContainerServiceInstaller.TryAddServiceTransient<IEntityCacheManager, EntityCacheManager>();

            // Assert
            Assert.AreEqual(2, windsorContainer.Kernel.GetAssignableHandlers(typeof(object)).Count());
        }

    }
}
