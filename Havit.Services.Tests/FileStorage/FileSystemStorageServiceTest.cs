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
		public void FileSystemStorageService_SavedAndReadContentsWithEncryptionAreSame()
		{
			FileStorageServiceTestInternals.FileStorageService_SavedAndReadContentsAreSame_Perform(GetFileSystemStorageService(new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
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
		public void FileSystemStorageServic_Read_StopReadingFarBeforeEndDoesNotThrowCryptographicException()
		{
			FileStorageServiceTestInternals.FileStorageService_Read_StopReadingFarBeforeEndDoesNotThrowCryptographicException(GetFileSystemStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
		}

		private FileSystemStorageService GetFileSystemStorageService(EncryptionOptions encryptionOptions = null)
		{
			return new FileSystemStorageService(GetStoragePath(), encryptionOptions);
		}

		private static string GetStoragePath()
		{
			return System.IO.Path.GetTempPath();
		}
	}
}
