using DotNet.Testcontainers.Builders;
using Havit.Services.FileStorage;
using Havit.Services.Sftp.FileStorage;
using Havit.Services.Sftp.Tests.FileStorage.Infrastructure;
using Havit.Services.TestHelpers.FileStorage;
using Microsoft.Extensions.DependencyInjection;
using Renci.SshNet;
using Testcontainers.Sftp;

namespace Havit.Services.Sftp.Tests.FileStorage;

/// <summary>
/// Test SftpStorageService.
/// </summary>
/// <remarks>
/// Třída předpokládá:
/// * Azure Storage with hiearchical namespaces.
/// * Container sftp-primary, v něm složku home (lze díky hiearchical namespaces).
/// * Container sftp-secondary, v něm složku home (lze díky hiearchical namespaces).
/// * Zapnuté SFTP.
/// * SFTP local user hfw-primary, s home nastaveným na sftp-primary/home.
/// * SFTP local user hfw-secondary, s home nastaveným na sftp-secondary/home.
/// * Hesla lokálních uživatelů v KeyVaultu/DevOps variables.
/// </remarks>
[TestClass]
public class SftpFileStorageServiceTests
{
	private static SftpContainer s_SftpContainer;

	private static SftpUserInfo PrimaryUserInfo = new SftpUserInfo("primary", 1001);
	private static SftpUserInfo SecondaryUserInfo = new SftpUserInfo("secondary", 1002);
	private static SftpUserInfo EnumerateFilesSupportsSearchPatternUserInfo = new SftpUserInfo("enumerate1", 1003);
	private static SftpUserInfo EnumerateFilesAsyncSupportsSearchPatternUserInfo = new SftpUserInfo("enumerate2", 1004);
	private static SftpUserInfo EnumerateFilesSearchPatternIsCaseSensitiveUserInfo = new SftpUserInfo("enumerate3", 1005);
	private static SftpUserInfo EnumerateFilesAsyncSearchPatternIsCaseSensitiveUserInfo = new("enumerate4", 1006);
	private static SftpUserInfo EnumerateFilesSupportsSearchPatternInSubfolder = new SftpUserInfo("enumerate5", 1007);
	private static SftpUserInfo EnumerateFilesAsyncSupportsSearchPatternInSubfolder = new SftpUserInfo("enumerate6", 1008);

	private static SftpUserInfo[] AllUserInfo = new[]
	{
		PrimaryUserInfo,
		SecondaryUserInfo,
		EnumerateFilesSupportsSearchPatternUserInfo,
		EnumerateFilesAsyncSupportsSearchPatternUserInfo,
		EnumerateFilesSearchPatternIsCaseSensitiveUserInfo,
		EnumerateFilesAsyncSearchPatternIsCaseSensitiveUserInfo,
		EnumerateFilesSupportsSearchPatternInSubfolder,
		EnumerateFilesAsyncSupportsSearchPatternInSubfolder
	};

	[ClassInitialize]
	public static async void Initialize()
	{
		string sftpUsers = string.Join(",", AllUserInfo.Select(credentials => $"{credentials.Username}:{credentials.Password}:{credentials.UID}:{credentials.Gid}:{credentials.HomeDirectory}"));

		s_SftpContainer = new SftpBuilder()
			.WithImage("atmoz/sftp:alpine")
			.WithEnvironment("SFTP_USERS", sftpUsers)
			.WithPortBinding(22, true)
			.WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(22))
			.Build();

