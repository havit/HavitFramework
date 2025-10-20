using Havit.Services.FileStorage;
using Havit.Services.Tests.FileStorage.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Services.Tests.FileStorage;

[TestClass]
public class ServiceCollectionExtensionsTests
{
	[TestMethod]
	public void ServiceCollectionExtensions_AddFileSystemStorageService()
	{
		// Arrange
		ServiceCollection services = new ServiceCollection();

		// Act
		services.AddFileSystemStorageService<TestFileStorage>(Path.GetTempPath());
		var serviceProvider = services.BuildServiceProvider();
		var service = serviceProvider.GetService<IFileStorageService<TestFileStorage>>();

		// Assert
		Assert.IsNotNull(service);
		Assert.IsInstanceOfType(service, typeof(FileSystemStorageService<TestFileStorage>));
	}

	[TestMethod]
	public void ServiceCollectionExtensions_AddFileStorageWrappingService()
	{
		// Tento test testuje spíše než podmínky v sekci Assert to, jakým způsobem lze v aplikaci pohodlně vytvořit (nebo z důvodu zpětné kompatibility ponechat)
		// aplikační služby pro použití se file storage services.

		// Arrange
		ServiceCollection services = new ServiceCollection();
		services.AddFileStorageWrappingService<IApplicationFileStorageService, ApplicationFileStorageService, TestUnderlyingFileStorage>();
		services.AddFileSystemStorageService<TestUnderlyingFileStorage>(Path.GetTempPath());
		var serviceProvider = services.BuildServiceProvider();

		// Act
		var service = serviceProvider.GetService<IApplicationFileStorageService>();

		// Assert
		Assert.IsNotNull(service);
		Assert.IsInstanceOfType(service, typeof(ApplicationFileStorageService));
	}

}
