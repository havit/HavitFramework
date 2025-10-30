using Azure.Core;
using Havit.Services.FileStorage;

namespace Havit.Services.Azure.FileStorage;

/// <summary>
/// Parametry pro AzureBlobStorageService.
/// </summary>
public class AzureBlobStorageServiceOptions
{
	/// <summary>
	/// Connection string nebo název storage pro přístup k blob storage.
	/// Zda jde o connection string nebo název storage se rozpoznává existencí středníku - pokud je obsažen, považuje se hodnota za connection string, pokud není obsažen, považuje se za název storage.
	/// Pokud je zadán název storage, vyžaduje se TokenCredential (pro connection string je ignorován).
	/// </summary>
	public string BlobStorage { get; set; }

	/// <summary>
	/// Název containeru, ke kterému se přistupuje.
	/// </summary>
	public string ContainerName { get; set; }

	/// <summary>
	/// Určeno pro použití s Managed Identity.
	/// Je potřeba nastavit hodnotu: new DefaultAzureCredential().
	/// Vyžaduje nastavený BlobStorageName.
	/// </summary>
	public TokenCredential TokenCredential { get; set; }

	/// <summary>
	/// CacheControl, která je nastavena do (CloudBlobReference.)Properties při save (uploadech) souborů.
	/// </summary>
	public string CacheControl { get; set; }

	/// <summary>
	/// Parametry šifrování file storage.
	/// </summary>
	public EncryptionOptions EncryptionOptions { get; set; }

	/// <summary>
	/// Indikuje, zda má automaticky zakládat BlobContainer, pokud neexistuje.
	/// </summary>
	public bool AutoCreateBlobContainer { get; set; } = true;
}
