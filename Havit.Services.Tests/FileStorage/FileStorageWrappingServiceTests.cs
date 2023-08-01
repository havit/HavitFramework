using Havit.Services.FileStorage;
using Havit.Services.Tests.FileStorage.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Services.Tests.FileStorage;

[TestClass]
public class FileStorageWrappingServiceTests
{
	[TestMethod]
	public void FileStorageWrappingService_DependencyInjectionContainerIntegration()
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