		await s_SftpContainer.StartAsync();
	}

	[ClassCleanup]
	public static void CleanUp(TestContext _)
	{
	//	await s_SftpContainer.DisposeAsync();
	}

	[TestMethod]
	public void SftpStorageService_Exists_ReturnsFalseWhenNotFound()
	{
		FileStorageServiceTestHelpers.FileStorageService_Exists_ReturnsFalseWhenNotFound(GetSftpFileStorageService());
	}

	[TestMethod]
	public async Task SftpStorageService_ExistsAsync_ReturnsFalseWhenNotFound()
	{
		await FileStorageServiceTestHelpers.FileStorageService_ExistsAsync_ReturnsFalseWhenNotFound(GetSftpFileStorageService());
	}

	[TestMethod]
	public void SftpStorageService_Exists_ReturnsTrueWhenExists()
	{
		FileStorageServiceTestHelpers.FileStorageService_Exists_ReturnsTrueWhenExists(GetSftpFileStorageService());
	}

	[TestMethod]
	public async Task SftpStorageService_ExistsAsync_ReturnsTrueWhenExists()
	{
		await FileStorageServiceTestHelpers.FileStorageService_ExistsAsync_ReturnsTrueWhenExists(GetSftpFileStorageService());
	}

	[TestMethod]
	public void SftpStorageService_SaveDoNotAcceptSeekedStream()
	{
		FileStorageServiceTestHelpers.FileStorageService_Save_DoNotAcceptSeekedStream(GetSftpFileStorageService());
	}

	[TestMethod]
	public async Task SftpStorageService_SaveAsyncDoesNotAcceptSeekedStream()
	{
		await FileStorageServiceTestHelpers.FileStorageService_SaveAsyncDoNotAcceptSeekedStream(GetSftpFileStorageService());
	}

	[TestMethod]
	public void SftpStorageService_SavedAndReadContentsAreSame()
	{
		FileStorageServiceTestHelpers.FileStorageService_SavedAndReadContentsAreSame_Perform(GetSftpFileStorageService());
	}

	[TestMethod]
	public async Task SftpStorageService_SavedAndReadContentsAreSame_Async()
	{
		await FileStorageServiceTestHelpers.FileStorageService_SavedAndReadContentsAreSame_PerformAsync(GetSftpFileStorageService());
	}

	[TestMethod]
	public void SftpStorageService_Save_AcceptsPathWithNewSubfolders()
	{
		FileStorageServiceTestHelpers.FileStorageService_Save_AcceptsPathWithNewSubfolders(GetSftpFileStorageService());
	}

	[TestMethod]
	public async Task SftpStorageService_SaveAsync_AcceptsPathWithNewSubfolders()
	{
		await FileStorageServiceTestHelpers.FileStorageService_SaveAsync_AcceptsPathWithNewSubfolders(GetSftpFileStorageService());
	}

	//[TestMethod]
	public void SftpStorageService_SavedAndReadContentsWithEncryptionAreSame()
	{
		//Šifrování není podporováno.
		//FileStorageServiceTestHelpers.FileStorageService_SavedAndReadContentsAreSame_Perform(GetAzureFileStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
	}

	//[TestMethod]
	public async Task SftpStorageService_SavedAndReadContentsWithEncryptionAreSame_Async()
	{
		await Task.CompletedTask;
		//Šifrování není podporováno.
		//await FileStorageServiceTestHelpers.FileStorageService_SavedAndReadContentsAreSame_PerformAsync(GetAzureFileStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
	}

	[TestMethod]
	public void SftpStorageService_Save_OverwritesTargetFile()
	{
		FileStorageServiceTestHelpers.FileStorageService_Save_OverwritesTargetFile(GetSftpFileStorageService());
	}

	[TestMethod]
	public async Task SftpStorageService_SaveAsync_OverwritesTargetFile()
	{
		await FileStorageServiceTestHelpers.FileStorageService_SaveAsync_OverwritesTargetFile(GetSftpFileStorageService());
	}

	[TestMethod]
	public void SftpStorageService_DoesNotExistsAfterDelete()
	{
		FileStorageServiceTestHelpers.FileStorageService_DoesNotExistsAfterDelete(GetSftpFileStorageService());
	}

	[TestMethod]
	public async Task SftpStorageService_DoesNotExistsAfterDeleteAsync()
	{
		await FileStorageServiceTestHelpers.FileStorageService_DoesNotExistsAfterDeleteAsync(GetSftpFileStorageService());
	}

	[TestMethod]
	public void SftpStorageService_EnumerateFiles_SupportsSearchPattern()
	{
		FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_SupportsSearchPattern(GetSftpFileStorageService(EnumerateFilesSupportsSearchPatternUserInfo));
	}

	[TestMethod]
	public async Task SftpStorageService_EnumerateFilesAsync_SupportsSearchPattern()
	{
		await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_SupportsSearchPattern(GetSftpFileStorageService(EnumerateFilesAsyncSupportsSearchPatternUserInfo));
	}

	[TestMethod]
	public void SftpStorageService_EnumerateFiles_SearchPatternIsCaseSensitive()
	{
		FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_SearchPatternIsCaseSensitive(GetSftpFileStorageService(EnumerateFilesSearchPatternIsCaseSensitiveUserInfo));
	}

	[TestMethod]
	public async Task SftpStorageService_EnumerateFilesAsync_SearchPatternIsCaseSensitive()
	{
		await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_SearchPatternIsCaseSensitive(GetSftpFileStorageService(EnumerateFilesAsyncSearchPatternIsCaseSensitiveUserInfo));
	}

	[TestMethod]
	public void SftpStorageService_EnumerateFiles_SupportsSearchPatternInSubfolder()
	{
		FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_SupportsSearchPatternInSubfolder(GetSftpFileStorageService(EnumerateFilesSupportsSearchPatternInSubfolder));
	}

	[TestMethod]
	public async Task SftpStorageService_EnumerateFilesAsync_SupportsSearchPatternInSubfolder()
	{
		await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_SupportsSearchPatternInSubfolder(GetSftpFileStorageService(EnumerateFilesAsyncSupportsSearchPatternInSubfolder));
	}

	[TestMethod]
	public void SftpStorageService_EnumerateFiles_ReturnsEmptyOnNonExistingFolder()
	{
		FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_ReturnsEmptyOnNonExistingFolder(GetSftpFileStorageService());
	}

	[TestMethod]
	public async Task SftpStorageService_EnumerateFilesAsync_ReturnsEmptyOnNonExistingFolder()
	{
		await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_ReturnsEmptyOnNonExistingFolder(GetSftpFileStorageService());
	}

	//[TestMethod]
	public void SftpStorageService_EnumerateFiles_HasLastModifiedUtc()
	{
		FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_HasLastModifiedUtc(GetSftpFileStorageService());
	}

	[TestMethod]
	public void SftpStorageService_EnumerateFiles_HasSize()
	{
		FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_HasSize(GetSftpFileStorageService());
	}

	//[TestMethod]
	public async Task SftpStorageService_EnumerateFilesAsync_HasLastModifiedUtc()
	{
		await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_HasLastModifiedUtc(GetSftpFileStorageService());
	}

	[TestMethod]
	public async Task SftpStorageService_EnumerateFilesAsync_HasSize()
	{
		await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_HasSize(GetSftpFileStorageService());
	}

	[TestMethod]
	public void SftpStorageService_GetLastModifiedTimeUtc_ThrowsFileNotFoundExceptionForNonExistingFile()
	{
		FileStorageServiceTestHelpers.FileStorageService_GetLastModifiedTimeUtc_ThrowsFileNotFoundExceptionForNonExistingFile(GetSftpFileStorageService());
	}

	[TestMethod]
	public async Task SftpStorageService_GetLastModifiedTimeUtcAsync_ThrowsFileNotFoundExceptionForNonExistingFile()
	{
		await FileStorageServiceTestHelpers.FileStorageService_GetLastModifiedTimeUtcAsync_ThrowsFileNotFoundExceptionForNonExistingFile(GetSftpFileStorageService());
	}

	//[TestMethod]
	public void SftpStorageService_OpenRead_StopReadingFarBeforeEndDoesNotThrowCryptographicException()
	{
		//Šifrování není podporováno.
		//FileStorageServiceTestHelpers.FileStorageService_Read_StopReadingFarBeforeEndDoesNotThrowCryptographicException(GetAzureFileStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
	}

	[TestMethod]
	public void SftpStorageService_OpenRead_ThrowsFileNotFoundExceptionForNonExistingFile()
	{
		FileStorageServiceTestHelpers.FileStorageService_OpenRead_ThrowsFileNotFoundExceptionForNonExistingFile(GetSftpFileStorageService());
	}

	//[TestMethod]
	public async Task SftpStorageService_OpenReadAsync_StopReadingFarBeforeEndDoesNotThrowCryptographicException()
	{
		//Šifrování není podporováno.
		//await FileStorageServiceTestHelpers.FileStorageService_ReadAsync_StopReadingFarBeforeEndDoesNotThrowCryptographicException(GetAzureFileStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
		await Task.CompletedTask;
	}

	[TestMethod]
	public async Task SftpStorageService_OpenReadAsync_ThrowsFileNotFoundExceptionForNonExistingFile()
	{
		await FileStorageServiceTestHelpers.FileStorageService_OpenReadAsync_ThrowsFileNotFoundExceptionForNonExistingFile(GetSftpFileStorageService());
	}

	[TestMethod]
	public void SftpStorageService_OpenCreateAndOpenRead_ContentsAreSame()
	{
		FileStorageServiceTestHelpers.FileStorageService_OpenCreateAndOpenRead_ContentsAreSame(GetSftpFileStorageService());
	}

	[TestMethod]
	public async Task SftpStorageService_OpenCreateAsyncAndOpenReadAsync_ContentsAreSame()
	{
		await FileStorageServiceTestHelpers.FileStorageService_OpenCreateAsyncAndOpenReadAsync_ContentsAreSame(GetSftpFileStorageService());
	}

	//[TestMethod]
	public void SftpStorageService_OpenCreateAndOpenReadWithEncryption_ContentsAreSame()
	{
		//Šifrování není podporováno.
		//FileStorageServiceTestHelpers.FileStorageService_OpenCreateAndOpenRead_ContentsAreSame(GetSftpFileStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
	}

	//[TestMethod]
	public Task SftpStorageService_OpenCreateAsyncAndOpenReadAsyncWithEncryption_ContentsAreSame()
	{
		//Šifrování není podporováno.
		//await FileStorageServiceTestHelpers.FileStorageService_OpenCreateAsyncAndOpenReadAsync_ContentsAreSame(GetSftpFileStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
		return Task.CompletedTask;
	}

	[TestMethod]
	public void SftpStorageService_OpenCreate_OverwritesExistingFileAndContent()
	{
		FileStorageServiceTestHelpers.FileStorageService_OpenCreate_OverwritesExistingFileAndContent(GetSftpFileStorageService());
	}

	[TestMethod]
	public async Task SftpStorageService_OpenCreateAsync_OverwritesExistingFileAndContent()
	{
		await FileStorageServiceTestHelpers.FileStorageService_OpenCreateAsync_OverwritesExistingFileAndContent(GetSftpFileStorageService());
	}

	[TestMethod]
	public void SftpStorageService_ReadToStream_ThrowsFileNotFoundExceptionForNonExistingFile()
	{
		FileStorageServiceTestHelpers.FileStorageService_ReadToStream_ThrowsFileNotFoundExceptionForNonExistingFile(GetSftpFileStorageService());
	}

	[TestMethod]
	public async Task SftpStorageService_ReadToStreamAsync_ThrowsFileNotFoundExceptionForNonExistingFile()
	{
		await FileStorageServiceTestHelpers.FileStorageService_ReadToStreamAsync_ThrowsFileNotFoundExceptionForNonExistingFile(GetSftpFileStorageService());
	}

	[TestMethod]
	public void SftpStorageService_Copy()
	{
		FileStorageServiceTestHelpers.FileStorageService_Copy(GetSftpFileStorageService(), GetSftpFileStorageService(SecondaryUserInfo), suffix: "multiple");
	}

	[TestMethod]
	public void SftpStorageService_Copy_SingleInstance()
	{
		SftpStorageService sftpStorageService = GetSftpFileStorageService();
		FileStorageServiceTestHelpers.FileStorageService_Copy(sftpStorageService, sftpStorageService, suffix: "single");
	}

	[TestMethod]
	public async Task SftpStorageService_CopyAsync()
	{
		await FileStorageServiceTestHelpers.FileStorageService_CopyAsync(GetSftpFileStorageService(), GetSftpFileStorageService(SecondaryUserInfo), suffix: "multiple");
	}

	[TestMethod]
	public async Task SftpStorageService_CopyAsync_SingleInstance()
	{
		SftpStorageService sftpStorageService = GetSftpFileStorageService();
		await FileStorageServiceTestHelpers.FileStorageService_CopyAsync(sftpStorageService, sftpStorageService, suffix: "single");
	}

	[TestMethod]
	public void SftpStorageService_Copy_OverwritesTargetFile()
	{
		FileStorageServiceTestHelpers.FileStorageService_Copy_OverwritesTargetFile(GetSftpFileStorageService(), GetSftpFileStorageService(SecondaryUserInfo));
	}

	[TestMethod]
	public async Task SftpStorageService_CopyAsync_OverwritesTargetFile()
	{
		await FileStorageServiceTestHelpers.FileStorageService_CopyAsync_OverwritesTargetFile(GetSftpFileStorageService(), GetSftpFileStorageService(SecondaryUserInfo));
	}

	[TestMethod]
	public void SftpStorageService_Move()
	{
		FileStorageServiceTestHelpers.FileStorageService_Move(GetSftpFileStorageService());
	}

	[TestMethod]
	public async Task SftpStorageService_MoveAsync()
	{
		await FileStorageServiceTestHelpers.FileStorageService_MoveAsync(GetSftpFileStorageService());
	}

	[TestMethod]
	public void SftpStorageService_Move_DoesNotDeleteFile()
	{
		SftpStorageService sftpStorageService = GetSftpFileStorageService();
		FileStorageServiceTestHelpers.FileStorageService_Move_DoesNotDeleteFile(sftpStorageService);
	}

	[TestMethod]
	public async Task SftpStorageService_MoveAsync_DoesNotDeleteFile()
	{
		SftpStorageService sftpStorageService = GetSftpFileStorageService();
		await FileStorageServiceTestHelpers.FileStorageService_MoveAsync_DoesNotDeleteFile(sftpStorageService);
	}

	[TestMethod]
	public void SftpStorageService_Move_OverwritesTargetFile()
	{
		SftpStorageService sftpStorageService = GetSftpFileStorageService();
		FileStorageServiceTestHelpers.FileStorageService_Move_OverwritesTargetFile(sftpStorageService);
	}

	[TestMethod]
	public async Task SftpStorageService_MoveAsync_OverwritesTargetFile()
	{
		SftpStorageService sftpStorageService = GetSftpFileStorageService();
		await FileStorageServiceTestHelpers.FileStorageService_MoveAsync_OverwritesTargetFile(sftpStorageService);
	}

	[TestMethod]
	public void SftpStorageService_Move_WithFileStorageService()
	{
		FileStorageServiceTestHelpers.FileStorageService_Move_WithFileStorageService(GetSftpFileStorageService(), GetSftpFileStorageService(SecondaryUserInfo));
	}

	[TestMethod]
	public async Task SftpStorageService_MoveAsync_WithFileStorageService()
	{
		await FileStorageServiceTestHelpers.FileStorageService_MoveAsync_WithFileStorageService(GetSftpFileStorageService(), GetSftpFileStorageService(SecondaryUserInfo));
	}

	[TestMethod]
	public void SftpStorageService_Move_WithFileStorageService_OverwritesTargetFile()
	{
		FileStorageServiceTestHelpers.FileStorageService_Move_WithFileStorageService_OverwritesTargetFile(GetSftpFileStorageService(), GetSftpFileStorageService(SecondaryUserInfo));
	}

	[TestMethod]
	public async Task SftpStorageService_MoveAsync_WithFileStorageService_OverwritesTargetFile()
	{
		await FileStorageServiceTestHelpers.FileStorageService_MoveAsync_WithFileStorageService_OverwritesTargetFile(GetSftpFileStorageService(), GetSftpFileStorageService(SecondaryUserInfo));
	}

	[TestMethod]
	public void SftpStorageService_DependencyInjectionContainerIntegration()
	{
		// Arrange
		ServiceCollection services = new ServiceCollection();
		services.AddSftpStorageService<TestFileStorage>(() => new ConnectionInfo("fake", "fake", new PasswordAuthenticationMethod("fake", "fake")));
		var provider = services.BuildServiceProvider();

		// Act
		var service = provider.GetService<IFileStorageService<TestFileStorage>>();

		// Assert
		Assert.IsNotNull(service);
		Assert.IsInstanceOfType(service, typeof(SftpStorageService<TestFileStorage>));
	}

	private static SftpStorageService GetSftpFileStorageService(SftpUserInfo sftpUserInfo = null)
	{
		// we do not want to leak our Azure Storage connection string + we need to have it accessible for build + all HAVIT developers as easy as possible

		sftpUserInfo ??= PrimaryUserInfo; // default credentials

		return new SftpStorageService(new SftpStorageServiceOptions
		{
			ConnectionInfoFunc = () => new Renci.SshNet.ConnectionInfo(
				host: s_SftpContainer.Hostname,
				port: s_SftpContainer.GetMappedPublicPort(22),
				username: sftpUserInfo.Username,
				authenticationMethods: new Renci.SshNet.PasswordAuthenticationMethod(sftpUserInfo.Username, sftpUserInfo.Password))
			{ MaxSessions = 1 }
		});
	}
}

