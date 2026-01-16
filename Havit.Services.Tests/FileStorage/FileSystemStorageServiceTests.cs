using Havit.Services.FileStorage;
using Havit.Services.TestHelpers;
using Havit.Services.TestHelpers.FileStorage;
using Havit.Services.Tests.FileStorage.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Services.Tests.FileStorage;

[TestClass]
public class FileSystemStorageServiceTests
{
	private const string PrimaryDirectoryName = "hfw-primary";
	private const string SecondaryDirectoryName = "hfw-secondary";

	private const string EnumerateFilesSupportsSearchPatternDirectoryName = "hfw-enumerate1";
	private const string EnumerateFilesAsyncSupportsSearchPatternDirectoryName = "hfw-enumerate2";
	//private const string EnumerateFilesSearchPatternIsCaseSensitiveDirectoryName = "hfw-enumerate3";
	//private const string EnumerateFilesAsyncSearchPatternIsCaseSensitiveDirectoryName = "hfw-enumerate4";
	private const string EnumerateFilesSupportsSearchPatternInSubfolderDirectoryName = "hfw-enumerate5";
	private const string EnumerateFilesAsyncSupportsSearchPatternInSubfolderDirectoryName = "hfw-enumerate6";
	private const string EnumerateFilesDoesNotContainStoragePath = "hfw-enumerate7";
	private const string EnumerateFilesAsyncDoesNotContainStoragePath = "hfw-enumerate8";

	private readonly static string[] AllDirectoryNames = [
		PrimaryDirectoryName,
		SecondaryDirectoryName,
		EnumerateFilesSupportsSearchPatternDirectoryName,
		EnumerateFilesAsyncSupportsSearchPatternDirectoryName,
		EnumerateFilesSupportsSearchPatternInSubfolderDirectoryName,
		EnumerateFilesAsyncSupportsSearchPatternInSubfolderDirectoryName,
		EnumerateFilesDoesNotContainStoragePath,
		EnumerateFilesAsyncDoesNotContainStoragePath
	];

	public TestContext TestContext { get; set; }

	[ClassInitialize]
	public static void Initialize(TestContext _)
	{
		// testy jsou slušné, mažou po sobě
		// ve scénáři, kdy testy procházejí, není nutno tedy čistit před každým testem, ale čistíme pouze preventivně před všemi testy
		CleanUp();
		Parallel.ForEach(AllDirectoryNames, directoryName =>
		{
			Directory.CreateDirectory(GetStoragePath(directoryName));
		});
	}

	[ClassCleanup]
	public static void CleanUp()
	{
		Parallel.ForEach(AllDirectoryNames, directoryName =>
		{
			string path = GetStoragePath(directoryName);
			if (Directory.Exists(path))
			{
				Directory.Delete(path, true);
			}
		});
	}

	[TestMethod]
	public void FileSystemStorageService_Constructor_ReplacesTEMP()
	{
		// Arrange
		// noop

		// Act
		var fileSystemStorageService = new FileSystemStorageService(@"%TEMP%\SomeFolder");

		// Assert
		Assert.DoesNotContain("%TEMP%", fileSystemStorageService.StoragePath);
		Assert.DoesNotContain(@"\\", fileSystemStorageService.StoragePath); // neobsahuje dvě zpětná lomítka za sebou
	}

	[TestMethod]
	public void FileSystemStorageService_Constructor_NeedsStoragePathWhenNotUsingFullyQualifiedPathNames()
	{
		// Arrange
		// noop

		// Assert
		Assert.ThrowsExactly<ArgumentException>(() =>
		{
			// Act
			new FileSystemStorageService(String.Empty);
		});
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
	public void FileSystemStorageService_Constructor_CannotUseFullyQualifiedPathNamesWithStoragePath()
	{
		// Arrange
		// noop

		// Assert
		Assert.ThrowsExactly<ArgumentException>(() =>
		{
			// Act
			new FileSystemStorageService(@"D:\", useFullyQualifiedPathNames: true, encryptionOptions: null);
		});
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
	public void FileSystemStorageService_SaveDoesNotAcceptSeekedStream()
	{
		FileStorageServiceTestHelpers.FileStorageService_Save_DoNotAcceptSeekedStream(GetFileSystemStorageService());
	}

	[TestMethod]
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
		FileStorageServiceTestHelpers.FileStorageService_SavedAndReadContentsAreSame_Perform(GetFileSystemStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())), isEncrypted: true);
	}

