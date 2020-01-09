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
		public void ServiceCollectionExtensions_AddByServiceAttribute_AddsOneClassWithOneInterfaces()
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
		public void ServiceCollectionExtensions_AddByServiceAttribute_AddsOneClassWithTwoInterfaces()
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
		public void ServiceCollectionExtensions_AddByServiceAttribute_DoesNotAddsClassWithNoInterface()
		{
			// Arrange
			ServiceCollection services = new ServiceCollection();

			// Act
			services.AddByServiceAttribute(typeof(NoInterfaceService).Assembly, nameof(NoInterfaceService));

			// Assert
			// assert: exception was thrown
		}
	}
}
