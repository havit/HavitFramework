﻿using Havit.Services.FileStorage;
using Havit.Services.TestHelpers;
using Havit.Services.TestHelpers.FileStorage;
using Havit.Services.Tests.FileStorage.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Services.Tests.FileStorage;

[TestClass]
public class FileSystemStorageServiceTests
{
	[ClassInitialize]
	public static void Initialize(TestContext testContext)
	{
		// testy jsou slušné, mažou po sobě
		// ve scénáři, kdy testy procházejí, není nutno tedy čistit před každým testem, ale čistíme pouze preventivně před všemi testy
		CleanUp();
		Directory.CreateDirectory(GetStoragePath());
		Directory.CreateDirectory(GetStoragePath(secondary: true));
	}

	[ClassCleanup]
	public static void CleanUp()
	{
		string path = GetStoragePath();
		if (Directory.Exists(path))
		{
			Directory.Delete(path, true);
		}

		path = GetStoragePath(secondary: true);
		if (Directory.Exists(path))
		{
			Directory.Delete(path, true);
		}
	}

	[TestMethod]
	public void FileSystemStorageService_Constructor_ReplacesTEMP()
	{
		// Arrange
		// noop

		// Act
		var fileSystemStorageService = new FileSystemStorageService(@"%TEMP%\SomeFolder");

		// Assert
		Assert.IsFalse(fileSystemStorageService.StoragePath.Contains("%TEMP%"));
		Assert.IsFalse(fileSystemStorageService.StoragePath.Contains(@"\\")); // neobsahuje dvě zpětná lomítka za sebou
	}

	[TestMethod]
	[ExpectedException(typeof(ArgumentException))]
	public void FileSystemStorageService_Constructor_NeedsStoragePathWhenNotUsingFullyQualifiedPathNames()
	{
		// Arrange
		// noop

		// Act
		new FileSystemStorageService(String.Empty);

		// Assert by method attribute
	}

	[TestMethod]
	public void FileSystemStorageService_Constructor_DoesNotNeedStoragePathWhenUsingFullyQualifiedPathNames()
	{
		// Arrange
		// noop

		// Act
		new FileSystemStorageService(String.Empty, useFullyQualifiedPathNames: true, encryptionOptions: null);

		// Assert
		// no exception is throws
	}

