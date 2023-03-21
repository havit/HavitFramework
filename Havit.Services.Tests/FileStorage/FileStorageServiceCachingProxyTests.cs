using Havit.Services.Caching;
using Havit.Services.FileStorage;
using Havit.Services.TestHelpers.Caching;
using Havit.Services.Tests.FileStorage.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Havit.Services.Tests.FileStorage
{
	[TestClass]
	public class FileStorageServiceCachingProxyTests
	{
		[TestMethod]
		public void FileStorageServiceCachingProxy_Exists_UsesCache()
		{
			// Arrange
			Mock<IFileStorageService> fileStorageServiceMock = new Mock<IFileStorageService>(MockBehavior.Strict);
			fileStorageServiceMock.Setup(m => m.Exists(It.IsAny<string>())).Returns(false);
			var cachingServiceProxy = new FileStorageServiceCachingProxy(fileStorageServiceMock.Object, new DictionaryCacheService());

			// Act
			cachingServiceProxy.Exists("abc.txt");
			cachingServiceProxy.Exists("abc.txt");
			cachingServiceProxy.Exists("abc.txt");

			// Assert
			fileStorageServiceMock.Verify(m => m.Exists("abc.txt"), Times.Once); // třikrát voláme Exists, ale druhé a třetí volání má být odbaveno z cache			
		}

		[TestMethod]
		public void FileStorageServiceCachingProxy_GetLastModifiedTimeUtc_UsesCache()
		{
			// Arrange
			Mock<IFileStorageService> fileStorageServiceMock = new Mock<IFileStorageService>(MockBehavior.Strict);
			fileStorageServiceMock.Setup(m => m.GetLastModifiedTimeUtc(It.IsAny<string>())).Returns(new DateTime(2019, 1, 1));
			var cachingServiceProxy = new FileStorageServiceCachingProxy(fileStorageServiceMock.Object, new DictionaryCacheService());

			// Act
			cachingServiceProxy.GetLastModifiedTimeUtc("abc.txt");
			cachingServiceProxy.GetLastModifiedTimeUtc("abc.txt");
			cachingServiceProxy.GetLastModifiedTimeUtc("abc.txt");

			// Assert
			fileStorageServiceMock.Verify(m => m.GetLastModifiedTimeUtc("abc.txt"), Times.Once); // třikrát voláme GetLastModifiedTimeUtc, ale druhé a třetí volání má být odbaveno z cache			
		}

		[TestMethod]
		public void FileStorageServiceCachingProxy_OpenRead_UsesCache()
		{
			// Arrange
			Mock<IFileStorageService> fileStorageServiceMock = new Mock<IFileStorageService>(MockBehavior.Strict);
			fileStorageServiceMock.Setup(m => m.OpenRead(It.IsAny<string>())).Returns(new MemoryStream());
			var cachingServiceProxy = new FileStorageServiceCachingProxy(fileStorageServiceMock.Object, new DictionaryCacheService());

			// Act
			cachingServiceProxy.OpenRead("abc.txt");
			cachingServiceProxy.OpenRead("abc.txt");
			cachingServiceProxy.OpenRead("abc.txt");

			// Assert
			fileStorageServiceMock.Verify(m => m.OpenRead("abc.txt"), Times.Once); // třikrát voláme Read, ale druhé a třetí volání má být odbaveno z cache			
		}

		[TestMethod]
		public void FileStorageServiceCachingProxy_Save_InvalidatesCache()
		{
			// Arrange
			Mock<IFileStorageService> fileStorageServiceMock = new Mock<IFileStorageService>(MockBehavior.Strict);
			fileStorageServiceMock.Setup(m => m.Save(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>()));

			Mock<ICacheService> cacheServiceMock = new Mock<ICacheService>(MockBehavior.Strict);
			cacheServiceMock.Setup(m => m.Remove(It.IsAny<string>()));

			var cachingServiceProxy = new FileStorageServiceCachingProxy(fileStorageServiceMock.Object, cacheServiceMock.Object);

			// Act
			cachingServiceProxy.Save("abc.txt", new MemoryStream(), "");

			// Assert
			cacheServiceMock.Verify(m => m.Remove(It.IsAny<string>()), Times.AtLeastOnce);
		}

		[TestMethod]
		public void FileStorageServiceCachingProxy_Delete_InvalidatesCache()
		{
			// Arrange
			Mock<IFileStorageService> fileStorageServiceMock = new Mock<IFileStorageService>(MockBehavior.Strict);
			fileStorageServiceMock.Setup(m => m.Delete(It.IsAny<string>()));

			Mock<ICacheService> cacheServiceMock = new Mock<ICacheService>(MockBehavior.Strict);
			cacheServiceMock.Setup(m => m.Remove(It.IsAny<string>()));

			var cachingServiceProxy = new FileStorageServiceCachingProxy(fileStorageServiceMock.Object, cacheServiceMock.Object);

			// Act
			cachingServiceProxy.Delete("abc.txt");

			// Assert
			cacheServiceMock.Verify(m => m.Remove(It.IsAny<string>()), Times.AtLeastOnce);
		}

		[TestMethod]
		public void FileStorageServiceCachingProxy_Delete_InvalidatesCache2()
		{
			// Arrange
			Mock<IFileStorageService> fileStorageServiceMock = new Mock<IFileStorageService>(MockBehavior.Strict);
			fileStorageServiceMock.Setup(m => m.Exists(It.IsAny<string>())).Returns(false);
			fileStorageServiceMock.Setup(m => m.Delete(It.IsAny<string>()));
			var cachingServiceProxy = new FileStorageServiceCachingProxy(fileStorageServiceMock.Object, new DictionaryCacheService());

			// Act
			cachingServiceProxy.Exists("abc.txt");
			cachingServiceProxy.Delete("abc.txt");
			cachingServiceProxy.Exists("abc.txt");

			// Assert
			fileStorageServiceMock.Verify(m => m.Exists("abc.txt"), Times.Exactly(2)); // dvakrát voláme Exists, ale mezi ním delete, což invaliduje záznam v cache, proto druhý Exists nemá hodnotu v cache
		}

		[TestMethod]
		public void FileStorageServiceCachingProxy_Save_InvalidatesCache2()
		{
			// Arrange
			Mock<IFileStorageService> fileStorageServiceMock = new Mock<IFileStorageService>(MockBehavior.Strict);
			fileStorageServiceMock.Setup(m => m.Exists(It.IsAny<string>())).Returns(false);
			fileStorageServiceMock.Setup(m => m.Save(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>()));
			var cachingServiceProxy = new FileStorageServiceCachingProxy(fileStorageServiceMock.Object, new DictionaryCacheService());

			// Act
			cachingServiceProxy.Exists("abc.txt");
			cachingServiceProxy.Save("abc.txt", new MemoryStream(), "");
			cachingServiceProxy.Exists("abc.txt");

			// Assert
			fileStorageServiceMock.Verify(m => m.Exists("abc.txt"), Times.Exactly(2)); // dvakrát voláme Exists, ale mezi ním delete, což invaliduje záznam v cache, proto druhý Exists nemá hodnotu v cache
		}

		[TestMethod]
		public void FileStorageServiceCachingProxy_Enumerate_UsesWrappedStorageService()
		{
			// Arrange
			Mock<IFileStorageService> fileStorageServiceMock = new Mock<IFileStorageService>(MockBehavior.Strict);

			fileStorageServiceMock.Setup(m => m.EnumerateFiles(It.IsAny<string>())).Returns(new List<Services.FileStorage.FileInfo>());
			var cachingServiceProxy = new FileStorageServiceCachingProxy(fileStorageServiceMock.Object, new DictionaryCacheService());

			// Act
			cachingServiceProxy.EnumerateFiles().ToList();

			// Assert	
			fileStorageServiceMock.Verify(m => m.EnumerateFiles(null), Times.Once);
		}

		[TestMethod]
		public async Task FileStorageServiceCachingProxy_EnumerateAsync_UsesWrappedStorageService()
		{
			// Arrange
			Mock<IFileStorageService> fileStorageServiceMock = new Mock<IFileStorageService>(MockBehavior.Strict);

			async IAsyncEnumerable<Services.FileStorage.FileInfo> GetMockFileInfos()
			{
				yield return new Services.FileStorage.FileInfo();
				await Task.CompletedTask; // to make the compiler warning go away
			}

			fileStorageServiceMock.Setup(m => m.EnumerateFilesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(GetMockFileInfos());
			var cachingServiceProxy = new FileStorageServiceCachingProxy(fileStorageServiceMock.Object, new DictionaryCacheService());

			// Act
			await foreach (var file in cachingServiceProxy.EnumerateFilesAsync(null))
			{
				// NOOP
			}

			// Assert	
			fileStorageServiceMock.Verify(m => m.EnumerateFilesAsync(null, It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public void FileSystemStorageService_DependencyInjectionContainerIntegration()
		{
			// Arrange
			ServiceCollection services = new ServiceCollection();
			services.AddFileStorageCachingProxy<TestFileStorage, TestUnderlyingFileStorage>();
			services.AddFileSystemStorageService<TestUnderlyingFileStorage>(System.IO.Path.GetTempPath());
			services.AddSingleton<ICacheService, MemoryCacheService>();
			services.AddMemoryCache();
			var provider = services.BuildServiceProvider();

			// Act
			var service = provider.GetService<IFileStorageService<TestFileStorage>>();

			// Assert
			Assert.IsNotNull(service);
			Assert.IsInstanceOfType(service, typeof(FileStorageServiceCachingProxy<TestFileStorage, TestUnderlyingFileStorage>));
		}
	}
}
