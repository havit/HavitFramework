using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Havit.Services.FileStorage;
using Havit.Services.TestHelpers;
using Havit.Services.TestHelpers.FileStorage;
using Havit.Services.Tests.FileStorage.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileInfo = Havit.Services.FileStorage.FileInfo;

namespace Havit.Services.Tests.FileStorage
{
	[TestClass]
	public class FileSystemStorageServiceTests
	{
		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			// testy jsou slušné, mažou po sobě
			// ve scénáři, kdy testy procházejí, není nutno tedy čistit před každým testem, ale čistíme pouze preventivně před všemi testy
			CleanUp();
			Directory.CreateDirectory(GetStoragePath());
		}

		[ClassCleanup]
		public static void CleanUp()
		{
			string path = GetStoragePath();
			if (Directory.Exists(path))
			{
				Directory.Delete(path, true);
			}
		}

		[TestMethod]
		public void FileSystemStorageService_Exists_ReturnsFalseWhenNotFound()
		{
			FileStorageServiceTestHelpers.FileStorageService_Exists_ReturnsFalseWhenNotFound(GetFileSystemStorageService());
		}

		[TestMethod]
		public async Task FileSystemStorageService_ExistsAsync_ReturnsFalseWhenNotFound()
		{
			await FileStorageServiceTestHelpers.FileStorageService_ExistsAsync_ReturnsFalseWhenNotFound(GetFileSystemStorageService());
		}

		[TestMethod]
		public void FileSystemStorageService_Exists_ReturnsTrueWhenExists()
		{
			FileStorageServiceTestHelpers.FileStorageService_Exists_ReturnsTrueWhenExists(GetFileSystemStorageService());
		}

