using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Havit.Services.Azure.FileStorage;
using Havit.Services.FileStorage;
using Havit.Services.Tests.FileStorage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage.Blob;
using FileInfo = Havit.Services.FileStorage.FileInfo;

namespace Havit.Services.Azure.Tests.FileStorage
{
	[TestClass]
	public class AzureBlobStorageServiceTests
	{
		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			// testy jsou slušné, mažou po sobě
			// ve scénáři, kdy testy procházejí, není nutno tedy čistit před každým testem, ale čistíme pouze preventivně před všemi testy
			CleanUp();
		}

		[ClassCleanup]
		public static void CleanUp()
		{
			var service = GetAzureBlobStorageService();
			service.EnumerateFiles().ToList().ForEach(item => service.Delete(item.Name));
		}

		[TestMethod]
		public void AzureBlobStorageService_Exists_ReturnsFalseWhenNotFound()
		{
			FileStorageServiceTestInternals.FileStorageService_Exists_ReturnsFalseWhenNotFound(GetAzureBlobStorageService());
		}

		[TestMethod]
		public async Task AzureBlobStorageService_ExistsAsync_ReturnsFalseWhenNotFound()
		{
			await FileStorageServiceTestInternals.FileStorageService_ExistsAsync_ReturnsFalseWhenNotFound(GetAzureBlobStorageService());
		}

		[TestMethod]
		public void AzureBlobStorageService_Exists_ReturnsTrueForExistingBlob()
		{
			FileStorageServiceTestInternals.FileStorageService_Exists_ReturnsTrueForExistingBlob(GetAzureBlobStorageService());
		}

