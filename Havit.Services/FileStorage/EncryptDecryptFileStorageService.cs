using Havit.Diagnostics.Contracts;

namespace Havit.Services.FileStorage;

/// <summary>
/// Zašifruje či dešifruje všechny soubory v úložišti.
/// </summary>
/// <remarks>
/// URČENO PRO ŠIFROVÁNÍ SOUBORŮ NA CHESTERU.
/// </remarks>
public class EncryptDecryptFileStorageService
{
	/// <summary>
	/// Zašifruje všechny soubory v úložišti.
	/// </summary>
	/// <remarks>
	/// URČENO PRO ZAŠIFROVÁNÍ SOUBORŮ NA CHESTERU.
	/// </remarks>
	public void EncryptAllFiles(IFileStorageService sourceDecryptedStorageService, IFileStorageService targetEncryptedStorageService)
	{
		Contract.Requires(sourceDecryptedStorageService != null);
		Contract.Requires(targetEncryptedStorageService != null);
		Contract.Requires(sourceDecryptedStorageService is FileStorageServiceBase);
		Contract.Requires(targetEncryptedStorageService is FileStorageServiceBase);
		Contract.Requires(!(sourceDecryptedStorageService as FileStorageServiceBase).SupportsBasicEncryption, "Zdrojový storage nesmí podporovat (nesmí mít zapnuté) šifrování.");
		Contract.Requires((targetEncryptedStorageService as FileStorageServiceBase).SupportsBasicEncryption, "Cílový storage musí podporovat (mít zapnuté) šifrování.");

		EncryptDecryptAllFiles(sourceDecryptedStorageService, targetEncryptedStorageService);
	}

	/// <summary>
	/// Dešifruje všechny soubory v úložišti.
	/// </summary>
	/// <remarks>
	/// URČENO PRO ZAŠIFROVÁNÍ SOUBORŮ NA CHESTERU.
	/// </remarks>
	public void DecryptAllFiles(IFileStorageService sourceEncryptedStorageService, IFileStorageService targetDecryptedStorageService)
	{
		Contract.Requires(sourceEncryptedStorageService != null);
		Contract.Requires(targetDecryptedStorageService != null);
		Contract.Requires(sourceEncryptedStorageService is FileStorageServiceBase);
		Contract.Requires(targetDecryptedStorageService is FileStorageServiceBase);
		Contract.Requires((sourceEncryptedStorageService as FileStorageServiceBase).SupportsBasicEncryption, "Zdrojový storage musí podporovat (musí mít zapnuté) šifrování.");
		Contract.Requires(!(targetDecryptedStorageService as FileStorageServiceBase).SupportsBasicEncryption, "Cílový storage nesmí podporovat (nesmí mít zapnuté) šifrování.");

		EncryptDecryptAllFiles(sourceEncryptedStorageService, targetDecryptedStorageService);
	}

	private static void EncryptDecryptAllFiles(IFileStorageService sourceStorageService, IFileStorageService targetStorageService)
	{
		List<string> completedFiles = new List<string>();

		FileInfo[] filesToProcess = sourceStorageService
			.EnumerateFiles()
			.OrderBy(item => item.Name)
			.ToArray();

		try
		{
			foreach (FileInfo file in filesToProcess)
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					sourceStorageService.ReadToStream(file.Name, memoryStream);
					memoryStream.Seek(0, SeekOrigin.Begin);
					targetStorageService.Save(file.Name, memoryStream, file.ContentType);
				}
				completedFiles.Add(file.Name);
			}
		}
		catch (Exception exception)
		{
			throw new EncryptDecryptFilesException("All files encryption/decryption failed.", exception, completedFiles.ToArray(), filesToProcess.Select(item => item.Name).ToArray());
		}
	}
}
