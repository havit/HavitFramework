using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System;
using Havit.Services.TestHelpers.FileStorage;
using Havit.Services.Sftp.FileStorage;
using Microsoft.Extensions.DependencyInjection;
using Renci.SshNet;
using Havit.Services.Sftp.Tests.FileStorage.Infrastructure;
using Havit.Services.FileStorage;
using System.Linq;
using System.Collections.Generic;

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
	[ClassInitialize]
	public static void Initialize(TestContext testContext)
	{
		// testy jsou slušné, mažou po sobě
		// ve scénáři, kdy testy procházejí, není nutno tedy čistit před každým testem, ale čistíme pouze preventivně před všemi testy

		CleanDirectory(GetSftpFileStorageService(), "");
		CleanDirectory(GetSftpFileStorageService(secondary: true), "");
	}

	private static void CleanDirectory(SftpStorageService sftpStorageService, string directory)
	{
		var sftpClient = sftpStorageService.GetConnectedSftpClient();
		List<Renci.SshNet.Sftp.SftpFile> sftpFiles = sftpClient.ListDirectory(directory).ToList();
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
	[ExpectedException(typeof(InvalidOperationException))]
	public void SftpStorageService_SaveDoNotAcceptSeekedStream()
	{
		FileStorageServiceTestHelpers.FileStorageService_Save_DoNotAcceptSeekedStream(GetSftpFileStorageService());
	}

	[TestMethod]
	[ExpectedException(typeof(InvalidOperationException))]
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
		FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_SupportsSearchPattern(GetSftpFileStorageService());
	}

	[TestMethod]
	public async Task SftpStorageService_EnumerateFilesAsync_SupportsSearchPattern()
	{
		await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_SupportsSearchPattern(GetSftpFileStorageService());
	}

	[TestMethod]
	public void SftpStorageService_EnumerateFiles_SupportsSearchPatternInSubfolder()
	{
		FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_SupportsSearchPatternInSubfolder(GetSftpFileStorageService());
	}

	[TestMethod]
	public async Task SftpStorageService_EnumerateFilesAsync_SupportsSearchPatternInSubfolder()
	{
		await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_SupportsSearchPatternInSubfolder(GetSftpFileStorageService());
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
	public void SftpStorageService_Read_StopReadingFarBeforeEndDoesNotThrowCryptographicException()
	{
		//Šifrování není podporováno.
		//FileStorageServiceTestHelpers.FileStorageService_Read_StopReadingFarBeforeEndDoesNotThrowCryptographicException(GetAzureFileStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
	}

	//[TestMethod]
	public async Task SftpStorageService_ReadAsync_StopReadingFarBeforeEndDoesNotThrowCryptographicException()
	{
		//Šifrování není podporováno.
		//await FileStorageServiceTestHelpers.FileStorageService_ReadAsync_StopReadingFarBeforeEndDoesNotThrowCryptographicException(GetAzureFileStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
		await Task.CompletedTask;
	}

	[TestMethod]
	public void SftpStorageService_OpenWrite_OpenWriteAndOpenRead_ContentsAreSame()
	{
		FileStorageServiceTestHelpers.FileStorageService_OpenWriteAndOpenRead_ContentsAreSame(GetSftpFileStorageService());
	}

	[TestMethod]
	public async Task SftpStorageService_OpenWriteAsyncAndOpenReadAsync_ContentsAreSame()
	{
		await FileStorageServiceTestHelpers.FileStorageService_OpenWriteAsyncAndOpenReadAsync_ContentsAreSame(GetSftpFileStorageService());
	}

	//[TestMethod]
	public void SftpStorageService_OpenWriteAndOpenReadWithEncryption_ContentsAreSame()
	{
		//Šifrování není podporováno.
		//FileStorageServiceTestHelpers.FileStorageService_OpenWriteAndOpenRead_ContentsAreSame(GetSftpFileStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
	}

	//[TestMethod]
	public Task SftpStorageService_OpenWriteAsyncAndOpenReadAsyncWithEncryption_ContentsAreSame()
	{
		//Šifrování není podporováno.
		//await FileStorageServiceTestHelpers.FileStorageService_OpenWriteAsyncAndOpenReadAsync_ContentsAreSame(GetSftpFileStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
		return Task.CompletedTask;
	}

	[TestMethod]
	public void SftpStorageService_OpenWrite_OverwritesExistingFileAndContent()
	{
		FileStorageServiceTestHelpers.FileStorageService_OpenWrite_OverwritesExistingFileAndContent(GetSftpFileStorageService());
	}

	[TestMethod]
	public async Task SftpStorageService_OpenWriteAsync_OverwritesExistingFileAndContent()
	{
		await FileStorageServiceTestHelpers.FileStorageService_OpenWriteAsync_OverwritesExistingFileAndContent(GetSftpFileStorageService());
	}

	[TestMethod]
	public void SftpStorageService_Copy()
	{
		FileStorageServiceTestHelpers.FileStorageService_Copy(GetSftpFileStorageService(), GetSftpFileStorageService(secondary: true));
	}

	[TestMethod]
	public void SftpStorageService_Copy_SingleInstance()
	{
		SftpStorageService sftpStorageService = GetSftpFileStorageService();
		FileStorageServiceTestHelpers.FileStorageService_Copy(sftpStorageService, sftpStorageService);
	}

	[TestMethod]
	public async Task SftpStorageService_CopyAsync()
	{
		await FileStorageServiceTestHelpers.FileStorageService_CopyAsync(GetSftpFileStorageService(), GetSftpFileStorageService(secondary: true));
	}

	[TestMethod]
	public async Task SftpStorageService_CopyAsync_SingleInstance()
	{
		SftpStorageService sftpStorageService = GetSftpFileStorageService();
		await FileStorageServiceTestHelpers.FileStorageService_CopyAsync(sftpStorageService, sftpStorageService);
	}

	[TestMethod]
	public void SftpStorageService_Copy_OverwritesTargetFile()
	{
		FileStorageServiceTestHelpers.FileStorageService_Copy_OverwritesTargetFile(GetSftpFileStorageService(), GetSftpFileStorageService(secondary: true));
	}

	[TestMethod]
	public async Task SftpStorageService_CopyAsync_OverwritesTargetFile()
	{
		await FileStorageServiceTestHelpers.FileStorageService_CopyAsync_OverwritesTargetFile(GetSftpFileStorageService(), GetSftpFileStorageService(secondary: true));
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
		FileStorageServiceTestHelpers.FileStorageService_Move_WithFileStorageService(GetSftpFileStorageService(), GetSftpFileStorageService(secondary: true));
	}

	[TestMethod]
	public async Task SftpStorageService_MoveAsync_WithFileStorageService()
	{
		await FileStorageServiceTestHelpers.FileStorageService_MoveAsync_WithFileStorageService(GetSftpFileStorageService(), GetSftpFileStorageService(secondary: true));
	}

	[TestMethod]
	public void SftpStorageService_Move_WithFileStorageService_OverwritesTargetFile()
	{
		FileStorageServiceTestHelpers.FileStorageService_Move_WithFileStorageService_OverwritesTargetFile(GetSftpFileStorageService(), GetSftpFileStorageService(secondary: true));
	}

	[TestMethod]
	public async Task SftpStorageService_MoveAsync_WithFileStorageService_OverwritesTargetFile()
	{
		await FileStorageServiceTestHelpers.FileStorageService_MoveAsync_WithFileStorageService_OverwritesTargetFile(GetSftpFileStorageService(), GetSftpFileStorageService(secondary: true));
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
	
	private static SftpStorageService GetSftpFileStorageService(bool secondary = false)
	{
		// we do not want to leak our Azure Storage connection string + we need to have it accessible for build + all HAVIT developers as easy as possible

		string primarySftpUsername = "hfwsftpteststorage.sftp-primary.hfwprimary";
		string secondarySftpUsername = "hfwsftpteststorage.sftp-secondary.hfwsecondary";

		return secondary
			? secondarySftpStorageService ??= new SftpStorageService(new SftpStorageServiceOptions { ConnectionInfoFunc = () => new Renci.SshNet.ConnectionInfo("hfwsftpteststorage.blob.core.windows.net", secondarySftpUsername, new Renci.SshNet.PasswordAuthenticationMethod(secondarySftpUsername, SftpPasswordHelper.GetPasswordForSecondaryAccount())) { MaxSessions = 1 } })
			: primarySftpStorageService ??= new SftpStorageService(new SftpStorageServiceOptions { ConnectionInfoFunc = () => new Renci.SshNet.ConnectionInfo("hfwsftpteststorage.blob.core.windows.net", primarySftpUsername, new Renci.SshNet.PasswordAuthenticationMethod(primarySftpUsername, SftpPasswordHelper.GetPasswordForPrimaryAccount())) { MaxSessions = 1 } });
	}
	private static SftpStorageService primarySftpStorageService;
	private static SftpStorageService secondarySftpStorageService;


}