		[TestMethod]
		public async Task FileSystemStorageService_ExistsAsync_ReturnsTrueWhenExists()
		{
			await FileStorageServiceTestHelpers.FileStorageService_ExistsAsync_ReturnsTrueWhenExists(GetFileSystemStorageService());
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void FileSystemStorageService_SaveDoesNotAcceptSeekedStream()
		{
			FileStorageServiceTestHelpers.FileStorageService_SaveDoNotAcceptSeekedStream(GetFileSystemStorageService());
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public async Task FileSystemStorageService_SaveAsyncDoesNotAcceptSeekedStream()
		{
			await FileStorageServiceTestHelpers.FileStorageService_SaveAsyncDoNotAcceptSeekedStream(GetFileSystemStorageService());
		}

		[TestMethod]
		public void FileSystemStorageService_SavedAndReadContentsAreSame()
		{
			FileStorageServiceTestHelpers.FileStorageService_SavedAndReadContentsAreSame_Perform(GetFileSystemStorageService());
		}

		[TestMethod]
		public async Task FileSystemStorageService_SavedAndReadContentsAreSame_Async()
		{
			await FileStorageServiceTestHelpers.FileStorageService_SavedAndReadContentsAreSame_PerformAsync(GetFileSystemStorageService());
		}

		[TestMethod]
		public void FileSystemStorageService_Save_AcceptsPathWithNewSubfolders()
		{
			FileStorageServiceTestHelpers.FileStorageService_Save_AcceptsPathWithNewSubfolders(GetFileSystemStorageService());
		}

		[TestMethod]
		public async Task FileSystemStorageService_SaveAsync_AcceptsPathWithNewSubfolders()
		{
			await FileStorageServiceTestHelpers.FileStorageService_SaveAsync_AcceptsPathWithNewSubfolders(GetFileSystemStorageService());
		}

		[TestMethod]
		public void FileSystemStorageService_SavedAndReadContentsWithEncryptionAreSame()
		{
			FileStorageServiceTestHelpers.FileStorageService_SavedAndReadContentsAreSame_Perform(GetFileSystemStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
		}

		[TestMethod]
		public async Task FileSystemStorageService_SavedAndReadContentsWithEncryptionAreSame_Async()
		{
			await FileStorageServiceTestHelpers.FileStorageService_SavedAndReadContentsAreSame_PerformAsync(GetFileSystemStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
		}

		[TestMethod]
		public void FileSystemStorageService_DoesNotExistsAfterDelete()
		{
			FileStorageServiceTestHelpers.FileStorageService_DoesNotExistsAfterDelete(GetFileSystemStorageService());
		}

		[TestMethod]
		public async Task FileSystemStorageService_DoesNotExistsAfterDeleteAsync()
		{
			await FileStorageServiceTestHelpers.FileStorageService_DoesNotExistsAfterDeleteAsync(GetFileSystemStorageService());
		}

		[TestMethod]
		public void FileSystemStorageService_EnumerateFiles_DoesNotContainStoragePath()
		{
			// Arrange
			string storagePath = GetStoragePath();
			FileSystemStorageService fileSystemStorageService = GetFileSystemStorageService();

			string testFilename = "test.txt";
			using (MemoryStream ms = new MemoryStream())
			{
				fileSystemStorageService.Save(testFilename, ms, "text/plain");
			}

			// Act
			List<Havit.Services.FileStorage.FileInfo> fileInfos = fileSystemStorageService.EnumerateFiles().ToList();

			// Assert 
			Assert.IsFalse(fileInfos.Any(fileInfo => fileInfo.Name.Contains(storagePath)));

			// Clean-up
			fileSystemStorageService.Delete(testFilename);
		}

		[TestMethod]
		public async Task FileSystemStorageService_EnumerateFilesAsync_DoesNotContainStoragePath()
		{
			// Arrange
			string storagePath = GetStoragePath();
			FileSystemStorageService fileSystemStorageService = GetFileSystemStorageService();

			string testFilename = "test.txt";
			using (MemoryStream ms = new MemoryStream())
			{
				await fileSystemStorageService.SaveAsync(testFilename, ms, "text/plain");
			}

			// Act
			List<Havit.Services.FileStorage.FileInfo> fileInfos = await fileSystemStorageService.EnumerateFilesAsync().ToListAsync();

			// Assert 
			Assert.IsFalse(fileInfos.Any(fileInfo => fileInfo.Name.Contains(storagePath)));

			// Clean-up
			await fileSystemStorageService.DeleteAsync(testFilename);
		}

		[TestMethod]
		public void FileSystemStorageService_EnumerateFiles_SupportsSearchPattern()
		{
			FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_SupportsSearchPattern(GetFileSystemStorageService());
		}

		[TestMethod]
		public async Task FileSystemStorageService_EnumerateFilesAsync_SupportsSearchPattern()
		{
			await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_SupportsSearchPattern(GetFileSystemStorageService());
		}

		[TestMethod]
		public void FileSystemStorageService_EnumerateFiles_SupportsSearchPatternInSubfolder()
		{
			FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_SupportsSearchPatternInSubfolder(GetFileSystemStorageService());
		}

		[TestMethod]
		public async Task FileSystemStorageService_EnumerateFilesAsync_SupportsSearchPatternInSubfolder()
		{
			await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_SupportsSearchPatternInSubfolder(GetFileSystemStorageService());
		}

		[TestMethod]
		public void FileSystemStorageService_EnumerateFiles_HasLastModifiedUtc()
		{
			FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_HasLastModifiedUtc(GetFileSystemStorageService());
		}

		[TestMethod]
		public void FileSystemStorageService_EnumerateFiles_HasSize()
		{
			FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_HasSize(GetFileSystemStorageService());
		}

		[TestMethod]
		public async Task FileSystemStorageService_EnumerateFilesAsync_HasLastModifiedUtc()
		{
			await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_HasLastModifiedUtc(GetFileSystemStorageService());
		}
		[TestMethod]
		public async Task FileSystemStorageService_EnumerateFilesAsync_HasSize()
		{
			await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_HasSize(GetFileSystemStorageService());
		}

		[TestMethod]
		public void FileSystemStorageService_Read_StopReadingFarBeforeEndDoesNotThrowCryptographicException()
		{
			FileStorageServiceTestHelpers.FileStorageService_Read_StopReadingFarBeforeEndDoesNotThrowCryptographicException(GetFileSystemStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
		}

		[TestMethod]
		public async Task FileSystemStorageService_ReadAsync_StopReadingFarBeforeEndDoesNotThrowCryptographicException()
		{
			await FileStorageServiceTestHelpers.FileStorageService_ReadAsync_StopReadingFarBeforeEndDoesNotThrowCryptographicException(GetFileSystemStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
		}

		[TestMethod]
		public void FileSystemStorageService_GetFullPath_DoesNotThrowExceptionForCorrectPaths()
		{
			// Arrange

			// Act
			FileSystemStorageService fileSystemStorageService = new FileSystemStorageService(@"C:\");
			fileSystemStorageService.GetFullPath(@"abc.txt");
			fileSystemStorageService.GetFullPath(@"A\abc.txt");

			// Assert - no exception was thrown
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void FileSystemStorageService_GetFullPath_ThrowsExceptionForDirectoryTraversal()
		{
			// Arrange

			// Act
			FileSystemStorageService fileSystemStorageService = new FileSystemStorageService(@"C:\A");
			fileSystemStorageService.GetFullPath(@"..\AB\file.txt"); //--> C:\AB\file.txt

			// Assert by method attribute
		}

		[TestMethod]
		public void FileSystemStorageService_DependencyInjectionContainerIntegration()
		{
			// Arrange
			ServiceCollection services = new ServiceCollection();
			services.AddFileSystemStorageService<TestFileStorage>(System.IO.Path.GetTempPath());
			var provider = services.BuildServiceProvider();
			
			// Act
			var service = provider.GetService<IFileStorageService<TestFileStorage>>();

			// Assert
			Assert.IsNotNull(service);
			Assert.IsInstanceOfType(service, typeof(FileSystemStorageService<TestFileStorage>));
		}

		private static FileSystemStorageService GetFileSystemStorageService(EncryptionOptions encryptionOptions = null)
		{
			return new FileSystemStorageService(GetStoragePath(), encryptionOptions);
		}

		private static string GetStoragePath()
		{
			return Path.Combine(System.IO.Path.GetTempPath(), "hfwtests");
		}
	}
}
