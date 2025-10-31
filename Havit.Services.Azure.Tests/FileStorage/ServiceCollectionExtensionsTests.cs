using Azure.Core;
using Havit.Services.Azure.FileStorage;
using Havit.Services.Azure.Tests.FileStorage.Infrastructure;
using Havit.Services.FileStorage;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Havit.Services.Azure.Tests.FileStorage;

[TestClass]
public class ServiceCollectionExtensionsTests
{
	[TestMethod]
	public void ServiceCollectionExtensions_AddAzureBlobStorageService()
	{
		// Arrange
		ServiceCollection services = new ServiceCollection();

		// Act
		services.AddAzureBlobStorageService<TestFileStorage>("DefaultEndpointsProtocol=https;AccountName=fake;AccountKey=fake", "fake");

		var provider = services.BuildServiceProvider();
		var service = provider.GetService<IFileStorageService<TestFileStorage>>();

		// Assert
		Assert.IsNotNull(service);
		Assert.IsInstanceOfType(service, typeof(AzureBlobStorageService<TestFileStorage>));
	}

	[TestMethod]
	public void ServiceCollectionExtensions_AddAzureBlobStorageService_WithTokenCredential()
	{
		// Arrange
		ServiceCollection services = new ServiceCollection();

		// Act
		services.AddAzureBlobStorageService<TestFileStorage>("fake", "fake", new Mock<TokenCredential>().Object);

		var provider = services.BuildServiceProvider();
		var service = provider.GetService<IFileStorageService<TestFileStorage>>();

		// Assert
		Assert.IsNotNull(service);
		Assert.IsInstanceOfType(service, typeof(AzureBlobStorageService<TestFileStorage>));
	}

	[TestMethod]
	public void ServiceCollectionExtensions_AddAzureFileStorageService()
	{
		// Arrange
		ServiceCollection services = new ServiceCollection();

		// Act
		services.AddAzureFileStorageService<TestFileStorage>("fake", "fake");

		var provider = services.BuildServiceProvider();
		var service = provider.GetService<IFileStorageService<TestFileStorage>>();

		// Assert
		Assert.IsNotNull(service);
		Assert.IsInstanceOfType(service, typeof(AzureFileStorageService<TestFileStorage>));
	}

}
