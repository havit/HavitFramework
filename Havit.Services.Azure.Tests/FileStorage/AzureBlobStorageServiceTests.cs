﻿using System.Text;
using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Havit.Services.Azure.FileStorage;
using Havit.Services.Azure.Tests.FileStorage.Infrastructure;
using Havit.Services.FileStorage;
using Havit.Services.TestHelpers.FileStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Services.Azure.Tests.FileStorage;

[TestClass]
public class AzureBlobStorageServiceTests
{
	private static string containersSuffix = Guid.NewGuid().ToString("N");

	[ClassInitialize]
	public static void InitializeTestClass(TestContext testContext)
	{
		// testy jsou slušné, mažou po sobě
		// ve scénáři, kdy testy procházejí, není nutno tedy čistit před každým testem, ale čistíme pouze preventivně před všemi testy

		// Nemůžeme smazat celý container, protože pokud metoda je ukončena před skutečným smazáním. Další operace nad containerem,
		// dokud je mazán, oznamují chybu 409 Confict s popisem "The specified container is being deleted."
		// Proto raději smažeme jen bloby.

		AzureBlobStorageService service = GetAzureBlobStorageService();
		service.EnumerateFiles().ToList().ForEach(item => service.Delete(item.Name));

		service = GetAzureBlobStorageService(secondary: true);
		service.EnumerateFiles().ToList().ForEach(item => service.Delete(item.Name));
	}

	[ClassCleanup]
	public static void CleanUpTestClass()
	{
		GetAzureBlobStorageService().GetBlobContainerClient().Delete();
		GetAzureBlobStorageService(secondary: true).GetBlobContainerClient().Delete();
	}

	[TestMethod]
	public void AzureBlobStorageService_Exists_ReturnsFalseWhenNotFound()
	{
		FileStorageServiceTestHelpers.FileStorageService_Exists_ReturnsFalseWhenNotFound(GetAzureBlobStorageService());
	}

	[TestMethod]
	public async Task AzureBlobStorageService_ExistsAsync_ReturnsFalseWhenNotFound()
	{
		await FileStorageServiceTestHelpers.FileStorageService_ExistsAsync_ReturnsFalseWhenNotFound(GetAzureBlobStorageService());
	}

	[TestMethod]
	public void AzureBlobStorageService_Exists_ReturnsTrueWhenExists()
	{
		FileStorageServiceTestHelpers.FileStorageService_Exists_ReturnsTrueWhenExists(GetAzureBlobStorageService());
	}

	[TestMethod]
	public async Task AzureBlobStorageService_ExistsAsync_ReturnsTrueWhenExists()
	{
		await FileStorageServiceTestHelpers.FileStorageService_ExistsAsync_ReturnsTrueWhenExists(GetAzureBlobStorageService());
	}

	[TestMethod]
	[ExpectedException(typeof(InvalidOperationException))]
	public void AzureBlobStorageService_SaveDoNotAcceptSeekedStream()
	{
		FileStorageServiceTestHelpers.FileStorageService_Save_DoNotAcceptSeekedStream(GetAzureBlobStorageService());
	}

	[TestMethod]
	[ExpectedException(typeof(InvalidOperationException))]
	public async Task AzureBlobStorageService_SaveAsyncDoesNotAcceptSeekedStream()
	{
		await FileStorageServiceTestHelpers.FileStorageService_SaveAsyncDoNotAcceptSeekedStream(GetAzureBlobStorageService());
	}

	[TestMethod]
	public void AzureBlobStorageService_SavedAndReadContentsAreSame()
	{
		FileStorageServiceTestHelpers.FileStorageService_SavedAndReadContentsAreSame_Perform(GetAzureBlobStorageService());
	}

	[TestMethod]
	public async Task AzureBlobStorageService_SavedAndReadContentsAreSame_Async()
	{
		await FileStorageServiceTestHelpers.FileStorageService_SavedAndReadContentsAreSame_PerformAsync(GetAzureBlobStorageService());
	}

