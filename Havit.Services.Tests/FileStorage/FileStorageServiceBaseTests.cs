using Havit.Services.FileStorage;

namespace Havit.Services.Tests.FileStorage;

[TestClass]
public class FileStorageServiceBaseTests
{
	[TestMethod]
	public void FileStorageServiceBase_EnumerableFilesGetPrefix_CorrectlyGetPrefix()
	{
		// Act + Assert
		Assert.IsNull(FileStorageServiceBase.EnumerableFilesGetPrefix(null));
		Assert.IsNull(FileStorageServiceBase.EnumerableFilesGetPrefix(""));
		Assert.IsNull(FileStorageServiceBase.EnumerableFilesGetPrefix("test.txt"));
		Assert.AreEqual(String.Empty, FileStorageServiceBase.EnumerableFilesGetPrefix("/test.*"));
		Assert.AreEqual(String.Empty, FileStorageServiceBase.EnumerableFilesGetPrefix("/test.txt"));

		Assert.AreEqual("SubFolder1/Subfolder2", FileStorageServiceBase.EnumerableFilesGetPrefix("SubFolder1/Subfolder2/test.txt"));
		Assert.AreEqual("SubFolder1/Subfolder2", FileStorageServiceBase.EnumerableFilesGetPrefix("SubFolder1/Subfolder2/*.txt"));
		Assert.AreEqual("SubFolder1/Subfolder2", FileStorageServiceBase.EnumerableFilesGetPrefix("SubFolder1/Subfolder2/t*.txt"));
		Assert.AreEqual("SubFolder1", FileStorageServiceBase.EnumerableFilesGetPrefix("SubFolder1/Sub*/*.txt"));
		Assert.IsNull(FileStorageServiceBase.EnumerableFilesGetPrefix("Sub*/Sub*/*.txt"));
		Assert.AreEqual("Subfolder1", FileStorageServiceBase.EnumerableFilesGetPrefix("Subfolder1/Sub*/test.txt"));
		Assert.IsNull(FileStorageServiceBase.EnumerableFilesGetPrefix("Sub*/Sub/test.txt"));
		Assert.AreEqual(String.Empty, FileStorageServiceBase.EnumerableFilesGetPrefix("/Sub*/test.txt"));

		Assert.AreEqual("SubFolder1/Subfolder2", FileStorageServiceBase.EnumerableFilesGetPrefix("SubFolder1/Subfolder2/??.txt"));
		Assert.AreEqual("SubFolder1/Subfolder2", FileStorageServiceBase.EnumerableFilesGetPrefix("SubFolder1/Subfolder2/t??.txt"));
		Assert.AreEqual("SubFolder1", FileStorageServiceBase.EnumerableFilesGetPrefix("SubFolder1/Sub??/??.txt"));
		Assert.IsNull(FileStorageServiceBase.EnumerableFilesGetPrefix("Sub??/Sub??/??.txt"));
		Assert.AreEqual("Subfolder1", FileStorageServiceBase.EnumerableFilesGetPrefix("Subfolder1/Sub??/test.txt"));
		Assert.IsNull(FileStorageServiceBase.EnumerableFilesGetPrefix("Sub??/Sub/test.txt"));
	}
}
