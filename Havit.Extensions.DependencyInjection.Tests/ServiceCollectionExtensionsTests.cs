using Havit.Extensions.DependencyInjection.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Extensions.DependencyInjection.Tests;

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
		using (ServiceProvider serviceProvider = services.BuildServiceProvider())
		{
			serviceProvider.GetRequiredService<IService>();
			serviceProvider.GetRequiredService<IFirstService>();
		}
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
		IService firstService;
		IService secondService;
		// Act
		services.AddByServiceAttribute(typeof(MyFirstAndSecondService).Assembly, nameof(MyFirstAndSecondService));
		using (ServiceProvider serviceProvider = services.BuildServiceProvider())
		{
			firstService = serviceProvider.GetRequiredService<IFirstService>();
			secondService = serviceProvider.GetRequiredService<ISecondService>();
		}

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

	[TestMethod]
	public void ServiceCollectionExtensions_AddByServiceAttribute_RegistersAndResolvesOpenGenericTypes()
	{
		// Arrange
		ServiceCollection services = new ServiceCollection();

		// Act
		services.AddByServiceAttribute(typeof(MyGenericService<object, object>).Assembly, nameof(MyGenericService<object, object>)); // <object, object> nemá žádnou souvislost s registrací!
		using (var serviceProvider = services.BuildServiceProvider())
		{
			serviceProvider.GetRequiredService<IGenericService<object, object>>();
			serviceProvider.GetRequiredService<IGenericService<string, string>>();
			serviceProvider.GetRequiredService<IGenericService<object, string>>();
		}
		// Assert: no exception was thrown
	}

	[TestMethod]
	public void ServiceCollectionExtensions_AddByServiceAttribute_RegistersAndResolvesOpenGenericTypesFromServiceType()
	{
		// Arrange
		ServiceCollection services = new ServiceCollection();

		// Act
		services.AddByServiceAttribute(typeof(AttributedGenericService<object, object>).Assembly, nameof(AttributedGenericService<object, object>)); // <object, object> nemá žádnou souvislost s registrací!
		using (var serviceProvider = services.BuildServiceProvider())
		{
			serviceProvider.GetRequiredService<IGenericService<object, object>>();
			serviceProvider.GetRequiredService<IGenericService<string, string>>();
			serviceProvider.GetRequiredService<IGenericService<object, string>>();
		}

		// Assert: no exception was thrown
	}

	[TestMethod]
	[ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = false)]
	public void ServiceCollectionExtensions_AddByServiceAttribute_DoesNotRegisterCloseGenericServices()
	{

		// Arrange
		ServiceCollection services = new ServiceCollection();

		// Act
		// StringService is an interface implementing IGenericService<string>
		services.AddByServiceAttribute(typeof(MyStringService<object>).Assembly, nameof(MyStringService<object>));

		// Assert
		// assert: exception was thrown
	}

	[TestMethod]
	public void ServiceCollectionExtensions_AddByServiceAttribute_ServiceWithGenericServiceAttribute()
	{
		// Arrange
		ServiceCollection services = new ServiceCollection();

		// Act
		services.AddByServiceAttribute(typeof(DecoratedGenericServiceAttributeService).Assembly, nameof(DecoratedGenericServiceAttributeService));
		services.BuildServiceProvider().GetRequiredService<DecoratedGenericServiceAttributeService>();

		// Assert
		// assert: no exception was thrown
	}

}
