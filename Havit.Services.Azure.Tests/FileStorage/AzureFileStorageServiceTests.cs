using System;
using System.Linq;
using System.Threading.Tasks;
using Havit.Services.Azure.FileStorage;
using Havit.Services.Azure.Tests.FileStorage.Infrastructure;
using Havit.Services.FileStorage;
using Havit.Services.TestHelpers.FileStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Services.Azure.Tests.FileStorage
{
	[TestClass]
	public class AzureFileStorageServiceTests
	{
		private static string testRunSuffix = Guid.NewGuid().ToString("N");

		[ClassInitialize]
		public static void InitializeTestClass(TestContext testContext)
		{
			// testy jsou slušné, mažou po sobě
			// ve scénáři, kdy testy procházejí, není nutno tedy čistit před každým testem, ale čistíme pouze preventivně před všemi testy

			// nemůžeme smazat celý FileShare, protože pokud metoda je ukončena před skutečným smazáním. Další operace nad FileSharem,
			// dokud je mazán, oznamují chybu 409 Confict s popisem "The specified share is being deleted."
			// Proto raději smažeme jen soubory.
			AzureFileStorageService service;

			service = GetAzureFileStorageService();
			service.EnumerateFiles().ToList().ForEach(item => service.Delete(item.Name));

			service = GetAzureFileStorageService(secondary: true);
			service.EnumerateFiles().ToList().ForEach(item => service.Delete(item.Name));
		}

		[ClassCleanup]
		public static void CleanUpTestClass()
		{
			GetAzureFileStorageService().GetShareClient().DeleteIfExists();
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
			FileStorageServiceTestHelpers.FileStorageService_Save_DoNotAcceptSeekedStream(GetAzureFileStorageService());
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
		public void AzureFileStorageService_Save_OverwritesTargetFile()
		{
			FileStorageServiceTestHelpers.FileStorageService_Save_OverwritesTargetFile(GetAzureFileStorageService());
		}

		[TestMethod]
		public async Task AzureFileStorageService_SaveAsync_OverwritesTargetFile()
		{
			await FileStorageServiceTestHelpers.FileStorageService_SaveAsync_OverwritesTargetFile(GetAzureFileStorageService());
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

		[TestMethod]
		public void AzureFileStorageService_EnumerateFiles_ReturnsEmptyOnNonExistingFolder()
		{
			FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_ReturnsEmptyOnNonExistingFolder(GetAzureFileStorageService());
		}

		[TestMethod]
		public async Task AzureFileStorageService_EnumerateFilesAsync_ReturnsEmptyOnNonExistingFolder()
		{
			await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_ReturnsEmptyOnNonExistingFolder(GetAzureFileStorageService());
		}

		[TestMethod]
		public void AzureFileStorageService_EnumerateFiles_HasLastModifiedUtc()
		{
			FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_HasLastModifiedUtc(GetAzureFileStorageService());
		}

		[TestMethod]
		public void AzureFileStorageService_EnumerateFiles_HasSize()
		{
			FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_HasSize(GetAzureFileStorageService());
		}

		[TestMethod]
		public async Task AzureFileStorageService_EnumerateFilesAsync_HasLastModifiedUtc()
		{
			await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_HasLastModifiedUtc(GetAzureFileStorageService());
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

		[TestMethod]
		public void AzureFileStorageService_Copy()
		{
			FileStorageServiceTestHelpers.FileStorageService_Copy(GetAzureFileStorageService(), GetAzureFileStorageService(secondary: true));
		}

		[TestMethod]
		public async Task AzureFileStorageService_CopyAsync()
		{
			await FileStorageServiceTestHelpers.FileStorageService_CopyAsync(GetAzureFileStorageService(), GetAzureFileStorageService(secondary: true));
		}

		[TestMethod]
		public void AzureFileStorageService_Copy_OverwritesTargetFile()
		{
			FileStorageServiceTestHelpers.FileStorageService_Copy_OverwritesTargetFile(GetAzureFileStorageService(), GetAzureFileStorageService(secondary: true));
		}

		[TestMethod]
		public async Task AzureFileStorageService_CopyAsync_OverwritesTargetFile()
		{
			await FileStorageServiceTestHelpers.FileStorageService_CopyAsync_OverwritesTargetFile(GetAzureFileStorageService(), GetAzureFileStorageService(secondary: true));
		}

		[TestMethod]
		public void AzureFileStorageService_Move()
		{
			FileStorageServiceTestHelpers.FileStorageService_Move(GetAzureFileStorageService(), GetAzureFileStorageService(secondary: true));
		}

		[TestMethod]
		public async Task AzureFileStorageService_MoveAsync()
		{
			await FileStorageServiceTestHelpers.FileStorageService_MoveAsync(GetAzureFileStorageService(), GetAzureFileStorageService(secondary: true));
		}

		[TestMethod]
		public void AzureFileStorageService_Move_OverwritesTargetFile()
		{
			FileStorageServiceTestHelpers.FileStorageService_Move_OverwritesTargetFile(GetAzureFileStorageService(), GetAzureFileStorageService(secondary: true));
		}

		[TestMethod]
		public async Task AzureFileStorageService_MoveAsync_OverwritesTargetFile()
		{
			await FileStorageServiceTestHelpers.FileStorageService_MoveAsync_OverwritesTargetFile(GetAzureFileStorageService(), GetAzureFileStorageService(secondary: true));
		}

		[TestMethod]
		public void AzureFileStorageService_DependencyInjectionContainerIntegration()
		{
			// Arrange
			ServiceCollection services = new ServiceCollection();
			services.AddAzureFileStorageService<TestFileStorage>("fake", "fake");
			var provider = services.BuildServiceProvider();

			// Act
			var service = provider.GetService<IFileStorageService<TestFileStorage>>();

			// Assert
			Assert.IsNotNull(service);
			Assert.IsInstanceOfType(service, typeof(AzureFileStorageService<TestFileStorage>));
		}

		private static AzureFileStorageService GetAzureFileStorageService(bool secondary = false)
		{
			return new AzureFileStorageService(new AzureFileStorageServiceOptions
			{
				FileStorageConnectionString = AzureStorageConnectionStringHelper.GetConnectionString(),
				FileShareName = "tests" + testRunSuffix,
				RootDirectoryName = secondary ? "root\\secondarytests" : "root\\primarytests"
			});
		}
	}
}
