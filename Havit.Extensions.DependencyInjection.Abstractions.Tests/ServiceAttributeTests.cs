using Havit.Extensions.DependencyInjection.Abstractions;
using Havit.Extensions.DependencyInjection.Abstractions.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Extensions.DependencyInjection.Tests;

[TestClass]
public class ServiceAttributeTests
{
	[TestMethod]
	[ExpectedException(typeof(InvalidOperationException), AllowDerivedTypes = false)]
	public void ServiceAttribute_CannotHaveBothServiceTypeAndServiceTypes()
	{
		// Arrange
		// noop

		// Act
		new ServiceAttribute { ServiceType = typeof(ServiceAttributeTests), ServiceTypes = new[] { typeof(ServiceAttributeTests) } };

		// Assert
		// exception was thrown
	}

	[TestMethod]
	[ExpectedException(typeof(ArgumentException), AllowDerivedTypes = false)]
	public void ServiceAttribute_CannotSetEmptyServiceTypes()
	{
		// Arrange
		// noop

		// Act
		new ServiceAttribute { ServiceTypes = new Type[] { } };

		// Assert
		// exception was thrown
	}


	[TestMethod]
	public void ServiceAttribute_CanUseServiceTypeAndServiceTypes()
	{
		// Arrange
		// noop

		// Act
		new ServiceAttribute { ServiceType = typeof(IFirstService) };
		new ServiceAttribute { ServiceTypes = new[] { typeof(IFirstService) } };

		// Assert
		// no exception was thrown
	}

	[TestMethod]
	public void ServiceAttribute_GetServiceTypes()
	{
		// Arrange
		// noop

		// Act
		Type[] serviceTypes1 = new ServiceAttribute { ServiceType = typeof(IFirstService) }.GetServiceTypes();
		Type[] serviceTypes2 = new ServiceAttribute { ServiceTypes = new[] { typeof(IFirstService), typeof(ISecondService) } }.GetServiceTypes();

		// Assert
		CollectionAssert.AreEquivalent(new[] { typeof(IFirstService) }, serviceTypes1);
		CollectionAssert.AreEquivalent(new[] { typeof(IFirstService), typeof(ISecondService) }, serviceTypes2);
	}

	[TestMethod]
	public void ServiceAttribute_ToString()
	{
		// Arrange
		// noop

		// Act
		string result = new ServiceAttribute
		{
			Profile = "MyProfile",
			Lifetime = ServiceLifetime.Singleton,
			ServiceTypes = new Type[] { typeof(IFirstService), typeof(ISecondService) }				
		}.ToString();

		// Assert
		Assert.AreEqual("Profile = \"MyProfile\", ServiceTypes = { Havit.Extensions.DependencyInjection.Abstractions.Tests.Infrastructure.IFirstService, Havit.Extensions.DependencyInjection.Abstractions.Tests.Infrastructure.ISecondService }, Lifetime = Singleton", result);
	}

}