	[TestMethod]
	[ExpectedException(typeof(ArgumentException))]
	public void FileSystemStorageService_Constructor_CannotUseFullyQualifiedPathNamesWithStoragePath()
	{
		// Arrange
		// noop

		// Act
		new FileSystemStorageService(@"D:\", useFullyQualifiedPathNames: true, encryptionOptions: null);

		// Assert by method attribute
	}

	[TestMethod]
	public void FileSystemStorageService_Exists_ReturnsFalseWhenNotFound()
	{
		FileStorageServiceTestHelpers.FileStorageService_Exists_ReturnsFalseWhenNotFound(GetFileSystemStorageService());
	}

	[TestMethod]
	public async Task FileSystemStorageService_ExistsAsync_ReturnsFalseWhenNotFound()
	{
		await FileStorageServiceTestHelpers.FileStorageService_ExistsAsync_ReturnsFalseWhenNotFound(GetFileSystemStorageService());
	}

	[TestMethod]
	public void FileSystemStorageService_Exists_ReturnsTrueWhenExists()
	{
		FileStorageServiceTestHelpers.FileStorageService_Exists_ReturnsTrueWhenExists(GetFileSystemStorageService());
	}

	[TestMethod]
	public async Task FileSystemStorageService_ExistsAsync_ReturnsTrueWhenExists()
	{
		await FileStorageServiceTestHelpers.FileStorageService_ExistsAsync_ReturnsTrueWhenExists(GetFileSystemStorageService());
	}

	[TestMethod]
	[ExpectedException(typeof(InvalidOperationException))]
	public void FileSystemStorageService_SaveDoesNotAcceptSeekedStream()
	{
		FileStorageServiceTestHelpers.FileStorageService_Save_DoNotAcceptSeekedStream(GetFileSystemStorageService());
	}

	[TestMethod]
	[ExpectedException(typeof(InvalidOperationException))]
	public async Task FileSystemStorageService_SaveAsyncDoesNotAcceptSeekedStream()
	{
		await FileStorageServiceTestHelpers.FileStorageService_SaveAsyncDoNotAcceptSeekedStream(GetFileSystemStorageService());
	}

	[TestMethod]
	public void FileSystemStorageService_SavedAndReadContentsAreSame()
	{
		FileStorageServiceTestHelpers.FileStorageService_SavedAndReadContentsAreSame_Perform(GetFileSystemStorageService());
	}

	[TestMethod]
	public async Task FileSystemStorageService_SavedAndReadContentsAreSame_Async()
	{
		await FileStorageServiceTestHelpers.FileStorageService_SavedAndReadContentsAreSame_PerformAsync(GetFileSystemStorageService());
	}

	[TestMethod]
	public void FileSystemStorageService_Save_AcceptsPathWithNewSubfolders()
	{
		FileStorageServiceTestHelpers.FileStorageService_Save_AcceptsPathWithNewSubfolders(GetFileSystemStorageService());
	}

	[TestMethod]
	public async Task FileSystemStorageService_SaveAsync_AcceptsPathWithNewSubfolders()
	{
		await FileStorageServiceTestHelpers.FileStorageService_SaveAsync_AcceptsPathWithNewSubfolders(GetFileSystemStorageService());
	}

	[TestMethod]
	public void FileSystemStorageService_SavedAndReadContentsWithEncryptionAreSame()
	{
		FileStorageServiceTestHelpers.FileStorageService_SavedAndReadContentsAreSame_Perform(GetFileSystemStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
	}

	[TestMethod]
	public async Task FileSystemStorageService_SavedAndReadContentsWithEncryptionAreSame_Async()
	{
		await FileStorageServiceTestHelpers.FileStorageService_SavedAndReadContentsAreSame_PerformAsync(GetFileSystemStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
	}

	[TestMethod]
	public void FileSystemStorageService_Save_OverwritesTargetFile()
	{
		FileStorageServiceTestHelpers.FileStorageService_Save_OverwritesTargetFile(GetFileSystemStorageService());
	}

	[TestMethod]
	public async Task FileSystemStorageService_SaveAsync_OverwritesTargetFile()
	{
		await FileStorageServiceTestHelpers.FileStorageService_SaveAsync_OverwritesTargetFile(GetFileSystemStorageService());
	}

	[TestMethod]
	public void FileSystemStorageService_DoesNotExistsAfterDelete()
	{
		FileStorageServiceTestHelpers.FileStorageService_DoesNotExistsAfterDelete(GetFileSystemStorageService());
	}

	[TestMethod]
	public async Task FileSystemStorageService_DoesNotExistsAfterDeleteAsync()
	{
		await FileStorageServiceTestHelpers.FileStorageService_DoesNotExistsAfterDeleteAsync(GetFileSystemStorageService());
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
	public async Task FileSystemStorageService_EnumerateFilesAsync_DoesNotContainStoragePath()
	{
		// Arrange
		string storagePath = GetStoragePath();
		FileSystemStorageService fileSystemStorageService = GetFileSystemStorageService();

		string testFilename = "test.txt";
		using (MemoryStream ms = new MemoryStream())
		{
			await fileSystemStorageService.SaveAsync(testFilename, ms, "text/plain");
		}

		// Act
		List<Havit.Services.FileStorage.FileInfo> fileInfos = await fileSystemStorageService.EnumerateFilesAsync().ToListAsync();

		// Assert 
		Assert.IsFalse(fileInfos.Any(fileInfo => fileInfo.Name.Contains(storagePath)));

		// Clean-up
		await fileSystemStorageService.DeleteAsync(testFilename);
	}

	[TestMethod]
	public void FileSystemStorageService_EnumerateFiles_SupportsSearchPattern()
	{
		FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_SupportsSearchPattern(GetFileSystemStorageService());
	}

	[TestMethod]
	public async Task FileSystemStorageService_EnumerateFilesAsync_SupportsSearchPattern()
	{
		await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_SupportsSearchPattern(GetFileSystemStorageService());
	}

	// Vyhledávání není case-sensitivní v části složky.
	//[TestMethod]
	//public void FileSystemStorageService_EnumerateFiles_SearchPatternIsCaseSensitive()
	//{
	//	FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_SearchPatternIsCaseSensitive(GetFileSystemStorageService());
	//}

	// Vyhledávání není case-sensitivní v části složky.
	//[TestMethod]
	//public async Task FileSystemStorageService_EnumerateFilesAsync_SearchPatternIsCaseSensitive()
	//{
	//	await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_SearchPatternIsCaseSensitive(GetFileSystemStorageService());
	//}


	[TestMethod]
	public void FileSystemStorageService_EnumerateFiles_SupportsSearchPatternInSubfolder()
	{
		FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_SupportsSearchPatternInSubfolder(GetFileSystemStorageService());
	}

	[TestMethod]
	public async Task FileSystemStorageService_EnumerateFilesAsync_SupportsSearchPatternInSubfolder()
	{
		await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_SupportsSearchPatternInSubfolder(GetFileSystemStorageService());
	}

	[TestMethod]
	public void FileSystemStorageService_EnumerateFiles_ReturnsEmptyOnNonExistingFolder()
	{
		FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_ReturnsEmptyOnNonExistingFolder(GetFileSystemStorageService());
	}

	[TestMethod]
	public async Task FileSystemStorageService_EnumerateFilesAsync_ReturnsEmptyOnNonExistingFolder()
	{
		await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_ReturnsEmptyOnNonExistingFolder(GetFileSystemStorageService());
	}

	[TestMethod]
	public void FileSystemStorageService_EnumerateFiles_HasLastModifiedUtc()
	{
		FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_HasLastModifiedUtc(GetFileSystemStorageService());
	}

	[TestMethod]
	public void FileSystemStorageService_EnumerateFiles_HasSize()
	{
		FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_HasSize(GetFileSystemStorageService());
	}

	[TestMethod]
	public async Task FileSystemStorageService_EnumerateFilesAsync_HasLastModifiedUtc()
	{
		await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_HasLastModifiedUtc(GetFileSystemStorageService());
	}
	[TestMethod]
	public async Task FileSystemStorageService_EnumerateFilesAsync_HasSize()
	{
		await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_HasSize(GetFileSystemStorageService());
	}

	[TestMethod]
	public void FileSystemStorageService_OpenRead_StopReadingFarBeforeEndDoesNotThrowCryptographicException()
	{
		FileStorageServiceTestHelpers.FileStorageService_OpenRead_StopReadingFarBeforeEndDoesNotThrowCryptographicException(GetFileSystemStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
	}

	[TestMethod]
	public async Task FileSystemStorageService_OpenReadAsync_StopReadingFarBeforeEndDoesNotThrowCryptographicException()
	{
		await FileStorageServiceTestHelpers.FileStorageService_OpenReadAsync_StopReadingFarBeforeEndDoesNotThrowCryptographicException(GetFileSystemStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
	}

	[TestMethod]
	public void FileSystemStorageService_OpenCreateAndOpenRead_ContentsAreSame()
	{
		FileStorageServiceTestHelpers.FileStorageService_OpenCreateAndOpenRead_ContentsAreSame(GetFileSystemStorageService());
	}

	[TestMethod]
	public async Task FileSystemStorageService_OpenCreateAsyncAndOpenReadAsync_ContentsAreSame()
	{
		await FileStorageServiceTestHelpers.FileStorageService_OpenCreateAsyncAndOpenReadAsync_ContentsAreSame(GetFileSystemStorageService());
	}

	[TestMethod]
	public void FileSystemStorageService_OpenCreateAndOpenReadWithEncryption_ContentsAreSame()
	{
		FileStorageServiceTestHelpers.FileStorageService_OpenCreateAndOpenRead_ContentsAreSame(GetFileSystemStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
	}

	[TestMethod]
	public async Task FileSystemStorageService_OpenCreateAsyncAndOpenReadAsyncWithEncryption_ContentsAreSame()
	{
		await FileStorageServiceTestHelpers.FileStorageService_OpenCreateAsyncAndOpenReadAsync_ContentsAreSame(GetFileSystemStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
	}

	[TestMethod]
	public void FileSystemStorageService_OpenCreate_OverwritesExistingFileAndContent()
	{
		FileStorageServiceTestHelpers.FileStorageService_OpenCreate_OverwritesExistingFileAndContent(GetFileSystemStorageService());
	}

	[TestMethod]
	public async Task FileSystemStorageService_OpenCreateAsync_OverwritesExistingFileAndContent()
	{
		await FileStorageServiceTestHelpers.FileStorageService_OpenCreateAsync_OverwritesExistingFileAndContent(GetFileSystemStorageService());
	}

	[TestMethod]
	public void FileSystemStorageService_Copy()
	{
		FileStorageServiceTestHelpers.FileStorageService_Copy(GetFileSystemStorageService(), GetFileSystemStorageService(secondary: true));
	}

	[TestMethod]
	public void FileSystemStorageService_Copy_SingleInstance()
	{
		FileSystemStorageService fileSystemStorageService = GetFileSystemStorageService();
		FileStorageServiceTestHelpers.FileStorageService_Copy(fileSystemStorageService, fileSystemStorageService);
	}

	[TestMethod]
	public async Task FileSystemStorageService_CopyAsync()
	{
		await FileStorageServiceTestHelpers.FileStorageService_CopyAsync(GetFileSystemStorageService(), GetFileSystemStorageService(secondary: true));
	}

	[TestMethod]
	public async Task FileSystemStorageService_CopyAsync_SingleInstance()
	{
		FileSystemStorageService fileSystemStorageService = GetFileSystemStorageService();
		await FileStorageServiceTestHelpers.FileStorageService_CopyAsync(fileSystemStorageService, fileSystemStorageService);
	}

	[TestMethod]
	public void FileSystemStorageService_Copy_OverwritesTargetFile()
	{
		FileStorageServiceTestHelpers.FileStorageService_Copy_OverwritesTargetFile(GetFileSystemStorageService(), GetFileSystemStorageService(secondary: true));
	}

	[TestMethod]
	public async Task FileSystemStorageService_CopyAsync_OverwritesTargetFile()
	{
		await FileStorageServiceTestHelpers.FileStorageService_CopyAsync_OverwritesTargetFile(GetFileSystemStorageService(), GetFileSystemStorageService(secondary: true));
	}

	[TestMethod]
	public void FileSystemStorageService_Move()
	{
		FileStorageServiceTestHelpers.FileStorageService_Move(GetFileSystemStorageService());
	}

	[TestMethod]
	public async Task FileSystemStorageService_MoveAsync()
	{
		await FileStorageServiceTestHelpers.FileStorageService_MoveAsync(GetFileSystemStorageService());
	}

	[TestMethod]
	public void FileSystemStorageService_Move_DoesNotDeleteFile()
	{
		FileSystemStorageService fileSystemStorageService = GetFileSystemStorageService();
		FileStorageServiceTestHelpers.FileStorageService_Move_DoesNotDeleteFile(fileSystemStorageService);
	}

	[TestMethod]
	public async Task FileSystemStorageService_MoveAsync_DoesNotDeleteFile()
	{
		FileSystemStorageService fileSystemStorageService = GetFileSystemStorageService();
		await FileStorageServiceTestHelpers.FileStorageService_MoveAsync_DoesNotDeleteFile(fileSystemStorageService);
	}

	[TestMethod]
	public void FileSystemStorageService_Move_OverwritesTargetFile()
	{
		FileStorageServiceTestHelpers.FileStorageService_Move_OverwritesTargetFile(GetFileSystemStorageService());
	}

	[TestMethod]
	public async Task FileSystemStorageService_MoveAsync_OverwritesTargetFile()
	{
		await FileStorageServiceTestHelpers.FileStorageService_MoveAsync_OverwritesTargetFile(GetFileSystemStorageService());
	}

	[TestMethod]
	public void FileSystemStorageService_Move_WithFileStorageService()
	{
		FileStorageServiceTestHelpers.FileStorageService_Move_WithFileStorageService(GetFileSystemStorageService(), GetFileSystemStorageService(secondary: true));
	}

	[TestMethod]
	public async Task FileSystemStorageService_MoveAsync_WithFileStorageService()
	{
		await FileStorageServiceTestHelpers.FileStorageService_MoveAsync_WithFileStorageService(GetFileSystemStorageService(), GetFileSystemStorageService(secondary: true));
	}

	[TestMethod]
	public void FileSystemStorageService_Move_WithFileStorageService_OverwritesTargetFile()
	{
		FileStorageServiceTestHelpers.FileStorageService_Move_WithFileStorageService_OverwritesTargetFile(GetFileSystemStorageService(), GetFileSystemStorageService(secondary: true));
	}

	[TestMethod]
	public async Task FileSystemStorageService_MoveAsync_WithFileStorageService_OverwritesTargetFile()
	{
		await FileStorageServiceTestHelpers.FileStorageService_MoveAsync_WithFileStorageService_OverwritesTargetFile(GetFileSystemStorageService(), GetFileSystemStorageService(secondary: true));
	}

	[TestMethod]
	public void FileSystemStorageService_GetFullPath()
	{
		// Arrange
		FileSystemStorageService fileSystemStorageService;

		// Act + Assert
		fileSystemStorageService = new FileSystemStorageService(@"C:\Data", useFullyQualifiedPathNames: false, encryptionOptions: null);
		Assert.AreEqual(@"C:\Data\abc.txt", fileSystemStorageService.GetFullPath(@"abc.txt"));
		Assert.AreEqual(@"C:\Data\A\abc.txt", fileSystemStorageService.GetFullPath(@"A\abc.txt"));

		fileSystemStorageService = new FileSystemStorageService(String.Empty, useFullyQualifiedPathNames: true, encryptionOptions: null);
		Assert.AreEqual(@"D:\abc.txt", fileSystemStorageService.GetFullPath(@"D:\abc.txt"));
		Assert.AreEqual(@"C:\Program Files\Microsoft Visual Studio\abc.txt", fileSystemStorageService.GetFullPath(@"C:\Program Files\Microsoft Visual Studio\abc.txt"));
	}

	[TestMethod]
	[ExpectedException(typeof(InvalidOperationException))]
	public void FileSystemStorageService_GetFullPath_ThrowsExceptionForDirectoryTraversal()
	{
		// Arrange

		// Act
		FileSystemStorageService fileSystemStorageService = new FileSystemStorageService(@"C:\A");
		fileSystemStorageService.GetFullPath(@"..\AB\file.txt"); //--> C:\AB\file.txt

		// Assert by method attribute
	}


	[TestMethod]
	[ExpectedException(typeof(InvalidOperationException))]
	public void FileSystemStorageService_GetFullPath_ThrowsExceptionForNonQualifiedPathNamesWhenUsingFullyQualifiedPathNames()
	{
		// Arrange
		FileSystemStorageService fileSystemStorageService = new FileSystemStorageService(null, useFullyQualifiedPathNames: true, encryptionOptions: null);

		// Act
		fileSystemStorageService.GetFullPath(@"\file.txt");

		// Assert by method attribute
	}

	[TestMethod]
	public void FileSystemStorageService_DependencyInjectionContainerIntegration()
	{
		// Arrange
		ServiceCollection services = new ServiceCollection();
		services.AddFileSystemStorageService<TestFileStorage>(System.IO.Path.GetTempPath());
		var provider = services.BuildServiceProvider();

		// Act
		var service = provider.GetService<IFileStorageService<TestFileStorage>>();

		// Assert
		Assert.IsNotNull(service);
		Assert.IsInstanceOfType(service, typeof(FileSystemStorageService<TestFileStorage>));
	}

	[TestMethod]
	public void FileSystemStorageService_IsPathFullyQualified()
	{
		// Arrange

		// Act + Assert
		Assert.IsFalse(FileSystemStorageService.IsPathFullyQualified(null), "null");
		Assert.IsFalse(FileSystemStorageService.IsPathFullyQualified(""), "");
		Assert.IsFalse(FileSystemStorageService.IsPathFullyQualified("d:"), "d:");
		Assert.IsFalse(FileSystemStorageService.IsPathFullyQualified("D:"), "D:");
		Assert.IsFalse(FileSystemStorageService.IsPathFullyQualified("d:*"), "d:*");
		Assert.IsFalse(FileSystemStorageService.IsPathFullyQualified("D:*"), "D:*");
		Assert.IsFalse(FileSystemStorageService.IsPathFullyQualified("d:abc"), "d:abc");
		Assert.IsFalse(FileSystemStorageService.IsPathFullyQualified("D:Abc"), "D:Abc");
		Assert.IsFalse(FileSystemStorageService.IsPathFullyQualified(@"\\"), @"\\");
		Assert.IsFalse(FileSystemStorageService.IsPathFullyQualified(@"\\Abc"), @"\\Abc");
		Assert.IsFalse(FileSystemStorageService.IsPathFullyQualified(@"\\abc"), @"\\abc");

		Assert.IsTrue(FileSystemStorageService.IsPathFullyQualified(@"C:\"), @"C:\");
		Assert.IsTrue(FileSystemStorageService.IsPathFullyQualified(@"c:\"), @"c:\");
		Assert.IsTrue(FileSystemStorageService.IsPathFullyQualified(@"C:\Data"), @"C:\Data");
		Assert.IsTrue(FileSystemStorageService.IsPathFullyQualified(@"c:\data"), @"c:\data");
		Assert.IsTrue(FileSystemStorageService.IsPathFullyQualified(@"C:\*"), @"C:\*");
		Assert.IsTrue(FileSystemStorageService.IsPathFullyQualified(@"c:\*"), @"c:\*");
		Assert.IsTrue(FileSystemStorageService.IsPathFullyQualified(@"\\Abc\"), @"\\Abc\\");
		Assert.IsTrue(FileSystemStorageService.IsPathFullyQualified(@"\\abc\"), @"\\abc\\");
		Assert.IsTrue(FileSystemStorageService.IsPathFullyQualified(@"\\abc\*"), @"\\abc\\*");
		Assert.IsTrue(FileSystemStorageService.IsPathFullyQualified(@"\\abc\def\ghi"), @"\\abc\def\ghi");
	}

	private static FileSystemStorageService GetFileSystemStorageService(bool secondary = false, EncryptionOptions encryptionOptions = null)
	{
		return new FileSystemStorageService(GetStoragePath(secondary), false, encryptionOptions);
	}

	private static string GetStoragePath(bool secondary = false)
	{
		return Path.Combine(System.IO.Path.GetTempPath(), secondary ? "hfwtests_secondary" : "hfwtests_primary");
	}
}
