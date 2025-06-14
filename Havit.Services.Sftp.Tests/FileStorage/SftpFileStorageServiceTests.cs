﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Havit.Services.TestHelpers.FileStorage;
using Havit.Services.Sftp.FileStorage;
using Microsoft.Extensions.DependencyInjection;
using Renci.SshNet;
using Havit.Services.Sftp.Tests.FileStorage.Infrastructure;
using Havit.Services.FileStorage;

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
	private static SftpCredentials PrimaryUserCredentials = new SftpCredentials("hfwsftpteststorage.sftp-primary.hfwprimary", "Sftp_Primary_Password");
	private static SftpCredentials SecondaryUserCredentials = new SftpCredentials("hfwsftpteststorage.sftp-secondary.hfwsecondary", "Sftp_Secondary_Password");
	private static SftpCredentials EnumerateFilesSupportsSearchPatternUserCredentials = new SftpCredentials("hfwsftpteststorage.sftp-enumerate1.hfwenumerate1", "Sftp_Enumerate1_Password");
	private static SftpCredentials EnumerateFilesAsyncSupportsSearchPatternUserCredentials = new SftpCredentials("hfwsftpteststorage.sftp-enumerate2.hfwenumerate2", "Sftp_Enumerate2_Password");
	private static SftpCredentials EnumerateFilesSearchPatternIsCaseSensitiveUserCredentials = new SftpCredentials("hfwsftpteststorage.sftp-enumerate3.hfwenumerate3", "Sftp_Enumerate3_Password");
	private static SftpCredentials EnumerateFilesAsyncSearchPatternIsCaseSensitiveUserCredentials = new SftpCredentials("hfwsftpteststorage.sftp-enumerate4.hfwenumerate4", "Sftp_Enumerate4_Password");
	private static SftpCredentials EnumerateFilesSupportsSearchPatternInSubfolder = new SftpCredentials("hfwsftpteststorage.sftp-enumerate5.hfwenumerate5", "Sftp_Enumerate5_Password");
	private static SftpCredentials EnumerateFilesAsyncSupportsSearchPatternInSubfolder = new SftpCredentials("hfwsftpteststorage.sftp-enumerate6.hfwenumerate6", "Sftp_Enumerate6_Password");

	private static SftpCredentials[] AllUserCredentials = new[]
	{
		PrimaryUserCredentials,
		SecondaryUserCredentials,
		EnumerateFilesSupportsSearchPatternUserCredentials,
		EnumerateFilesAsyncSupportsSearchPatternUserCredentials,
		EnumerateFilesSearchPatternIsCaseSensitiveUserCredentials,
		EnumerateFilesAsyncSearchPatternIsCaseSensitiveUserCredentials,
		EnumerateFilesSupportsSearchPatternInSubfolder,
		EnumerateFilesAsyncSupportsSearchPatternInSubfolder
	};

	private static bool _cleanUpEnabled = true;

	[ClassInitialize]
	public static void Initialize(TestContext testContext)
	{
		try
		{
			CleanUp(testContext);
		}
#if DEBUG
		catch (AggregateException aex) when (aex.InnerException is Renci.SshNet.Common.SshConnectionException && aex.InnerException.Message.Contains("SSH/SFTP are not enabled for this account."))
		{
			_cleanUpEnabled = false;
			Assert.Inconclusive("SFTP server is disabled (on the Azure Blob Storage).");
		}
#endif
		catch
		{
			_cleanUpEnabled = false;
			throw;
		}
	}

	[ClassCleanup]
	public static void CleanUp(TestContext testContext)
	{
		if (_cleanUpEnabled)
		{
			// testy jsou slušné, mažou po sobě
			// ve scénáři, kdy testy procházejí, není nutno tedy čistit před každým testem, ale čistíme pouze preventivně před všemi testy

			Parallel.ForEach(AllUserCredentials, (sftpCredentials) =>
			{
				CleanDirectory(GetSftpFileStorageService(sftpCredentials), "");
			});
		}
	}

	private static void CleanDirectory(SftpStorageService sftpStorageService, string directory)
	{
		var sftpClient = sftpStorageService.GetConnectedSftpClient();
		List<Renci.SshNet.Sftp.ISftpFile> sftpFiles = sftpClient.ListDirectory(directory).ToList();
		foreach (var sftpFile in sftpFiles)
		{
			if (sftpFile.IsRegularFile)
			{
				sftpClient.Delete(sftpFile.FullName);
			}
			if (sftpFile.IsDirectory)
			{
				CleanDirectory(sftpStorageService, directory + sftpFile.Name + "/");
				sftpClient.Delete(sftpFile.FullName);
			}
		}
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
		FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_SupportsSearchPattern(GetSftpFileStorageService(EnumerateFilesSupportsSearchPatternUserCredentials));
	}

	[TestMethod]
	public async Task SftpStorageService_EnumerateFilesAsync_SupportsSearchPattern()
	{
		await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_SupportsSearchPattern(GetSftpFileStorageService(EnumerateFilesAsyncSupportsSearchPatternUserCredentials));
	}

	[TestMethod]
	public void SftpStorageService_EnumerateFiles_SearchPatternIsCaseSensitive()
	{
		FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_SearchPatternIsCaseSensitive(GetSftpFileStorageService(EnumerateFilesSearchPatternIsCaseSensitiveUserCredentials));
	}

	[TestMethod]
	public async Task SftpStorageService_EnumerateFilesAsync_SearchPatternIsCaseSensitive()
	{
		await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_SearchPatternIsCaseSensitive(GetSftpFileStorageService(EnumerateFilesAsyncSearchPatternIsCaseSensitiveUserCredentials));
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

	//[TestMethod]
	public void SftpStorageService_OpenRead_StopReadingFarBeforeEndDoesNotThrowCryptographicException()
	{
		//Šifrování není podporováno.
		//FileStorageServiceTestHelpers.FileStorageService_Read_StopReadingFarBeforeEndDoesNotThrowCryptographicException(GetAzureFileStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
	}

	//[TestMethod]
	public async Task SftpStorageService_OpenReadAsync_StopReadingFarBeforeEndDoesNotThrowCryptographicException()
	{
		//Šifrování není podporováno.
		//await FileStorageServiceTestHelpers.FileStorageService_ReadAsync_StopReadingFarBeforeEndDoesNotThrowCryptographicException(GetAzureFileStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
		await Task.CompletedTask;
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
	public void SftpStorageService_Copy()
	{
		FileStorageServiceTestHelpers.FileStorageService_Copy(GetSftpFileStorageService(), GetSftpFileStorageService(SecondaryUserCredentials), suffix: "multiple");
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
		await FileStorageServiceTestHelpers.FileStorageService_CopyAsync(GetSftpFileStorageService(), GetSftpFileStorageService(SecondaryUserCredentials), suffix: "multiple");
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
		FileStorageServiceTestHelpers.FileStorageService_Copy_OverwritesTargetFile(GetSftpFileStorageService(), GetSftpFileStorageService(SecondaryUserCredentials));
	}

	[TestMethod]
	public async Task SftpStorageService_CopyAsync_OverwritesTargetFile()
	{
		await FileStorageServiceTestHelpers.FileStorageService_CopyAsync_OverwritesTargetFile(GetSftpFileStorageService(), GetSftpFileStorageService(SecondaryUserCredentials));
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
		FileStorageServiceTestHelpers.FileStorageService_Move_WithFileStorageService(GetSftpFileStorageService(), GetSftpFileStorageService(SecondaryUserCredentials));
	}

	[TestMethod]
	public async Task SftpStorageService_MoveAsync_WithFileStorageService()
	{
		await FileStorageServiceTestHelpers.FileStorageService_MoveAsync_WithFileStorageService(GetSftpFileStorageService(), GetSftpFileStorageService(SecondaryUserCredentials));
	}

	[TestMethod]
	public void SftpStorageService_Move_WithFileStorageService_OverwritesTargetFile()
	{
		FileStorageServiceTestHelpers.FileStorageService_Move_WithFileStorageService_OverwritesTargetFile(GetSftpFileStorageService(), GetSftpFileStorageService(SecondaryUserCredentials));
	}

	[TestMethod]
	public async Task SftpStorageService_MoveAsync_WithFileStorageService_OverwritesTargetFile()
	{
		await FileStorageServiceTestHelpers.FileStorageService_MoveAsync_WithFileStorageService_OverwritesTargetFile(GetSftpFileStorageService(), GetSftpFileStorageService(SecondaryUserCredentials));
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

	private static SftpStorageService GetSftpFileStorageService(SftpCredentials sftpStorageServiceProvider = null)
	{
		// we do not want to leak our Azure Storage connection string + we need to have it accessible for build + all HAVIT developers as easy as possible

		sftpStorageServiceProvider ??= PrimaryUserCredentials; // default credentials

		return new SftpStorageService(new SftpStorageServiceOptions { ConnectionInfoFunc = () => new Renci.SshNet.ConnectionInfo("hfwsftpteststorage.blob.core.windows.net", sftpStorageServiceProvider.Username, new Renci.SshNet.PasswordAuthenticationMethod(sftpStorageServiceProvider.Username, sftpStorageServiceProvider.Password)) { MaxSessions = 1 } });
	}
}
