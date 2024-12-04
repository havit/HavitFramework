using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Extensions.DependencyInjection.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Extensions.DependencyInjection.SourceGenerators.Tests;

[TestClass]
public class ServiceAttributeConstantsTests
{
	[TestMethod]
	public void ServiceAttributeConstants_ServiceAttributeFullnames()
	{
		// Arrange
		// noop

		// Act+Assert
		Assert.AreEqual(typeof(ServiceAttribute).FullName, ServiceAttributeConstants.ServiceAttributeNonGenericFullname);
		Assert.AreEqual(typeof(ServiceAttribute<>).FullName, ServiceAttributeConstants.ServiceAttributeGeneric1Fullname);
		Assert.AreEqual(typeof(ServiceAttribute<,>).FullName, ServiceAttributeConstants.ServiceAttributeGeneric2Fullname);
		Assert.AreEqual(typeof(ServiceAttribute<,,>).FullName, ServiceAttributeConstants.ServiceAttributeGeneric3Fullname);
		Assert.AreEqual(typeof(ServiceAttribute<,,,>).FullName, ServiceAttributeConstants.ServiceAttributeGeneric4Fullname);
	}

	[TestMethod]
	public void ServiceAttributeConstants_DefaultProfile()
	{
		// Arrange
		// noop

		// Act+Assert
		Assert.AreEqual(ServiceAttribute.DefaultProfile, ServiceAttributeConstants.DefaultProfile);
	}

	[TestMethod]
	public void ServiceAttributeConstants_PropertyNames()
	{
		// Arrange
		// noop

		// Act+Assert
		Assert.AreEqual(nameof(ServiceAttributeBase.Lifetime), ServiceAttributeConstants.LifetimePropertyName);
		Assert.AreEqual(nameof(ServiceAttributeBase.Profile), ServiceAttributeConstants.ProfilePropertyName);
		Assert.AreEqual(nameof(ServiceAttribute.ServiceType), ServiceAttributeConstants.ServiceTypePropertyName);
		Assert.AreEqual(nameof(ServiceAttribute.ServiceTypes), ServiceAttributeConstants.ServiceTypesPropertyName);
	}

	[TestMethod]
	public void ServiceAttributeConstants_DefaultLifetime()
	{
		// Arrange
		var serviceAttribute = new ServiceAttribute();

		// Act +Assert
		Assert.AreEqual(ServiceAttributeConstants.DefaultLifetime, serviceAttribute.Lifetime);
	}
}
