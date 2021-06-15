using Havit.Extensions.DependencyInjection.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Extensions.DependencyInjection.Tests
{
	[TestClass]
	public class ServiceCollectionExtensionsTests
	{
		[TestMethod]
		public void ServiceCollectionExtensions_AddByServiceAttribute_ServiceWithOneDefaultInterface()
		{
			// Arrange
			ServiceCollection services = new ServiceCollection();

			// Act
			services.AddByServiceAttribute(typeof(MyService).Assembly, nameof(MyService));
			services.BuildServiceProvider().GetRequiredService<IService>();

			// Assert
			// assert: no exception was thrown
		}

		[TestMethod]
		public void ServiceCollectionExtensions_AddByServiceAttribute_ServiceWithTwoDefaultInterfaces()
		{
			// Arrange
			ServiceCollection services = new ServiceCollection();

			// Act
			services.AddByServiceAttribute(typeof(MyFirstService).Assembly, nameof(MyFirstService));
			services.BuildServiceProvider().GetRequiredService<IService>();
			services.BuildServiceProvider().GetRequiredService<IFirstService>();

			// Assert
			// assert: no exception was thrown
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = false)]
		public void ServiceCollectionExtensions_AddByServiceAttribute_DoesNotAddsServiceWithNoInterface()
		{
			// Arrange
			ServiceCollection services = new ServiceCollection();

			// Act
			services.AddByServiceAttribute(typeof(NoInterfaceService).Assembly, nameof(NoInterfaceService));

			// Assert
			// assert: exception was thrown
		}

		[TestMethod]
		public void ServiceCollectionExtensions_AddByServiceAttribute_ClassWithExplicitServiceTypes()
		{
			// Arrange
			ServiceCollection services = new ServiceCollection();

			// Act
			services.AddByServiceAttribute(typeof(MyFirstAndSecondService).Assembly, nameof(MyFirstAndSecondService));
			ServiceProvider serviceProvider = services.BuildServiceProvider();
			IService firstService = serviceProvider.GetRequiredService<IFirstService>();
			IService secondService = serviceProvider.GetRequiredService<ISecondService>();

			// Assert
			// assert: no exception was thrown
			Assert.AreSame(firstService, secondService);
		}


		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = false)]
		public void ServiceCollectionExtensions_AddByServiceAttribute_ClassWithExplicitServiceTypeDoesNotRegisterBaseInterfaces()
		{
			// Arrange
			ServiceCollection services = new ServiceCollection();

			// Act
			services.AddByServiceAttribute(typeof(MyFirstAndSecondService).Assembly, nameof(MyFirstAndSecondService));
			services.BuildServiceProvider().GetRequiredService<IService>();

			// Assert
			// assert: exception was thrown
		}
	}
}
