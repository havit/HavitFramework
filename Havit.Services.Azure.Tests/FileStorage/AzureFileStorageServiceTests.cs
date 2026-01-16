using Havit.Services.Azure.FileStorage;
using Havit.Services.Azure.Tests.FileStorage.Infrastructure;
using Havit.Services.TestHelpers.FileStorage;

namespace Havit.Services.Azure.Tests.FileStorage;

[TestClass]
public class AzureFileStorageServiceTests
{
	private const string PrimaryDirectoryName = "root\\primary";
	private const string SecondaryDirectoryName = "root\\secondary";

	private const string EnumerateFilesSupportsSearchPatternDirectoryName = "root\\enumerate1";
	private const string EnumerateFilesAsyncSupportsSearchPatternDirectoryName = "root\\enumerate2";
	private const string EnumerateFilesSearchPatternIsCaseSensitiveDirectoryName = "root\\enumerate3";
	private const string EnumerateFilesAsyncSearchPatternIsCaseSensitiveDirectoryName = "root\\enumerate4";
	private const string EnumerateFilesSupportsSearchPatternInSubfolderDirectoryName = "root\\enumerate5";
	private const string EnumerateFilesAsyncSupportsSearchPatternInSubfolderDirectoryName = "root\\enumerate6";

	private readonly static string[] AllDirectoryNames = [
		PrimaryDirectoryName,
		SecondaryDirectoryName,
		EnumerateFilesSupportsSearchPatternDirectoryName,
		EnumerateFilesAsyncSupportsSearchPatternDirectoryName,
		EnumerateFilesSearchPatternIsCaseSensitiveDirectoryName,
		EnumerateFilesAsyncSearchPatternIsCaseSensitiveDirectoryName,
		EnumerateFilesSupportsSearchPatternInSubfolderDirectoryName,
		EnumerateFilesAsyncSupportsSearchPatternInSubfolderDirectoryName
	];

	public TestContext TestContext { get; set; }

	[ClassInitialize]
	public static void InitializeTestClass(TestContext _)
	{
		// testy jsou slušné, mažou po sobě
		// ve scénáři, kdy testy procházejí, není nutno tedy čistit před každým testem, ale čistíme pouze preventivně před všemi testy

		// nemůžeme smazat celý FileShare, protože pokud metoda je ukončena před skutečným smazáním. Další operace nad FileSharem,
		// dokud je mazán, oznamují chybu 409 Confict s popisem "The specified share is being deleted."
		// Proto raději smažeme jen soubory.
		Parallel.ForEach(AllDirectoryNames, directoryName =>
		{
			var service = GetAzureFileStorageService(directoryName);
			service.EnumerateFiles().ToList().ForEach(item => service.Delete(item.Name));
		});
	}

	[ClassCleanup]
	public static void CleanUpTestClass()
	{
		GetAzureFileStorageService().GetShareClient().DeleteIfExists(cancellationToken: CancellationToken.None);
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
	public void AzureFileStorageService_SaveDoNotAcceptSeekedStream()
	{
		FileStorageServiceTestHelpers.FileStorageService_Save_DoNotAcceptSeekedStream(GetAzureFileStorageService());
	}

	[TestMethod]
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
		FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_SupportsSearchPattern(GetAzureFileStorageService(EnumerateFilesSupportsSearchPatternDirectoryName));
	}

	[TestMethod]
	public async Task AzureFileStorageService_EnumerateFilesAsync_SupportsSearchPattern()
	{
		await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_SupportsSearchPattern(GetAzureFileStorageService(EnumerateFilesAsyncSupportsSearchPatternDirectoryName));
	}

	[TestMethod]
	public void AzureFileStorageService_EnumerateFiles_SearchPatternIsCaseSensitive()
	{
		FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_SearchPatternIsCaseSensitive(GetAzureFileStorageService(EnumerateFilesSearchPatternIsCaseSensitiveDirectoryName));
	}

	[TestMethod]
	public async Task AzureFileStorageService_EnumerateFilesAsync_SearchPatternIsCaseSensitive()
	{
		await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_SearchPatternIsCaseSensitive(GetAzureFileStorageService(EnumerateFilesAsyncSearchPatternIsCaseSensitiveDirectoryName));
	}

	[TestMethod]
	public void AzureFileStorageService_EnumerateFiles_SupportsSearchPatternInSubfolder()
	{
		FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_SupportsSearchPatternInSubfolder(GetAzureFileStorageService(EnumerateFilesSupportsSearchPatternInSubfolderDirectoryName));
	}

