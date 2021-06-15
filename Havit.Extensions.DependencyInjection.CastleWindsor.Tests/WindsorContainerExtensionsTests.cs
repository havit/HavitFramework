using Castle.Windsor;
using Havit.Extensions.DependencyInjection.CastleWindsor.Tests.Infrastructure;
using Havit.Extensions.DependencyInjection.CastleWindsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel;

namespace Havit.Extensions.DependencyInjection.CastleWindsor.Tests
{
	[TestClass]
	public class WindsorContainerExtensionsTests
	{
		[TestMethod]
		public void WindsorContainerExtensions_AddByServiceAttribute_ServiceWithOneDefaultInterface()
		{
			// Arrange
			WindsorContainer container = new WindsorContainer();

			// Act
			container.InstallByServiceAttribute(typeof(MyService).Assembly, nameof(MyService));
			container.Resolve<IService>();

			// Assert
			// assert: no exception was thrown
		}

		[TestMethod]
		public void WindsorContainerExtensions_AddByServiceAttribute_ServiceWithTwoDefaultInterfaces()
		{
			// Arrange
			WindsorContainer container = new WindsorContainer();

			// Act
			container.InstallByServiceAttribute(typeof(MyFirstService).Assembly, nameof(MyFirstService));
			container.Resolve<IService>();
			container.Resolve<IFirstService>();

			// Assert
			// assert: no exception was thrown
		}

		[TestMethod]
		[ExpectedException(typeof(ComponentNotFoundException))]
		public void WindsorContainerExtensions_AddByServiceAttribute_ServiceWithInterfacesIsNotRegisteredThemselves()
		{
			// Arrange
			WindsorContainer container = new WindsorContainer();

			// Act
			container.InstallByServiceAttribute(typeof(MyFirstService).Assembly, nameof(MyFirstService));
			container.Resolve<MyFirstService>();

			// Assert
			// assert: exception was thrown
		}

		[TestMethod]
		public void WindsorContainerExtensions_AddByServiceAttribute_ServiceWithNoInterfaceIsRegisteredThemselves()
		{
			// Arrange
			WindsorContainer container = new WindsorContainer();

			// Act
			container.InstallByServiceAttribute(typeof(NoInterfaceService).Assembly, nameof(NoInterfaceService));
			container.Resolve<NoInterfaceService>();

			// Assert
			// assert: no exception was thrown
		}

		[TestMethod]
		public void WindsorContainerExtensions_AddByServiceAttribute_ClassWithExplicitServiceTypes()
		{
			// Arrange
			WindsorContainer container = new WindsorContainer();

			// Act
			container.InstallByServiceAttribute(typeof(MyFirstAndSecondService).Assembly, nameof(MyFirstAndSecondService));			
			IService firstService = container.Resolve<IFirstService>();
			IService secondService = container.Resolve<ISecondService>();

			// Assert
			// assert: no exception was thrown
			Assert.AreSame(firstService, secondService);
		}


		[TestMethod]
		[ExpectedException(typeof(ComponentNotFoundException))]
		public void WindsorContainerExtensions_AddByServiceAttribute_ClassWithExplicitServiceTypeDoesNotRegisterBaseInterfaces()
		{
			// Arrange
			WindsorContainer container = new WindsorContainer();

			// Act
			container.InstallByServiceAttribute(typeof(MyFirstAndSecondService).Assembly, nameof(MyFirstAndSecondService));
			container.Resolve<IService>();

			// Assert
			// assert: exception was thrown
		}
	}
}
