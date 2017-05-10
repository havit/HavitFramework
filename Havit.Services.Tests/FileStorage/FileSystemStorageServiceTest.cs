using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Havit.Services.FileStorage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
		public void FileSystemStorageService_Exists_ReturnsTrueForExistingBlob()
		{
			FileStorageServiceTestInternals.FileStorageService_Exists_ReturnsTrueForExistingBlob(GetFileSystemStorageService());
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void FileSystemStorageService_SaveDoNotAcceptSeekedStream()
		{
			FileStorageServiceTestInternals.FileStorageService_SaveDoNotAcceptSeekedStream(GetFileSystemStorageService());
		}

		[TestMethod]
		public void FileSystemStorageService_SavedAndReadContentsAreSame()
		{
			FileStorageServiceTestInternals.FileStorageService_SavedAndReadContentsAreSame_Perform(GetFileSystemStorageService());
		}

		[TestMethod]
		public void FileSystemStorageService_Save_AcceptsPathWithNewSubfolders()
		{
			FileStorageServiceTestInternals.FileStorageService_Save_AcceptsPathWithNewSubfolders(GetFileSystemStorageService());
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void FileSystemStorageService_BadDirectoryTraversal_ThrowsInvalidOperationException()
		{
			// Arrange
			string storagePath = @"C:\A\";
			// file outside base storagePath (directory traversal attack)
			string testFilename = @"\..\AB\test.txt";

			FileSystemStorageService fileSystemStorageService = new FileSystemStorageService(storagePath);
			fileSystemStorageService.Exists(testFilename);
		}

		[TestMethod]
		public void FileSystemStorageService_SavedAndReadContentsWithEncryptionAreSame()
		{
			FileStorageServiceTestInternals.FileStorageService_SavedAndReadContentsAreSame_Perform(GetFileSystemStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
		}

		[TestMethod]
		public void FileSystemStorageService_DoesNotExistsAfterDelete()
		{
			FileStorageServiceTestInternals.FileStorageService_DoesNotExistsAfterDelete(GetFileSystemStorageService());
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
		public void FileSystemStorageService_EnumerateFiles_SupportsSearchPattern()
		{
			FileStorageServiceTestInternals.FileStorageService_EnumerateFiles_SupportsSearchPattern(GetFileSystemStorageService());
		}

		[TestMethod]
		public void FileSystemStorageService_EnumerateFiles_SupportsSearchPatternInSubfolder()
		{
			Assert.Inconclusive("Čeká na implementaci.");
			FileStorageServiceTestInternals.FileStorageService_EnumerateFiles_SupportsSearchPatternInSubfolder(GetFileSystemStorageService());
		}

		[TestMethod]
		public void FileSystemStorageService_Read_StopReadingFarBeforeEndDoesNotThrowCryptographicException()
		{
			FileStorageServiceTestInternals.FileStorageService_Read_StopReadingFarBeforeEndDoesNotThrowCryptographicException(GetFileSystemStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
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