	[TestMethod]
	public async Task AzureFileStorageService_EnumerateFilesAsync_SupportsSearchPatternInSubfolder()
	{
		await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_SupportsSearchPatternInSubfolder(GetAzureFileStorageService(EnumerateFilesAsyncSupportsSearchPatternInSubfolderDirectoryName));
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
	[TestMethod]
	public void AzureFileStorageService_GetLastModifiedTimeUtc_ThrowsFileNotFoundExceptionForNonExistingFile()
	{
		FileStorageServiceTestHelpers.FileStorageService_GetLastModifiedTimeUtc_ThrowsFileNotFoundExceptionForNonExistingFile(GetAzureFileStorageService());
	}

	[TestMethod]
	public async Task AzureFileStorageService_GetLastModifiedTimeUtcAsync_ThrowsFileNotFoundExceptionForNonExistingFile()
	{
		await FileStorageServiceTestHelpers.FileStorageService_GetLastModifiedTimeUtcAsync_ThrowsFileNotFoundExceptionForNonExistingFile(GetAzureFileStorageService());
	}

	//[TestMethod]
	public void AzureFileStorageService_OpenRead_StopReadingFarBeforeEndDoesNotThrowCryptographicException()
	{
		//Šifrování není podporováno.
		//FileStorageServiceTestHelpers.FileStorageService_Read_StopReadingFarBeforeEndDoesNotThrowCryptographicException(GetAzureFileStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
	}

	[TestMethod]
	public void AzureFileStorageService_OpenRead_ThrowsFileNotFoundExceptionForNonExistingFile()
	{
		FileStorageServiceTestHelpers.FileStorageService_OpenRead_ThrowsFileNotFoundExceptionForNonExistingFile(GetAzureFileStorageService());
	}

	//[TestMethod]
	public async Task AzureFileStorageService_OpenReadAsync_StopReadingFarBeforeEndDoesNotThrowCryptographicException()
	{
		//Šifrování není podporováno.
		//await FileStorageServiceTestHelpers.FileStorageService_ReadAsync_StopReadingFarBeforeEndDoesNotThrowCryptographicException(GetAzureFileStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
		await Task.CompletedTask;
	}

	[TestMethod]
	public async Task AzureFileStorageService_OpenReadAsync_ThrowsFileNotFoundExceptionForNonExistingFile()
	{
		await FileStorageServiceTestHelpers.FileStorageService_OpenReadAsync_ThrowsFileNotFoundExceptionForNonExistingFile(GetAzureFileStorageService());
	}

	[TestMethod]
	public void AzureFileStorageService_OpenCreateAndOpenRead_ContentsAreSame()
	{
		FileStorageServiceTestHelpers.FileStorageService_OpenCreateAndOpenRead_ContentsAreSame(GetAzureFileStorageService());
	}

	[TestMethod]
	public async Task AzureFileStorageService_OpenCreateAsyncAndOpenReadAsync_ContentsAreSame()
	{
		await FileStorageServiceTestHelpers.FileStorageService_OpenCreateAsyncAndOpenReadAsync_ContentsAreSame(GetAzureFileStorageService());
	}

	//[TestMethod]
	public void AzureFileStorageService_OpenCreateAndOpenReadWithEncryption_ContentsAreSame()
	{
		//Šifrování není podporováno.
		//FileStorageServiceTestHelpers.FileStorageService_OpenCreateAndOpenRead_ContentsAreSame(GetAzureFileStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
	}

	//[TestMethod]
	public Task AzureFileStorageService_OpenCreateAsyncAndOpenReadAsyncWithEncryption_ContentsAreSame()
	{
		//Šifrování není podporováno.
		//await FileStorageServiceTestHelpers.FileStorageService_OpenCreateAsyncAndOpenReadAsync_ContentsAreSame(GetAzureFileStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
		return Task.CompletedTask;
	}

	[TestMethod]
	public void AzureFileStorageService_OpenCreate_OverwritesExistingFileAndContent()
	{
		FileStorageServiceTestHelpers.FileStorageService_OpenCreate_OverwritesExistingFileAndContent(GetAzureFileStorageService());
	}

	[TestMethod]
	public async Task AzureFileStorageService_OpenCreateAsync_OverwritesExistingFileAndContent()
	{
		await FileStorageServiceTestHelpers.FileStorageService_OpenCreateAsync_OverwritesExistingFileAndContent(GetAzureFileStorageService());
	}

	[TestMethod]
	public void AzureFileStorageService_ReadToStream_ThrowsFileNotFoundExceptionForNonExistingFile()
	{
		FileStorageServiceTestHelpers.FileStorageService_ReadToStream_ThrowsFileNotFoundExceptionForNonExistingFile(GetAzureFileStorageService());
	}

	[TestMethod]
	public async Task AzureFileStorageService_ReadToStreamAsync_ThrowsFileNotFoundExceptionForNonExistingFile()
	{
		await FileStorageServiceTestHelpers.FileStorageService_ReadToStreamAsync_ThrowsFileNotFoundExceptionForNonExistingFile(GetAzureFileStorageService());
	}

	[TestMethod]
	public void AzureFileStorageService_Copy()
	{
		FileStorageServiceTestHelpers.FileStorageService_Copy(GetAzureFileStorageService(), GetAzureFileStorageService(secondary: true), suffix: "multiple");
	}

	[TestMethod]
	public void AzureFileStorageService_Copy_SingleInstance()
	{
		AzureFileStorageService azureFileStorageService = GetAzureFileStorageService();
		FileStorageServiceTestHelpers.FileStorageService_Copy(azureFileStorageService, azureFileStorageService, suffix: "single");
	}

	[TestMethod]
	public async Task AzureFileStorageService_CopyAsync()
	{
		await FileStorageServiceTestHelpers.FileStorageService_CopyAsync(GetAzureFileStorageService(), GetAzureFileStorageService(secondary: true), suffix: "multiple");
	}

	[TestMethod]
	public async Task AzureFileStorageService_CopyAsync_SingleInstance()
	{
		AzureFileStorageService azureFileStorageService = GetAzureFileStorageService();
		await FileStorageServiceTestHelpers.FileStorageService_CopyAsync(azureFileStorageService, azureFileStorageService, suffix: "single");
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
		FileStorageServiceTestHelpers.FileStorageService_Move(GetAzureFileStorageService());
	}

	[TestMethod]
	public async Task AzureFileStorageService_MoveAsync()
	{
		await FileStorageServiceTestHelpers.FileStorageService_MoveAsync(GetAzureFileStorageService());
	}

	[TestMethod]
	public void AzureFileStorageService_Move_DoesNotDeleteFile()
	{
		AzureFileStorageService azureFileStorageService = GetAzureFileStorageService();
		FileStorageServiceTestHelpers.FileStorageService_Move_DoesNotDeleteFile(azureFileStorageService);
	}

	[TestMethod]
	public async Task AzureFileStorageService_MoveAsync_DoesNotDeleteFile()
	{
		AzureFileStorageService azureFileStorageService = GetAzureFileStorageService();
		await FileStorageServiceTestHelpers.FileStorageService_MoveAsync_DoesNotDeleteFile(azureFileStorageService);
	}

	[TestMethod]
	public void AzureFileStorageService_Move_OverwritesTargetFile()
	{
		AzureFileStorageService azureFileStorageService = GetAzureFileStorageService();
		FileStorageServiceTestHelpers.FileStorageService_Move_OverwritesTargetFile(azureFileStorageService);
	}

	[TestMethod]
	public async Task AzureFileStorageService_MoveAsync_OverwritesTargetFile()
	{
		AzureFileStorageService azureFileStorageService = GetAzureFileStorageService();
		await FileStorageServiceTestHelpers.FileStorageService_MoveAsync_OverwritesTargetFile(azureFileStorageService);
	}

	[TestMethod]
	public void AzureFileStorageService_Move_WithFileStorageService()
	{
		FileStorageServiceTestHelpers.FileStorageService_Move_WithFileStorageService(GetAzureFileStorageService(), GetAzureFileStorageService(secondary: true));
	}

	[TestMethod]
	public async Task AzureFileStorageService_MoveAsync_WithFileStorageService()
	{
		await FileStorageServiceTestHelpers.FileStorageService_MoveAsync_WithFileStorageService(GetAzureFileStorageService(), GetAzureFileStorageService(secondary: true));
	}

	[TestMethod]
	public void AzureFileStorageService_Move_WithFileStorageService_OverwritesTargetFile()
	{
		FileStorageServiceTestHelpers.FileStorageService_Move_WithFileStorageService_OverwritesTargetFile(GetAzureFileStorageService(), GetAzureFileStorageService(secondary: true));
	}

	[TestMethod]
	public async Task AzureFileStorageService_MoveAsync_WithFileStorageService_OverwritesTargetFile()
	{
		await FileStorageServiceTestHelpers.FileStorageService_MoveAsync_WithFileStorageService_OverwritesTargetFile(GetAzureFileStorageService(), GetAzureFileStorageService(secondary: true));
	}

	private static AzureFileStorageService GetAzureFileStorageService(bool secondary = false)
	{
		return GetAzureFileStorageService(rootDirectoryName: secondary ? "root\\sedondary" : "root\\primary");
	}

	private static AzureFileStorageService GetAzureFileStorageService(string rootDirectoryName)
	{
		return new AzureFileStorageService(new AzureFileStorageServiceOptions
		{
			FileStorageConnectionString = AzureStorageConnectionStringHelper.GetConnectionString(),
			FileShareName = "unittests" + GetFileShareSuffix(),
			RootDirectoryName = rootDirectoryName
		});
	}

	internal static string GetFileShareSuffix() => AzureBlobStorageServiceTests.GetContainersSuffix();
}
