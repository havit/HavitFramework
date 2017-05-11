using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Havit.Services.FileStorage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileInfo = Havit.Services.FileStorage.FileInfo;

namespace Havit.Services.Tests.FileStorage
{
	[TestClass]
	public class FileSystemStorageServiceTest
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
			FileStorageServiceTestInternals.FileStorageService_Exists_ReturnsFalseWhenNotFound(GetFileSystemStorageService());
		}

		[TestMethod]
		public async Task FileSystemStorageService_ExistsAsync_ReturnsFalseWhenNotFound()
		{
			await FileStorageServiceTestInternals.FileStorageService_ExistsAsync_ReturnsFalseWhenNotFound(GetFileSystemStorageService());
		}

		[TestMethod]
		public void FileSystemStorageService_Exists_ReturnsTrueForExistingBlob()
		{
			FileStorageServiceTestInternals.FileStorageService_Exists_ReturnsTrueForExistingBlob(GetFileSystemStorageService());
		}

		[TestMethod]
		public async Task FileSystemStorageService_ExistsAsync_ReturnsTrueForExistingBlob()
		{
			await FileStorageServiceTestInternals.FileStorageService_ExistsAsync_ReturnsTrueForExistingBlob(GetFileSystemStorageService());
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void FileSystemStorageService_SaveDoesNotAcceptSeekedStream()
		{
			FileStorageServiceTestInternals.FileStorageService_SaveDoNotAcceptSeekedStream(GetFileSystemStorageService());
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public async Task FileSystemStorageService_SaveAsyncDoesNotAcceptSeekedStream()
		{
			await FileStorageServiceTestInternals.FileStorageService_SaveAsyncDoNotAcceptSeekedStream(GetFileSystemStorageService());
		}

		[TestMethod]
		public void FileSystemStorageService_SavedAndReadContentsAreSame()
		{
			FileStorageServiceTestInternals.FileStorageService_SavedAndReadContentsAreSame_Perform(GetFileSystemStorageService());
		}

		[TestMethod]
		public async Task FileSystemStorageService_SavedAndReadContentsAreSame_Async()
		{
			await FileStorageServiceTestInternals.FileStorageService_SavedAndReadContentsAreSame_PerformAsync(GetFileSystemStorageService());
		}

		[TestMethod]
		public void FileSystemStorageService_Save_AcceptsPathWithNewSubfolders()
		{
			FileStorageServiceTestInternals.FileStorageService_Save_AcceptsPathWithNewSubfolders(GetFileSystemStorageService());
		}

		[TestMethod]
		public async Task FileSystemStorageService_SaveAsync_AcceptsPathWithNewSubfolders()
		{
			await FileStorageServiceTestInternals.FileStorageService_SaveAsync_AcceptsPathWithNewSubfolders(GetFileSystemStorageService());
		}

		[TestMethod]
		public void FileSystemStorageService_SavedAndReadContentsWithEncryptionAreSame()
		{
			FileStorageServiceTestInternals.FileStorageService_SavedAndReadContentsAreSame_Perform(GetFileSystemStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
		}

		[TestMethod]
		public async Task FileSystemStorageService_SavedAndReadContentsWithEncryptionAreSame_Async()
		{
			await FileStorageServiceTestInternals.FileStorageService_SavedAndReadContentsAreSame_PerformAsync(GetFileSystemStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
		}

		[TestMethod]
		public void FileSystemStorageService_DoesNotExistsAfterDelete()
		{
			FileStorageServiceTestInternals.FileStorageService_DoesNotExistsAfterDelete(GetFileSystemStorageService());
		}

		[TestMethod]
		public async Task FileSystemStorageService_DoesNotExistsAfterDeleteAsync()
		{
			await FileStorageServiceTestInternals.FileStorageService_DoesNotExistsAfterDeleteAsync(GetFileSystemStorageService());
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
			List<Havit.Services.FileStorage.FileInfo> fileInfos = (await fileSystemStorageService.EnumerateFilesAsync()).ToList();

			// Assert 
			Assert.IsFalse(fileInfos.Any(fileInfo => fileInfo.Name.Contains(storagePath)));

			// Clean-up
			await fileSystemStorageService.DeleteAsync(testFilename);
		}

		[TestMethod]
		public void FileSystemStorageService_EnumerateFiles_SupportsSearchPattern()
		{
			FileStorageServiceTestInternals.FileStorageService_EnumerateFiles_SupportsSearchPattern(GetFileSystemStorageService());
		}

		[TestMethod]
		public async Task FileSystemStorageService_EnumerateFilesAsync_SupportsSearchPattern()
		{
			await FileStorageServiceTestInternals.FileStorageService_EnumerateFilesAsync_SupportsSearchPattern(GetFileSystemStorageService());
		}

		[TestMethod]
		public void FileSystemStorageService_EnumerateFiles_SupportsSearchPatternInSubfolder()
		{
			Assert.Inconclusive("Čeká na implementaci.");
			//FileStorageServiceTestInternals.FileStorageService_EnumerateFiles_SupportsSearchPatternInSubfolder(GetFileSystemStorageService());
		}

		[TestMethod]
		public void FileSystemStorageService_EnumerateFilesAsync_SupportsSearchPatternInSubfolder()
		{
			Assert.Inconclusive("Čeká na implementaci.");
		}

		[TestMethod]
		public void FileSystemStorageService_EnumerateFiles_HasLastModifiedUtcAndSize()
		{
			FileStorageServiceTestInternals.FileStorageService_EnumerateFiles_HasLastModifiedUtcAndSize(GetFileSystemStorageService());
		}

		[TestMethod]
		public async Task FileSystemStorageService_EnumerateFilesAsync_HasLastModifiedUtcAndSize()
		{
			await FileStorageServiceTestInternals.FileStorageService_EnumerateFilesAsync_HasLastModifiedUtcAndSize(GetFileSystemStorageService());
		}

		[TestMethod]
		public void FileSystemStorageService_Read_StopReadingFarBeforeEndDoesNotThrowCryptographicException()
		{
			FileStorageServiceTestInternals.FileStorageService_Read_StopReadingFarBeforeEndDoesNotThrowCryptographicException(GetFileSystemStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
		}

		[TestMethod]
		public async Task FileSystemStorageService_ReadAsync_StopReadingFarBeforeEndDoesNotThrowCryptographicException()
		{
			await FileStorageServiceTestInternals.FileStorageService_ReadAsync_StopReadingFarBeforeEndDoesNotThrowCryptographicException(GetFileSystemStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
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

			// Assert by methot attribute
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
