using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Castle.Core.Internal;
using Havit.Services.Azure.FileStorage;
using Havit.Services.FileStorage;
using Havit.Services.Tests.FileStorage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Services.Azure.Tests.FileStorage
{
	[TestClass]
	public class GetAzureBlobStorageServiceTest
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
			service.EnumerateFiles().ForEach(item => service.Delete(item.Name));
		}

		[TestMethod]
		public void AzureBlobStorageService_Exists_ReturnsFalseWhenNotFound()
		{
			FileStorageServiceTestInternals.FileStorageService_Exists_ReturnsFalseWhenNotFound(GetAzureBlobStorageService());
		}

		[TestMethod]
		public void AzureBlobStorageService_Exists_ReturnsTrueForExistingBlob()
		{
			FileStorageServiceTestInternals.FileStorageService_Exists_ReturnsTrueForExistingBlob(GetAzureBlobStorageService());
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void AzureBlobStorageService_SaveDoNotAcceptSeekedStream()
		{
			FileStorageServiceTestInternals.FileStorageService_SaveDoNotAcceptSeekedStream(GetAzureBlobStorageService());
		}

		[TestMethod]
		public void AzureBlobStorageService_SavedAndReadContentsAreSame()
		{
			FileStorageServiceTestInternals.FileStorageService_SavedAndReadContentsAreSame_Perform(GetAzureBlobStorageService());
		}

		[TestMethod]
		public void AzureBlobStorageService_Save_AcceptsPathWithNewSubfolders()
		{
			FileStorageServiceTestInternals.FileStorageService_Save_AcceptsPathWithNewSubfolders(GetAzureBlobStorageService());
		}

		[TestMethod]
		public void AzureBlobStorageService_SavedAndReadContentsWithEncryptionAreSame()
		{
			FileStorageServiceTestInternals.FileStorageService_SavedAndReadContentsAreSame_Perform(GetAzureBlobStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
		}

		[TestMethod]
		public void AzureBlobStorageService_DoesNotExistsAfterDelete()
		{
			FileStorageServiceTestInternals.FileStorageService_DoesNotExistsAfterDelete(GetAzureBlobStorageService());
		}

		[TestMethod]
		public void AzureBlobStorageService_EnumerateFiles_SupportsSearchPattern()
		{
			FileStorageServiceTestInternals.FileStorageService_EnumerateFiles_SupportsSearchPattern(GetAzureBlobStorageService());
		}

		[TestMethod]
		public void AzureBlobStorageService_EnumerateFiles_SupportsSearchPatternInSubfolder()
		{
			Assert.Inconclusive("Čeká na implementaci.");
			FileStorageServiceTestInternals.FileStorageService_EnumerateFiles_SupportsSearchPatternInSubfolder(GetAzureBlobStorageService());
		}

		[TestMethod]
		public void AzureBlobStorageService_Read_StopReadingFarBeforeEndDoesNotThrowCryptographicException()
		{
			FileStorageServiceTestInternals.FileStorageService_Read_StopReadingFarBeforeEndDoesNotThrowCryptographicException(GetAzureBlobStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
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

		private static AzureBlobStorageService GetAzureBlobStorageService(string container = "tests", EncryptionOptions encryptionOptions = null)
		{
			return new AzureBlobStorageService("DefaultEndpointsProtocol=https;AccountName=hfwtestsstorage;AccountKey=3yuNhy/gYB6JDZ+bljB+vNBs4DrjjgvK7ZFfCR2QrZWoy4dEuYuSAApkQ2GkmKb01U2bidXq5/SpNDFm8uflDw==;", container, (EncryptionOptions)encryptionOptions);
		}
	}
}
