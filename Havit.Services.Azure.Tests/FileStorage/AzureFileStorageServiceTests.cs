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
using Havit.Services.TestHelpers.FileStorage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Services.Azure.Tests.FileStorage
{
	[TestClass]
	public class AzureFileStorageServiceTests
	{
		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			// testy jsou slušné, mažou po sobě
			// ve scénáři, kdy testy procházejí, není nutno tedy čistit před každým testem, ale čistíme pouze preventivně před všemi testy

			// nemůžeme smazat celý FileShare, protože pokud metoda je ukončena před skutečným smazáním. Další operace nad FileSharem,
			// dokud je mazán, oznamují chybu 409 Confict s popisem "The specified share is being deleted."
			// Proto raději smažeme jen soubory.
			var service = GetAzureFileStorageService();
			service.EnumerateFiles().ToList().ForEach(item => service.Delete(item.Name));
		}

		[ClassCleanup]
		public static void CleanUp()
		{
#if !DEBUG
			GetAzureFileStorageService().GetShareClient().DeleteIfExists();
#endif
		}

		[TestMethod]
		public void AzureFileStorageService_Exists_ReturnsFalseWhenNotFound()
		{
			FileStorageServiceTestHelpers.FileStorageService_Exists_ReturnsFalseWhenNotFound(GetAzureFileStorageService());
		}

		[TestMethod]
		public async Task AzureFileStorageService_ExistsAsync_ReturnsFalseWhenNotFound()
		{
			await FileStorageServiceTestHelpers.FileStorageService_ExistsAsync_ReturnsFalseWhenNotFound(GetAzureFileStorageService());
		}

		[TestMethod]
		public void AzureFileStorageService_Exists_ReturnsTrueWhenExists()
		{
			FileStorageServiceTestHelpers.FileStorageService_Exists_ReturnsTrueWhenExists(GetAzureFileStorageService());
		}

		[TestMethod]
		public async Task AzureFileStorageService_ExistsAsync_ReturnsTrueWhenExists()
		{
			await FileStorageServiceTestHelpers.FileStorageService_ExistsAsync_ReturnsTrueWhenExists(GetAzureFileStorageService());
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void AzureFileStorageService_SaveDoNotAcceptSeekedStream()
		{
			FileStorageServiceTestHelpers.FileStorageService_SaveDoNotAcceptSeekedStream(GetAzureFileStorageService());
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public async Task AzureFileStorageService_SaveAsyncDoesNotAcceptSeekedStream()
		{
			await FileStorageServiceTestHelpers.FileStorageService_SaveAsyncDoNotAcceptSeekedStream(GetAzureFileStorageService());
		}

		[TestMethod]
		public void AzureFileStorageService_SavedAndReadContentsAreSame()
		{
			FileStorageServiceTestHelpers.FileStorageService_SavedAndReadContentsAreSame_Perform(GetAzureFileStorageService());
		}

		[TestMethod]
		public async Task AzureFileStorageService_SavedAndReadContentsAreSame_Async()
		{
			await FileStorageServiceTestHelpers.FileStorageService_SavedAndReadContentsAreSame_PerformAsync(GetAzureFileStorageService());
		}

		[TestMethod]
		public void AzureFileStorageService_Save_AcceptsPathWithNewSubfolders()
		{
			FileStorageServiceTestHelpers.FileStorageService_Save_AcceptsPathWithNewSubfolders(GetAzureFileStorageService());
		}

		[TestMethod]
		public async Task AzureFileStorageService_SaveAsync_AcceptsPathWithNewSubfolders()
		{
			await FileStorageServiceTestHelpers.FileStorageService_SaveAsync_AcceptsPathWithNewSubfolders(GetAzureFileStorageService());
		}

		//[TestMethod]
		public void AzureFileStorageService_SavedAndReadContentsWithEncryptionAreSame()
		{
			//Šifrování není podporováno.
			//FileStorageServiceTestHelpers.FileStorageService_SavedAndReadContentsAreSame_Perform(GetAzureFileStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
		}

		//[TestMethod]
		public async Task AzureFileStorageService_SavedAndReadContentsWithEncryptionAreSame_Async()
		{
			await Task.CompletedTask;
			//Šifrování není podporováno.
			//await FileStorageServiceTestHelpers.FileStorageService_SavedAndReadContentsAreSame_PerformAsync(GetAzureFileStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
		}

		[TestMethod]
		public void AzureFileStorageService_DoesNotExistsAfterDelete()
		{
			FileStorageServiceTestHelpers.FileStorageService_DoesNotExistsAfterDelete(GetAzureFileStorageService());
		}

		[TestMethod]
		public async Task AzureFileStorageService_DoesNotExistsAfterDeleteAsync()
		{
			await FileStorageServiceTestHelpers.FileStorageService_DoesNotExistsAfterDeleteAsync(GetAzureFileStorageService());
		}

		[TestMethod]
		public void AzureFileStorageService_EnumerateFiles_SupportsSearchPattern()
		{
			FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_SupportsSearchPattern(GetAzureFileStorageService());
		}

		[TestMethod]
		public async Task AzureFileStorageService_EnumerateFilesAsync_SupportsSearchPattern()
		{
			await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_SupportsSearchPattern(GetAzureFileStorageService());
		}

		[TestMethod]
		public void AzureFileStorageService_EnumerateFiles_SupportsSearchPatternInSubfolder()
		{			
			FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_SupportsSearchPatternInSubfolder(GetAzureFileStorageService());
		}

		[TestMethod]
		public async Task AzureFileStorageService_EnumerateFilesAsync_SupportsSearchPatternInSubfolder()
		{
			await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_SupportsSearchPatternInSubfolder(GetAzureFileStorageService());
		}

		//[TestMethod]
		public void AzureFileStorageService_EnumerateFiles_HasLastModifiedUtc()
		{
			// Není podporováno.
			//FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_HasLastModifiedUtc(GetAzureFileStorageService());
		}

		[TestMethod]
		public void AzureFileStorageService_EnumerateFiles_HasSize()
		{
			FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_HasSize(GetAzureFileStorageService());
		}

		//[TestMethod]
		public async Task AzureFileStorageService_EnumerateFilesAsync_HasLastModifiedUtc()
		{
			// Není podporováno.
			//await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_HasLastModifiedUtc(GetAzureFileStorageService());
			await Task.CompletedTask;
		}

		[TestMethod]
		public async Task AzureFileStorageService_EnumerateFilesAsync_HasSize()
		{
			await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_HasSize(GetAzureFileStorageService());
		}

		//[TestMethod]
		public void AzureFileStorageService_Read_StopReadingFarBeforeEndDoesNotThrowCryptographicException()
		{
			//Šifrování není podporováno.
			//FileStorageServiceTestHelpers.FileStorageService_Read_StopReadingFarBeforeEndDoesNotThrowCryptographicException(GetAzureFileStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
		}

		//[TestMethod]
		public async Task AzureFileStorageService_ReadAsync_StopReadingFarBeforeEndDoesNotThrowCryptographicException()
		{
			//Šifrování není podporováno.
			//await FileStorageServiceTestHelpers.FileStorageService_ReadAsync_StopReadingFarBeforeEndDoesNotThrowCryptographicException(GetAzureFileStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
			await Task.CompletedTask;
		}

		private static AzureFileStorageService GetAzureFileStorageService(string fileShareName = "tests")
		{
			// we do not want to leak our Azure Storage connection string + we need to have it accessible for build + all HAVIT developers as easy as possible
			// use your own Azure Storage account if you do not have access to this file
			return new AzureFileStorageService(File.ReadAllText(@"\\topol.havit.local\Workspace\002.HFW\Havit.Services.Azure.Tests.HfwTestsStorage.connectionString.txt"), fileShareName, "root1\\root2");
		}
	}
}