		[TestMethod]
		public async Task AzureBlobStorageService_ExistsAsync_ReturnsTrueForExistingBlob()
		{
			await FileStorageServiceTestInternals.FileStorageService_ExistsAsync_ReturnsTrueForExistingBlob(GetAzureBlobStorageService());
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void AzureBlobStorageService_SaveDoNotAcceptSeekedStream()
		{
			FileStorageServiceTestInternals.FileStorageService_SaveDoNotAcceptSeekedStream(GetAzureBlobStorageService());
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public async Task AzureBlobStorageService_SaveAsyncDoesNotAcceptSeekedStream()
		{
			await FileStorageServiceTestInternals.FileStorageService_SaveAsyncDoNotAcceptSeekedStream(GetAzureBlobStorageService());
		}

		[TestMethod]
		public void AzureBlobStorageService_SavedAndReadContentsAreSame()
		{
			FileStorageServiceTestInternals.FileStorageService_SavedAndReadContentsAreSame_Perform(GetAzureBlobStorageService());
		}

		[TestMethod]
		public async Task AzureBlobStorageService_SavedAndReadContentsAreSame_Async()
		{
			await FileStorageServiceTestInternals.FileStorageService_SavedAndReadContentsAreSame_PerformAsync(GetAzureBlobStorageService());
		}

		[TestMethod]
		public void AzureBlobStorageService_Save_AcceptsPathWithNewSubfolders()
		{
			FileStorageServiceTestInternals.FileStorageService_Save_AcceptsPathWithNewSubfolders(GetAzureBlobStorageService());
		}

		[TestMethod]
		public async Task AzureBlobStorageService_SaveAsync_AcceptsPathWithNewSubfolders()
		{
			await FileStorageServiceTestInternals.FileStorageService_SaveAsync_AcceptsPathWithNewSubfolders(GetAzureBlobStorageService());
		}

		[TestMethod]
		public void AzureBlobStorageService_SavedAndReadContentsWithEncryptionAreSame()
		{
			FileStorageServiceTestInternals.FileStorageService_SavedAndReadContentsAreSame_Perform(GetAzureBlobStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
		}

		[TestMethod]
		public async Task AzureBlobStorageService_SavedAndReadContentsWithEncryptionAreSame_Async()
		{
			await FileStorageServiceTestInternals.FileStorageService_SavedAndReadContentsAreSame_PerformAsync(GetAzureBlobStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
		}

		[TestMethod]
		public void AzureBlobStorageService_DoesNotExistsAfterDelete()
		{
			FileStorageServiceTestInternals.FileStorageService_DoesNotExistsAfterDelete(GetAzureBlobStorageService());
		}

		[TestMethod]
		public async Task AzureBlobStorageService_DoesNotExistsAfterDeleteAsync()
		{
			await FileStorageServiceTestInternals.FileStorageService_DoesNotExistsAfterDeleteAsync(GetAzureBlobStorageService());
		}

		[TestMethod]
		public void AzureBlobStorageService_EnumerateFiles_SupportsSearchPattern()
		{
			FileStorageServiceTestInternals.FileStorageService_EnumerateFiles_SupportsSearchPattern(GetAzureBlobStorageService());
		}

		[TestMethod]
		public async Task AzureBlobStorageService_EnumerateFilesAsync_SupportsSearchPattern()
		{
			await FileStorageServiceTestInternals.FileStorageService_EnumerateFilesAsync_SupportsSearchPattern(GetAzureBlobStorageService());
		}

		[TestMethod]
		public void AzureBlobStorageService_EnumerateFiles_SupportsSearchPatternInSubfolder()
		{			
			FileStorageServiceTestInternals.FileStorageService_EnumerateFiles_SupportsSearchPatternInSubfolder(GetAzureBlobStorageService());
		}

		[TestMethod]
		public async Task AzureBlobStorageService_EnumerateFilesAsync_SupportsSearchPatternInSubfolder()
		{
			await FileStorageServiceTestInternals.FileStorageService_EnumerateFilesAsync_SupportsSearchPatternInSubfolder(GetAzureBlobStorageService());
		}

		[TestMethod]
		public void AzureBlobStorageService_EnumerableFilesGetPrefix_CorrectlyGetPrefix()
		{
			// Arrange
			AzureBlobStorageService azureBlobStorageService = GetAzureBlobStorageService();

			// Act + Assert
			Assert.AreEqual(null, azureBlobStorageService.EnumerableFilesGetPrefix(null));
			Assert.AreEqual(null, azureBlobStorageService.EnumerableFilesGetPrefix(""));
			Assert.AreEqual(null, azureBlobStorageService.EnumerableFilesGetPrefix("test.txt"));
			Assert.AreEqual(String.Empty, azureBlobStorageService.EnumerableFilesGetPrefix("/test.*"));
			Assert.AreEqual(String.Empty, azureBlobStorageService.EnumerableFilesGetPrefix("/test.txt"));

			Assert.AreEqual("SubFolder1/Subfolder2", azureBlobStorageService.EnumerableFilesGetPrefix("SubFolder1/Subfolder2/test.txt"));
			Assert.AreEqual("SubFolder1/Subfolder2", azureBlobStorageService.EnumerableFilesGetPrefix("SubFolder1/Subfolder2/*.txt"));
			Assert.AreEqual("SubFolder1/Subfolder2", azureBlobStorageService.EnumerableFilesGetPrefix("SubFolder1/Subfolder2/t*.txt"));
			Assert.AreEqual("SubFolder1", azureBlobStorageService.EnumerableFilesGetPrefix("SubFolder1/Sub*/*.txt"));
			Assert.AreEqual(null, azureBlobStorageService.EnumerableFilesGetPrefix("Sub*/Sub*/*.txt"));
			Assert.AreEqual("Subfolder1", azureBlobStorageService.EnumerableFilesGetPrefix("Subfolder1/Sub*/test.txt"));
			Assert.AreEqual(null, azureBlobStorageService.EnumerableFilesGetPrefix("Sub*/Sub/test.txt"));
			Assert.AreEqual(String.Empty, azureBlobStorageService.EnumerableFilesGetPrefix("/Sub*/test.txt"));

			Assert.AreEqual("SubFolder1/Subfolder2", azureBlobStorageService.EnumerableFilesGetPrefix("SubFolder1/Subfolder2/??.txt"));
			Assert.AreEqual("SubFolder1/Subfolder2", azureBlobStorageService.EnumerableFilesGetPrefix("SubFolder1/Subfolder2/t??.txt"));
			Assert.AreEqual("SubFolder1", azureBlobStorageService.EnumerableFilesGetPrefix("SubFolder1/Sub??/??.txt"));
			Assert.AreEqual(null, azureBlobStorageService.EnumerableFilesGetPrefix("Sub??/Sub??/??.txt"));
			Assert.AreEqual("Subfolder1", azureBlobStorageService.EnumerableFilesGetPrefix("Subfolder1/Sub??/test.txt"));
			Assert.AreEqual(null, azureBlobStorageService.EnumerableFilesGetPrefix("Sub??/Sub/test.txt"));
		}

		[TestMethod]
		public void AzureBlobStorageService_EnumerateFiles_HasLastModifiedUtcAndSize()
		{
			FileStorageServiceTestInternals.FileStorageService_EnumerateFiles_HasLastModifiedUtcAndSize(GetAzureBlobStorageService());
		}

		[TestMethod]
		public async Task AzureBlobStorageService_EnumerateFilesAsync_HasLastModifiedUtcAndSize()
		{
			await FileStorageServiceTestInternals.FileStorageService_EnumerateFilesAsync_HasLastModifiedUtcAndSize(GetAzureBlobStorageService());
		}

		[TestMethod]
		public void AzureBlobStorageService_Read_StopReadingFarBeforeEndDoesNotThrowCryptographicException()
		{
			FileStorageServiceTestInternals.FileStorageService_Read_StopReadingFarBeforeEndDoesNotThrowCryptographicException(GetAzureBlobStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
		}

		[TestMethod]
		public async Task AzureBlobStorageService_ReadAsync_StopReadingFarBeforeEndDoesNotThrowCryptographicException()
		{
			await FileStorageServiceTestInternals.FileStorageService_ReadAsync_StopReadingFarBeforeEndDoesNotThrowCryptographicException(GetAzureBlobStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
		}

		[TestMethod]
		public void AzureBlobStorageService_EncryptAndDecryptAllFiles()
		{
			// Arrange
			string content = "abcdefghijklmnopqrśtuvwxyz\r\n12346790\t+ěščřžýáíé";
			string testFilename = "encryption.txt";

			var plainStorageService = GetAzureBlobStorageService();
			var encryptedStorageService = GetAzureBlobStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String()));

			EncryptDecryptFileStorageService encryptDecryptFileStorageService = new EncryptDecryptFileStorageService();

			// Act
			// zapíšeme nešifrovaný soubor
			using (MemoryStream ms = new MemoryStream())
			{
				using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8, 1024, true))
				{
					sw.Write(content);
				}

				ms.Seek(0, SeekOrigin.Begin);
				plainStorageService.Save(testFilename, ms, "text/plain");
			}

			// zašifrujeme všechny soubory
			encryptDecryptFileStorageService.EncryptAllFiles(plainStorageService, encryptedStorageService);

			// kontrola, zda je soubor zašifrovaný
			using (MemoryStream ms = new MemoryStream())
			{
				encryptedStorageService.ReadToStream(testFilename, ms); 
				ms.Seek(0, SeekOrigin.Begin);

				using (StreamReader sr = new StreamReader(ms, Encoding.UTF8, false, 1024, true))
				{
					string readContent = sr.ReadToEnd();

					Assert.AreEqual(content, readContent);
				}
			}

			// dešifrujeme všechny soubory
			encryptDecryptFileStorageService.DecryptAllFiles(encryptedStorageService, plainStorageService);

			// kontrola, zda je soubor dešifrovaný
			using (MemoryStream ms = new MemoryStream())
			{
				plainStorageService.ReadToStream(testFilename, ms); // čteme z plainStorageService!
				ms.Seek(0, SeekOrigin.Begin);

				using (StreamReader sr = new StreamReader(ms, Encoding.UTF8, false, 1024, true))
				{
					string readContent = sr.ReadToEnd();

					Assert.AreEqual(content, readContent);
				}
			}
		}

		[TestMethod]
		public void AzureBlobStorageService_OptionsCacheControlSavesCacheControl()
		{
			// Arrange
			string cacheControlValue = "public, maxage=3600";
			AzureBlobStorageService service = GetAzureBlobStorageService(options: new AzureBlobStorageServiceOptions { CacheControl = cacheControlValue });

			// Act
			using (MemoryStream ms = new MemoryStream())
			{
				service.Save("test.txt", ms, "text/plain"); // uložíme prázdný soubor
			}

			// Assert
			CloudBlockBlob cloudBlockBlob = service.GetBlobReference("test.txt");
			cloudBlockBlob.FetchAttributes();
			Assert.AreEqual(cacheControlValue, cloudBlockBlob.Properties.CacheControl);
		}

		private static AzureBlobStorageService GetAzureBlobStorageService(string container = "tests", AzureBlobStorageServiceOptions options = null, EncryptionOptions encryptionOptions = null)
		{
			// we do not want to leak our Azure Storage connection string + we need to have it accessible for build + all HAVIT developers as easy as possible
			// use your own Azure Storage account if you do not have access to this file
			return new AzureBlobStorageService(File.ReadAllText(@"\\topol.havit.local\Workspace\002.HFW\Havit.Services.Azure.Tests.HfwTestsStorage.connectionString.txt"), container, options, (EncryptionOptions)encryptionOptions);
		}
	}
}