	[TestMethod]
	public void AzureBlobStorageService_Save_AcceptsPathWithNewSubfolders()
	{
		FileStorageServiceTestHelpers.FileStorageService_Save_AcceptsPathWithNewSubfolders(GetAzureBlobStorageService());
	}

	[TestMethod]
	public async Task AzureBlobStorageService_SaveAsync_AcceptsPathWithNewSubfolders()
	{
		await FileStorageServiceTestHelpers.FileStorageService_SaveAsync_AcceptsPathWithNewSubfolders(GetAzureBlobStorageService());
	}

	[TestMethod]
	public void AzureBlobStorageService_SavedAndReadContentsWithEncryptionAreSame()
	{
		FileStorageServiceTestHelpers.FileStorageService_SavedAndReadContentsAreSame_Perform(GetAzureBlobStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
	}

	[TestMethod]
	public async Task AzureBlobStorageService_SavedAndReadContentsWithEncryptionAreSame_Async()
	{
		await FileStorageServiceTestHelpers.FileStorageService_SavedAndReadContentsAreSame_PerformAsync(GetAzureBlobStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
	}

	[TestMethod]
	public void AzureBlobStorageService_Save_OverwritesTargetFile()
	{
		FileStorageServiceTestHelpers.FileStorageService_Save_OverwritesTargetFile(GetAzureBlobStorageService());
	}

	[TestMethod]
	public async Task AzureBlobStorageService_SaveAsync_OverwritesTargetFile()
	{
		await FileStorageServiceTestHelpers.FileStorageService_SaveAsync_OverwritesTargetFile(GetAzureBlobStorageService());
	}

	[TestMethod]
	public void AzureBlobStorageService_DoesNotExistsAfterDelete()
	{
		FileStorageServiceTestHelpers.FileStorageService_DoesNotExistsAfterDelete(GetAzureBlobStorageService());
	}

	[TestMethod]
	public async Task AzureBlobStorageService_DoesNotExistsAfterDeleteAsync()
	{
		await FileStorageServiceTestHelpers.FileStorageService_DoesNotExistsAfterDeleteAsync(GetAzureBlobStorageService());
	}

	[TestMethod]
	public void AzureBlobStorageService_EnumerateFiles_SupportsSearchPattern()
	{
		FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_SupportsSearchPattern(GetAzureBlobStorageService());
	}

	[TestMethod]
	public async Task AzureBlobStorageService_EnumerateFilesAsync_SupportsSearchPattern()
	{
		await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_SupportsSearchPattern(GetAzureBlobStorageService());
	}

	[TestMethod]
	public void AzureBlobStorageService_EnumerateFiles_SearchPatternIsCaseSensitive()
	{
		FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_SearchPatternIsCaseSensitive(GetAzureBlobStorageService());
	}

	[TestMethod]
	public async Task AzureBlobStorageService_EnumerateFilesAsync_SearchPatternIsCaseSensitive()
	{
		await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_SearchPatternIsCaseSensitive(GetAzureBlobStorageService());
	}

	[TestMethod]
	public void AzureBlobStorageService_EnumerateFiles_SupportsSearchPatternInSubfolder()
	{
		FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_SupportsSearchPatternInSubfolder(GetAzureBlobStorageService());
	}

	[TestMethod]
	public async Task AzureBlobStorageService_EnumerateFilesAsync_SupportsSearchPatternInSubfolder()
	{
		await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_SupportsSearchPatternInSubfolder(GetAzureBlobStorageService());
	}

	[TestMethod]
	public void AzureBlobStorageService_EnumerateFiles_ReturnsEmptyOnNonExistingFolder()
	{
		FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_ReturnsEmptyOnNonExistingFolder(GetAzureBlobStorageService());
	}

	[TestMethod]
	public async Task AzureBlobStorageService_EnumerateFilesAsync_ReturnsEmptyOnNonExistingFolder()
	{
		await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_ReturnsEmptyOnNonExistingFolder(GetAzureBlobStorageService());
	}

	[TestMethod]
	public void AzureBlobStorageService_EnumerateFiles_HasLastModifiedUtc()
	{
		FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_HasLastModifiedUtc(GetAzureBlobStorageService());
	}

	[TestMethod]
	public void AzureBlobStorageService_EnumerateFiles_HasSize()
	{
		FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_HasSize(GetAzureBlobStorageService());
	}

	[TestMethod]
	public async Task AzureBlobStorageService_EnumerateFilesAsync_HasLastModifiedUtc()
	{
		await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_HasLastModifiedUtc(GetAzureBlobStorageService());
	}

	[TestMethod]
	public async Task AzureBlobStorageService_EnumerateFilesAsync_HasSize()
	{
		await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_HasSize(GetAzureBlobStorageService());
	}

	[TestMethod]
	public void AzureBlobStorageService_OpenRead_StopReadingFarBeforeEndDoesNotThrowCryptographicException()
	{
		FileStorageServiceTestHelpers.FileStorageService_OpenRead_StopReadingFarBeforeEndDoesNotThrowCryptographicException(GetAzureBlobStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
	}

	[TestMethod]
	public async Task AzureBlobStorageService_OpenReadAsync_StopReadingFarBeforeEndDoesNotThrowCryptographicException()
	{
		await FileStorageServiceTestHelpers.FileStorageService_OpenReadAsync_StopReadingFarBeforeEndDoesNotThrowCryptographicException(GetAzureBlobStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
	}

	[TestMethod]
	public void AzureBlobStorageService_OpenCreateAndOpenRead_ContentsAreSame()
	{
		FileStorageServiceTestHelpers.FileStorageService_OpenCreateAndOpenRead_ContentsAreSame(GetAzureBlobStorageService());
	}

	[TestMethod]
	public async Task AzureBlobStorageService_OpenCreateAsyncAndOpenReadAsync_ContentsAreSame()
	{
		await FileStorageServiceTestHelpers.FileStorageService_OpenCreateAsyncAndOpenReadAsync_ContentsAreSame(GetAzureBlobStorageService());
	}

	[TestMethod]
	public void AzureBlobStorageService_OpenCreateAndOpenReadWithEncryption_ContentsAreSame()
	{
		FileStorageServiceTestHelpers.FileStorageService_OpenCreateAndOpenRead_ContentsAreSame(GetAzureBlobStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
	}

	[TestMethod]
	public async Task AzureBlobStorageService_OpenCreateAsyncAndOpenReadAsyncWithEncryption_ContentsAreSame()
	{
		await FileStorageServiceTestHelpers.FileStorageService_OpenCreateAsyncAndOpenReadAsync_ContentsAreSame(GetAzureBlobStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
	}

	[TestMethod]
	public void AzureBlobStorageService_OpenCreate_OverwritesExistingFileAndContent()
	{
		FileStorageServiceTestHelpers.FileStorageService_OpenCreate_OverwritesExistingFileAndContent(GetAzureBlobStorageService());
	}

	[TestMethod]
	public async Task AzureBlobStorageService_OpenCreateAsync_OverwritesExistingFileAndContent()
	{
		await FileStorageServiceTestHelpers.FileStorageService_OpenCreateAsync_OverwritesExistingFileAndContent(GetAzureBlobStorageService());
	}

	[TestMethod]
	public void AzureBlobStorageService_Copy()
	{
		FileStorageServiceTestHelpers.FileStorageService_Copy(GetAzureBlobStorageService(), GetAzureBlobStorageService(secondary: true));
	}

	[TestMethod]
	public void AzureBlobStorageService_Copy_SingleInstance()
	{
		AzureBlobStorageService azureBlobStorageService = GetAzureBlobStorageService();
		FileStorageServiceTestHelpers.FileStorageService_Copy(azureBlobStorageService, azureBlobStorageService);
	}

	[TestMethod]
	public async Task AzureBlobStorageService_CopyAsync()
	{
		await FileStorageServiceTestHelpers.FileStorageService_CopyAsync(GetAzureBlobStorageService(), GetAzureBlobStorageService(secondary: true));
	}

	[TestMethod]
	public async Task AzureBlobStorageService_CopyAsync_SingleInstance()
	{
		AzureBlobStorageService azureBlobStorageService = GetAzureBlobStorageService();
		await FileStorageServiceTestHelpers.FileStorageService_CopyAsync(azureBlobStorageService, azureBlobStorageService);
	}

	[TestMethod]
	public void AzureBlobStorageService_Copy_OverwritesTargetFile()
	{
		FileStorageServiceTestHelpers.FileStorageService_Copy_OverwritesTargetFile(GetAzureBlobStorageService(), GetAzureBlobStorageService(secondary: true));
	}

	[TestMethod]
	public async Task AzureBlobStorageService_CopyAsync_OverwritesTargetFile()
	{
		await FileStorageServiceTestHelpers.FileStorageService_CopyAsync_OverwritesTargetFile(GetAzureBlobStorageService(), GetAzureBlobStorageService(secondary: true));
	}

	[TestMethod]
	public void AzureBlobStorageService_Move()
	{
		FileStorageServiceTestHelpers.FileStorageService_Move(GetAzureBlobStorageService());
	}

	[TestMethod]
	public async Task AzureBlobStorageService_MoveAsync()
	{
		await FileStorageServiceTestHelpers.FileStorageService_MoveAsync(GetAzureBlobStorageService());
	}

	[TestMethod]
	public void AzureBlobStorageService_Move_DoesNotDeleteFile()
	{
		AzureBlobStorageService azureBlobStorageService = GetAzureBlobStorageService();
		FileStorageServiceTestHelpers.FileStorageService_Move_DoesNotDeleteFile(azureBlobStorageService);
	}

	[TestMethod]
	public async Task AzureBlobStorageService_MoveAsync_DoesNotDeleteFile()
	{
		AzureBlobStorageService azureBlobStorageService = GetAzureBlobStorageService();
		await FileStorageServiceTestHelpers.FileStorageService_MoveAsync_DoesNotDeleteFile(azureBlobStorageService);
	}

	[TestMethod]
	public void AzureBlobStorageService_Move_OverwritesTargetFile()
	{
		AzureBlobStorageService azureBlobStorageService = GetAzureBlobStorageService();
		FileStorageServiceTestHelpers.FileStorageService_Move_OverwritesTargetFile(azureBlobStorageService);
	}

	[TestMethod]
	public async Task AzureBlobStorageService_MoveAsync_OverwritesTargetFile()
	{
		AzureBlobStorageService azureBlobStorageService = GetAzureBlobStorageService();
		await FileStorageServiceTestHelpers.FileStorageService_MoveAsync_OverwritesTargetFile(azureBlobStorageService);
	}

	[TestMethod]
	public void AzureBlobStorageService_Move_WithFileStorageService()
	{
		FileStorageServiceTestHelpers.FileStorageService_Move_WithFileStorageService(GetAzureBlobStorageService(), GetAzureBlobStorageService(secondary: true));
	}

	[TestMethod]
	public async Task AzureBlobStorageService_MoveAsync_WithFileStorageService()
	{
		await FileStorageServiceTestHelpers.FileStorageService_MoveAsync_WithFileStorageService(GetAzureBlobStorageService(), GetAzureBlobStorageService(secondary: true));
	}

	[TestMethod]
	public void AzureBlobStorageService_Move_WithFileStorageService_OverwritesTargetFile()
	{
		FileStorageServiceTestHelpers.FileStorageService_Move_WithFileStorageService_OverwritesTargetFile(GetAzureBlobStorageService(), GetAzureBlobStorageService(secondary: true));
	}

	[TestMethod]
	public async Task AzureBlobStorageService_MoveAsync_WithFileStorageService_OverwritesTargetFile()
	{
		await FileStorageServiceTestHelpers.FileStorageService_MoveAsync_WithFileStorageService_OverwritesTargetFile(GetAzureBlobStorageService(), GetAzureBlobStorageService(secondary: true));
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
		AzureBlobStorageService service = GetAzureBlobStorageService(cacheControl: cacheControlValue);

		// Act
		using (MemoryStream ms = new MemoryStream())
		{
			service.Save("test.txt", ms, "text/plain"); // uložíme prázdný soubor
		}

		// Assert
		BlobClient blobClient = service.GetBlobClient("test.txt");
		BlobProperties properties = blobClient.GetProperties();
		Assert.AreEqual(cacheControlValue, properties.CacheControl);
	}

	[TestMethod]
	public void AzureBlobStorageService_DependencyInjectionContainerIntegration()
	{
		// Arrange
		ServiceCollection services = new ServiceCollection();
		services.AddAzureBlobStorageService<TestFileStorage>("DefaultEndpointsProtocol=https;AccountName=fake;AccountKey=fake", "fake");
		var provider = services.BuildServiceProvider();

		// Act
		var service = provider.GetService<IFileStorageService<TestFileStorage>>();

		// Assert
		Assert.IsNotNull(service);
		Assert.IsInstanceOfType(service, typeof(AzureBlobStorageService<TestFileStorage>));
	}

	[TestMethod]
	public void AzureBlobStorageService_DependencyInjectionContainerIntegration_WithTokenCredential()
	{
		// Arrange
		ServiceCollection services = new ServiceCollection();
		services.AddAzureBlobStorageService<TestFileStorage>("fake", "fake", new Mock<TokenCredential>().Object);
		var provider = services.BuildServiceProvider();

		// Act
		var service = provider.GetService<IFileStorageService<TestFileStorage>>();

		// Assert
		Assert.IsNotNull(service);
		Assert.IsInstanceOfType(service, typeof(AzureBlobStorageService<TestFileStorage>));
	}

	[TestMethod]
	public async Task AzureBlobStorageService_GenerateSasUri()
	{
		// Arrange
		string filename = "file.txt";
		string content = "abcdefghijklmnopqrśtuvwxyz\r\n12346790\t+ěščřžýáíé";

		var azureBlobStorageService = GetAzureBlobStorageService();
		using (MemoryStream ms = new MemoryStream())
		{
			using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8, 1024, true))
			{
				sw.Write(content);
			}
			ms.Seek(0, SeekOrigin.Begin);

			azureBlobStorageService.Save(filename, ms, "text/plain");
		}

		// Act
		Uri uri = azureBlobStorageService.GenerateSasUri(filename, global::Azure.Storage.Sas.BlobSasPermissions.Read, TimeSpan.FromMinutes(1));

		// Assert
		using var httpClient = new HttpClient();
		string readContent = await httpClient.GetStringAsync(uri);
		Assert.AreEqual(content, readContent);

		// Clean up
		azureBlobStorageService.Delete(filename);
	}

	[TestMethod]
	public async Task AzureBlobStorageService_GenerateSasUri_ExpiredTokenAsync()
	{
		// Act
		var azureBlobStorageService = GetAzureBlobStorageService();
		Uri uri = azureBlobStorageService.GenerateSasUri("abc", global::Azure.Storage.Sas.BlobSasPermissions.Read, TimeSpan.FromMinutes(-1)); // -1 = expired token

		// Assert
		try
		{
			using var httpClient = new HttpClient();
			string readContent = await httpClient.GetStringAsync(uri);
		}
		catch (HttpRequestException webExpcetion) when ((webExpcetion.StatusCode == System.Net.HttpStatusCode.Forbidden))
		{
			return; // test je úspěšný, pokud je odpověď 403 Forbidden
		}

		Assert.Fail();
	}

	private static AzureBlobStorageService GetAzureBlobStorageService(bool secondary = false, string cacheControl = "", EncryptionOptions encryptionOptions = null)
	{
		// we do not want to leak our Azure Storage connection string + we need to have it accessible for build + all HAVIT developers as easy as possible
		return new AzureBlobStorageService(
			new AzureBlobStorageServiceOptions
			{
				BlobStorage = AzureStorageConnectionStringHelper.GetConnectionString(),
				ContainerName = (secondary ? "secondarytests" : "primarytests") + containersSuffix,
				CacheControl = cacheControl,
				EncryptionOptions = encryptionOptions
			});
	}
}
