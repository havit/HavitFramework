using Havit.Extensions.DependencyInjection.Abstractions;

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
		// Chceme ověřit, že hodnoty konstant v kódu jsou rovny názvu jednotlivých typů, takže hodnoty expected a actual jsou správně.
		// Pravidlo MSTEST0017 však neočekává, že by někdo testoval konstantu a proto doporučuje prohození argumentů, což nechceme.
#pragma warning disable MSTEST0017 // Assertion arguments should be passed in the correct order
		Assert.AreEqual(expected: typeof(ServiceAttribute).FullName, actual: ServiceAttributeConstants.ServiceAttributeNonGenericFullname);
		Assert.AreEqual(expected: typeof(ServiceAttribute<>).FullName, actual: ServiceAttributeConstants.ServiceAttributeGeneric1Fullname);
		Assert.AreEqual(expected: typeof(ServiceAttribute<,>).FullName, actual: ServiceAttributeConstants.ServiceAttributeGeneric2Fullname);
		Assert.AreEqual(expected: typeof(ServiceAttribute<,,>).FullName, actual: ServiceAttributeConstants.ServiceAttributeGeneric3Fullname);
		Assert.AreEqual(expected: typeof(ServiceAttribute<,,,>).FullName, actual: ServiceAttributeConstants.ServiceAttributeGeneric4Fullname);
#pragma warning restore MSTEST0017 // Assertion arguments should be passed in the correct order
	}

	[TestMethod]
	public void ServiceAttributeConstants_DefaultProfile()
	{
		// Arrange
		// noop

		// Act+Assert
		// Chceme ověřit, že hodnota konstanty v kódu je rovna hodnotě jiné konstanty (ale mezi třídami není vztah,
		// tak to nelze zapsat v kódu), takže hodnoty expected a actual jsou správně.
		// Pravidlo MSTEST0032 však neočekává, že by někdo testoval konstanty.
#pragma warning disable MSTEST0032 // Assertion arguments should be passed in the correct order
		Assert.AreEqual(expected: ServiceAttribute.DefaultProfile, actual: ServiceAttributeConstants.DefaultProfile);
#pragma warning restore MSTEST0032 // Assertion arguments should be passed in the correct order
	}

	[TestMethod]
	public void ServiceAttributeConstants_PropertyNames()
	{
		// Arrange
		// noop

		// Act+Assert
		// Chceme ověřit, že hodnoty konstant v kódu jsou rovny názvu jednotlivých hodnot, takže hodnoty expected a actual jsou správně.
		// Pravidlo MSTEST0032 však neočekává, že by někdo testoval konstanty.
#pragma warning disable MSTEST0032 // Assertion arguments should be passed in the correct order
		Assert.AreEqual(expected: nameof(ServiceAttributeBase.Lifetime), actual: ServiceAttributeConstants.LifetimePropertyName);
		Assert.AreEqual(expected: nameof(ServiceAttributeBase.Profile), actual: ServiceAttributeConstants.ProfilePropertyName);
		Assert.AreEqual(expected: nameof(ServiceAttribute.ServiceType), actual: ServiceAttributeConstants.ServiceTypePropertyName);
		Assert.AreEqual(expected: nameof(ServiceAttribute.ServiceTypes), actual: ServiceAttributeConstants.ServiceTypesPropertyName);
#pragma warning restore MSTEST0017 // Assertion arguments should be passed in the correct order
	}

	[TestMethod]
	public void ServiceAttributeConstants_DefaultLifetime()
	{
		// Arrange
		var serviceAttribute = new ServiceAttribute();

		// Act +Assert
		Assert.AreEqual(ServiceAttributeConstants.DefaultLifetime, serviceAttribute.Lifetime.ToString());
	}
}
