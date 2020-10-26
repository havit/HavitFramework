using Azure.Core;
using Havit.Services.FileStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services.Azure.FileStorage
{
	/// <summary>
	/// Parametry pro AzureBlobStorageService.
	/// </summary>
	public class AzureBlobStorageServiceOptions
	{
		/// <summary>
		/// Connection string pro přístup k blob storage.
		/// Nelze použít společně s BlobStorageName a TokenCredential.
		/// </summary>
		public string BlobStorageConnectionString { get; set; }

		/// <summary>
		/// Název storage, ke kterému se přistupuje.
		/// Nelze použít s BlobStorageConnectionString. Vyžaduje nastavený TokenCredential.
		/// </summary>
		public string BlobStorageAccountName { get; set; }

		/// <summary>
		/// Název containeru, ke kterému se přistupuje.
		/// </summary>
		public string ContainerName { get; set; }

		/// <summary>
		/// Určeno pro použití s Managed Identity.
		/// Je potřeba nastavit hodnotu: new DefaultAzureCredential().
		/// Vyžaduje nastavený BlobStorageName.
		/// </summary>
		public TokenCredential TokenCredential { get;set; }
		
		/// <summary>
		/// CacheControl, která je nastavena do (CloudBlobReference.)Properties při save (uploadech) souborů.
		/// </summary>
		public string CacheControl { get; set; }

		/// <summary>
		/// Parametry šifrování file storage.
		/// </summary>
		public EncryptionOptions EncryptionOptions { get; set; }
	}
}
