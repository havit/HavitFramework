using Havit.Services.FileStorage;
using Havit.Services.Tests.FileStorage.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services.Tests.FileStorage
{
	[TestClass]
	public class EmbeddedResourceStorageServiceTests
	{
		[TestMethod]
		public void EmbeddedResourceStorageService_Exists()
		{
			// Arrange
			EmbeddedResourceStorageService service = GetEmbeddedResourceStorageService();

			// Act + Assert
			Assert.IsTrue(service.Exists("file.txt"));
			Assert.IsFalse(service.Exists("some_missing_file"));
		}

		[TestMethod]
		public async Task EmbeddedResourceStorageService_ExistsAsync()
		{
			// Arrange
			EmbeddedResourceStorageService service = GetEmbeddedResourceStorageService();

			// Act + Assert
			Assert.IsTrue(await service.ExistsAsync("file.txt"));
			Assert.IsFalse(await service.ExistsAsync("some_missing_file"));
		}

		[TestMethod]
		public void EmbeddedResourceStorageService_OpenRead()
		{
			// Arrange
			EmbeddedResourceStorageService service = GetEmbeddedResourceStorageService();
			string text;

			// Act
			using (StreamReader sr = new StreamReader(service.OpenRead("file.txt")))
			{
				text = sr.ReadToEnd();
			}

			// Assert
			Assert.AreEqual("embedded resource file", text);
		}

		[TestMethod]
		[ExpectedException(typeof(FileNotFoundException))]
		public void EmbeddedResourceStorageService_OpenRead_MissingFile()
		{
			// Arrange
			EmbeddedResourceStorageService service = GetEmbeddedResourceStorageService();

			// Act
			service.OpenRead("some_missing file.txt");

			// Assert - by method attribute
		}


		[TestMethod]
		public async Task EmbeddedResourceStorageService_OpenReadAsync()
		{
			// Arrange
			EmbeddedResourceStorageService service = GetEmbeddedResourceStorageService();
			string text;

			// Act
			using (StreamReader sr = new StreamReader(await service.OpenReadAsync("file.txt")))
			{
				text = await sr.ReadToEndAsync();
			}

			// Assert
			Assert.AreEqual("embedded resource file", text);
		}

		[TestMethod]
		public void EmbeddedResourceStorageService_DependencyInjectionContainerIntegration()
		{
			ServiceCollection services = new ServiceCollection();
			services.AddEmbeddedResourcesStorageService<TestFileStorage>(typeof(EmbeddedResourceStorageServiceTests).Assembly, null);
			var provider = services.BuildServiceProvider();

			// Act
			var service = provider.GetService<IFileStorageService<TestFileStorage>>();

			// Assert
			Assert.IsNotNull(service);
			Assert.IsInstanceOfType(service, typeof(EmbeddedResourceStorageService<TestFileStorage>));
		}

		private EmbeddedResourceStorageService GetEmbeddedResourceStorageService()
		{
			return new EmbeddedResourceStorageService(typeof(EmbeddedResourceStorageServiceTests).Assembly /* Havit.Services.Tests */, "Havit.Services.Tests.FileStorage.EmbeddedResources");
		}
	}
}