	[TestMethod]
	public async Task FileSystemStorageService_SavedAndReadContentsWithEncryptionAreSame_Async()
	{
		await FileStorageServiceTestHelpers.FileStorageService_SavedAndReadContentsAreSame_PerformAsync(GetFileSystemStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())), isEncrypted: true);
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
		string storagePath = GetStoragePath(EnumerateFilesDoesNotContainStoragePath);
		FileSystemStorageService fileSystemStorageService = GetFileSystemStorageService(EnumerateFilesDoesNotContainStoragePath);

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
		string storagePath = GetStoragePath(EnumerateFilesAsyncDoesNotContainStoragePath);
		FileSystemStorageService fileSystemStorageService = GetFileSystemStorageService(EnumerateFilesAsyncDoesNotContainStoragePath);

		string testFilename = "test.txt";
		using (MemoryStream ms = new MemoryStream())
		{
			await fileSystemStorageService.SaveAsync(testFilename, ms, "text/plain", TestContext.CancellationToken);
		}

		// Act
		List<Havit.Services.FileStorage.FileInfo> fileInfos = await fileSystemStorageService.EnumerateFilesAsync(cancellationToken: TestContext.CancellationToken).ToListAsync();

		// Assert 
		Assert.IsFalse(fileInfos.Any(fileInfo => fileInfo.Name.Contains(storagePath)));

		// Clean-up
		await fileSystemStorageService.DeleteAsync(testFilename, TestContext.CancellationToken);
	}

	[TestMethod]
	public void FileSystemStorageService_EnumerateFiles_SupportsSearchPattern()
	{
		FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_SupportsSearchPattern(GetFileSystemStorageService(EnumerateFilesSupportsSearchPatternDirectoryName));
	}

	[TestMethod]
	public async Task FileSystemStorageService_EnumerateFilesAsync_SupportsSearchPattern()
	{
		await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_SupportsSearchPattern(GetFileSystemStorageService(EnumerateFilesAsyncSupportsSearchPatternDirectoryName));
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
		FileStorageServiceTestHelpers.FileStorageService_EnumerateFiles_SupportsSearchPatternInSubfolder(GetFileSystemStorageService(EnumerateFilesSupportsSearchPatternInSubfolderDirectoryName));
	}

	[TestMethod]
	public async Task FileSystemStorageService_EnumerateFilesAsync_SupportsSearchPatternInSubfolder()
	{
		await FileStorageServiceTestHelpers.FileStorageService_EnumerateFilesAsync_SupportsSearchPatternInSubfolder(GetFileSystemStorageService(EnumerateFilesAsyncSupportsSearchPatternInSubfolderDirectoryName));
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
	public void FileSystemStorageService_GetLastModifiedTimeUtc_ThrowsFileNotFoundExceptionForNonExistingFile()
	{
		FileStorageServiceTestHelpers.FileStorageService_GetLastModifiedTimeUtc_ThrowsFileNotFoundExceptionForNonExistingFile(GetFileSystemStorageService());
	}

	[TestMethod]
	public async Task FileSystemStorageService_GetLastModifiedTimeUtcAsync_ThrowsFileNotFoundExceptionForNonExistingFile()
	{
		await FileStorageServiceTestHelpers.FileStorageService_GetLastModifiedTimeUtcAsync_ThrowsFileNotFoundExceptionForNonExistingFile(GetFileSystemStorageService());
	}

	[TestMethod]
	public void FileSystemStorageService_OpenRead_StopReadingFarBeforeEndDoesNotThrowCryptographicException()
	{
		FileStorageServiceTestHelpers.FileStorageService_OpenRead_StopReadingFarBeforeEndDoesNotThrowCryptographicException(GetFileSystemStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
	}

	[TestMethod]
	public void FileSystemStorageService_OpenRead_ThrowsFileNotFoundExceptionForNonExistingFile()
	{
		FileStorageServiceTestHelpers.FileStorageService_OpenRead_ThrowsFileNotFoundExceptionForNonExistingFile(GetFileSystemStorageService());
	}

	[TestMethod]
	public async Task FileSystemStorageService_OpenReadAsync_StopReadingFarBeforeEndDoesNotThrowCryptographicException()
	{
		await FileStorageServiceTestHelpers.FileStorageService_OpenReadAsync_StopReadingFarBeforeEndDoesNotThrowCryptographicException(GetFileSystemStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
	}

	[TestMethod]
	public async Task FileSystemStorageService_OpenReadAsync_ThrowsFileNotFoundExceptionForNonExistingFile()
	{
		await FileStorageServiceTestHelpers.FileStorageService_OpenReadAsync_ThrowsFileNotFoundExceptionForNonExistingFile(GetFileSystemStorageService());
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
		FileStorageServiceTestHelpers.FileStorageService_OpenCreateAndOpenRead_ContentsAreSame(GetFileSystemStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())), isEncrypted: true);
	}

	[TestMethod]
	public async Task FileSystemStorageService_OpenCreateAsyncAndOpenReadAsyncWithEncryption_ContentsAreSame()
	{
		await FileStorageServiceTestHelpers.FileStorageService_OpenCreateAsyncAndOpenReadAsync_ContentsAreSame(GetFileSystemStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())), isEncrypted: true);
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
	public void FileSystemStorageService_ReadToStream_ThrowsFileNotFoundExceptionForNonExistingFile()
	{
		FileStorageServiceTestHelpers.FileStorageService_ReadToStream_ThrowsFileNotFoundExceptionForNonExistingFile(GetFileSystemStorageService());
	}

	[TestMethod]
	public async Task FileSystemStorageService_ReadToStreamAsync_ThrowsFileNotFoundExceptionForNonExistingFile()
	{
		await FileStorageServiceTestHelpers.FileStorageService_ReadToStreamAsync_ThrowsFileNotFoundExceptionForNonExistingFile(GetFileSystemStorageService(encryptionOptions: new AesEncryptionOption(AesEncryptionOption.CreateRandomKeyAndIvAsBase64String())));
	}

	[TestMethod]
	public void FileSystemStorageService_Copy()
	{
		FileStorageServiceTestHelpers.FileStorageService_Copy(GetFileSystemStorageService(), GetFileSystemStorageService(secondary: true), suffix: "multiple");
	}

	[TestMethod]
	public void FileSystemStorageService_Copy_SingleInstance()
	{
		FileSystemStorageService fileSystemStorageService = GetFileSystemStorageService();
		FileStorageServiceTestHelpers.FileStorageService_Copy(fileSystemStorageService, fileSystemStorageService, suffix: "single");
	}

	[TestMethod]
	public async Task FileSystemStorageService_CopyAsync()
	{
		await FileStorageServiceTestHelpers.FileStorageService_CopyAsync(GetFileSystemStorageService(), GetFileSystemStorageService(secondary: true), suffix: "multiple");
	}

	[TestMethod]
	public async Task FileSystemStorageService_CopyAsync_SingleInstance()
	{
		FileSystemStorageService fileSystemStorageService = GetFileSystemStorageService();
		await FileStorageServiceTestHelpers.FileStorageService_CopyAsync(fileSystemStorageService, fileSystemStorageService, suffix: "single");
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
	public void FileSystemStorageService_GetFullPath_ThrowsExceptionForDirectoryTraversal()
	{
		// Arrange

		FileSystemStorageService fileSystemStorageService = new FileSystemStorageService(@"C:\A");

		// Assert
		Assert.ThrowsExactly<InvalidOperationException>(() =>
		{
			// Act
			fileSystemStorageService.GetFullPath(@"..\AB\file.txt"); //--> C:\AB\file.txt
		});
	}


	[TestMethod]
	public void FileSystemStorageService_GetFullPath_ThrowsExceptionForNonQualifiedPathNamesWhenUsingFullyQualifiedPathNames()
	{
		// Arrange
		FileSystemStorageService fileSystemStorageService = new FileSystemStorageService(null, useFullyQualifiedPathNames: true, encryptionOptions: null);

		// Assert
		Assert.ThrowsExactly<InvalidOperationException>(() =>
		{
			// Act
			fileSystemStorageService.GetFullPath(@"\file.txt");
		});
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
		return GetFileSystemStorageService(secondary ? PrimaryDirectoryName : SecondaryDirectoryName, encryptionOptions);
	}

	public static FileSystemStorageService GetFileSystemStorageService(string directoryName, EncryptionOptions encryptionOptions = null)
	{
		return new FileSystemStorageService(GetStoragePath(directoryName), false, encryptionOptions);
	}

	private static string GetStoragePath(string directoryName)
	{
		return Path.Combine(System.IO.Path.GetTempPath(), directoryName + "-" + FileStorageServiceTestHelpers.GetTestingScope().ToLower());
	}
}
