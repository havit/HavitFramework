using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Havit.Services.FileStorage;
using FileInfo = Havit.Services.FileStorage.FileInfo;
using Havit.Diagnostics.Contracts;
using Havit.Text.RegularExpressions;
using Azure.Storage.Blobs;
using Azure;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Azure;

namespace Havit.Services.Azure.FileStorage
{
	/// <summary>
	/// Úložiště souborů jako Azure Blob Storage.
	/// Podporuje šifrování a to jak transparentní šifrování z předka (FileStorageServiceBase), 
	/// tak šifrování vestavěné v Azure Storage klientu (https://azure.microsoft.com/en-us/documentation/articles/storage-client-side-encryption/).
	/// 
	/// Pro jednoduché šifrování se používá konstruktor s encryptionOptions (EncryptionOptions),
	/// pro šifrování pomocí Azure Storage klienta se použije kontruktor s encyptionPolicy (BlobEnctyptionPolicy).
	/// 	
	/// </summary>
	public class AzureBlobStorageService : FileStorageServiceBase, IFileStorageService, IFileStorageServiceAsync
	{
		private readonly string blobStorageConnectionString;
		private readonly string containerName;

		private readonly AzureBlobStorageServiceOptions options;

		private volatile bool containerAlreadyCreated = false;

		/// <summary>
		/// Konstruktor. Služba nebude šifrovat obsah.
		/// </summary>
		/// <param name="blobStorageConnectionString">Connection string pro připojení k Azure Blob Storage.</param>
		/// <param name="containerName">Container v Blob Storage pro práci se soubory.</param>
		public AzureBlobStorageService(string blobStorageConnectionString, string containerName) : this(blobStorageConnectionString, containerName, null, null)
		{
		}

		/// <summary>
		/// Konstruktor. Služba nebude šifrovat obsah.
		/// </summary>
		/// <param name="blobStorageConnectionString">Connection string pro připojení k Azure Blob Storage.</param>
		/// <param name="containerName">Container v Blob Storage pro práci se soubory.</param>
		/// <param name="options">Další nastavení.</param>
		public AzureBlobStorageService(string blobStorageConnectionString, string containerName, AzureBlobStorageServiceOptions options) : this(blobStorageConnectionString, containerName, options, null)
		{
		}		
		
		/// <summary>
		/// Konstruktor. Služba bude šifrovat obsah vlastní implementací.
		/// </summary>
		/// <param name="blobStorageConnectionString">Connection string pro připojení k Azure Blob Storage.</param>
		/// <param name="containerName">Container v Blob Storage pro práci se soubory.</param>
		/// <param name="encryptionOptions">Parametry šifrování.</param>
		public AzureBlobStorageService(string blobStorageConnectionString, string containerName, EncryptionOptions encryptionOptions) : this(blobStorageConnectionString, containerName, null, encryptionOptions)
		{			
		}

		/// <summary>
		/// Konstruktor. Služba bude šifrovat obsah vlastní implementací.
		/// </summary>
		/// <param name="blobStorageConnectionString">Connection string pro připojení k Azure Blob Storage.</param>
		/// <param name="containerName">Container v Blob Storage pro práci se soubory.</param>
		/// <param name="encryptionOptions">Parametry šifrování.</param>
		/// <param name="options">Další nastavení.</param>
		public AzureBlobStorageService(string blobStorageConnectionString, string containerName, AzureBlobStorageServiceOptions options, EncryptionOptions encryptionOptions) : base(encryptionOptions)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(blobStorageConnectionString), nameof(blobStorageConnectionString));
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(containerName), nameof(containerName));

			this.blobStorageConnectionString = blobStorageConnectionString;
			this.containerName = containerName;
			this.options = options;
		}

		/// <summary>
		/// Vrátí true, pokud uložený soubor v úložišti existuje. Jinak false.
		/// </summary>
		public override bool Exists(string fileName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName), nameof(fileName));

			BlobClient blobClient = GetBlobClient(fileName);
			return blobClient.Exists();
		}

		/// <summary>
		/// Vrátí true, pokud uložený soubor v úložišti existuje. Jinak false.
		/// </summary>
		public override async Task<bool> ExistsAsync(string fileName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName), nameof(fileName));

			BlobClient blobClient = GetBlobClient(fileName);
			return await blobClient.ExistsAsync().ConfigureAwait(false);
		}

		/// <summary>
		/// Zapíše obsah souboru z úložiště do streamu.
		/// </summary>
		protected override void PerformReadToStream(string fileName, Stream stream)
		{
			BlobClient blobClient = GetBlobClient(fileName);
			blobClient.DownloadTo(stream);
		}

		/// <summary>
		/// Zapíše obsah souboru z úložiště do streamu.
		/// </summary>
		protected override async Task PerformReadToStreamAsync(string fileName, Stream stream)
		{
			BlobClient blobClient = GetBlobClient(fileName);
			await blobClient.DownloadToAsync(stream).ConfigureAwait(false);
		}

		/// <summary>
		/// Vrátí stream s obsahem soubor z úložiště.
		/// </summary>
		protected override Stream PerformRead(string fileName)
		{
			BlobClient blobClient = GetBlobClient(fileName);
			BlobDownloadInfo downloadInfo = blobClient.Download();
			return downloadInfo.Content;		
		}

		/// <summary>
		/// Vrátí stream s obsahem soubor z úložiště.
		/// </summary>
		protected override async Task<Stream> PerformReadAsync(string fileName)
		{
			BlobClient blobClient = GetBlobClient(fileName);
			BlobDownloadInfo downloadInfo = await blobClient.DownloadAsync().ConfigureAwait(false);
			return downloadInfo.Content;
		}

		/// <summary>
		/// Uloží stream do úložiště.
		/// </summary>
		protected override void PerformSave(string fileName, Stream fileContent, string contentType)
		{
			EnsureContainer();

			BlobClient blobClient = GetBlobClient(fileName);

			BlobHttpHeaders blobHttpHeaders = new BlobHttpHeaders();
			blobHttpHeaders.ContentType = contentType;
			PerformSave_SetProperties(blobHttpHeaders);

			blobClient.Upload(fileContent, blobHttpHeaders);

		}

		/// <summary>
		/// Uloží stream do úložiště.
		/// </summary>
		protected override async Task PerformSaveAsync(string fileName, Stream fileContent, string contentType)
		{
			await EnsureContainerAsync().ConfigureAwait(false);

			BlobClient blobClient = GetBlobClient(fileName);

			BlobHttpHeaders blobHttpHeaders = new BlobHttpHeaders();
			blobHttpHeaders.ContentType = contentType;
			PerformSave_SetProperties(blobHttpHeaders);

			await blobClient.UploadAsync(fileContent, blobHttpHeaders).ConfigureAwait(false);
		}

		private void PerformSave_SetProperties(BlobHttpHeaders blobHttpHeaders)
		{
			if (!String.IsNullOrEmpty(options?.CacheControl))
			{
				blobHttpHeaders.CacheControl = options.CacheControl;
			}
		}

		/// <summary>
		/// Smaže soubor v úložišti.
		/// </summary>
		public override void Delete(string fileName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName));

			BlobClient blobClient = GetBlobClient(fileName);
			blobClient.Delete();			
		}

		/// <summary>
		/// Smaže soubor v úložišti.
		/// </summary>
		public override async Task DeleteAsync(string fileName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName));

			BlobClient blobClient = GetBlobClient(fileName);
			await blobClient.DeleteAsync().ConfigureAwait(false);
		}

		/// <summary>
		/// Vylistuje seznam souborů v úložišti.
		/// </summary>
		public override IEnumerable<FileInfo> EnumerateFiles(string searchPattern = null)
		{
			if (!String.IsNullOrWhiteSpace(searchPattern))
			{
				// zamen souborova '\\' za '/', ktere lze pouzit v Azure blobu
				searchPattern = searchPattern.Replace("\\", "/");
			}

			// ziskej prefix, uvodni cast cesty, ve kterem nejsou pouzite znaky '*' a '?'
			string prefix = FileStorageServiceBase.EnumerableFilesGetPrefix(searchPattern);

			EnsureContainer();

			// nacti soubory s danym prefixem - optimalizace na rychlost
			Pageable<BlobItem> blobItems = GetBlobContainerClient().GetBlobs(prefix: prefix);

			// filtruj soubory podle masky
			foreach (var blobItem in blobItems)
			{
				if (EnumerateFiles_FilterCloudBlob(blobItem, searchPattern))
				{
					yield return EnumerateFiles_ProjectCloudBlob(blobItem);
				}
			}
		}

		/// <summary>
		/// Vylistuje seznam souborů v úložišti.
		/// </summary>
		public override async IAsyncEnumerable<FileInfo> EnumerateFilesAsync(string searchPattern = null)
		{
			if (!String.IsNullOrWhiteSpace(searchPattern))
			{
				// zamen souborova '\\' za '/', ktere lze pouzit v Azure blobu
				searchPattern = searchPattern.Replace("\\", "/");
			}

			// ziskej prefix, uvodni cast cesty, ve kterem nejsou pouzite znaky '*' a '?'
			string prefix = FileStorageServiceBase.EnumerableFilesGetPrefix(searchPattern);

			await EnsureContainerAsync().ConfigureAwait(false);

			// nacti soubory s danym prefixem - optimalizace na rychlost
			AsyncPageable<BlobItem> blobItems = GetBlobContainerClient().GetBlobsAsync(prefix: prefix);

			await foreach (BlobItem blobItem in blobItems.ConfigureAwait(false))
			{
				// filtruj soubory podle masky
				if (EnumerateFiles_FilterCloudBlob(blobItem, searchPattern))
				{
					yield return EnumerateFiles_ProjectCloudBlob(blobItem);
				}
			}
		}

		private bool EnumerateFiles_FilterCloudBlob(BlobItem blobItem, string searchPattern)
		{
			if ((searchPattern != null) && !RegexPatterns.IsFileWildcardMatch(blobItem.Name, searchPattern))
			{
				return false;
			}
			
			return true;
		}

		private FileInfo EnumerateFiles_ProjectCloudBlob(BlobItem blobItem)
		{
			return new FileInfo
			{
				Name = blobItem.Name,
				LastModifiedUtc = blobItem.Properties.LastModified?.UtcDateTime ?? default(DateTime),
				Size = blobItem.Properties.ContentLength ?? -1,
				ContentType = blobItem.Properties.ContentType
			};
		}

		/// <summary>
		/// Vrátí čas poslední modifikace souboru v UTC timezone.
		/// </summary>
		public override DateTime? GetLastModifiedTimeUtc(string fileName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName));

			BlobClient blobClient = GetBlobClient(fileName);
			BlobProperties properties = blobClient.GetProperties();
			return properties.LastModified.UtcDateTime;
		}

		/// <summary>
		/// Vrátí čas poslední modifikace souboru v UTC timezone.
		/// </summary>
		public override async Task<DateTime?> GetLastModifiedTimeUtcAsync(string fileName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName));

			BlobClient blobClient = GetBlobClient(fileName);
			BlobProperties properties = await blobClient.GetPropertiesAsync().ConfigureAwait(false);
			return properties.LastModified.UtcDateTime;
		}

		/// <summary>
		/// Vrátí BlobClient pro daný blob v containeru používaného Azure Storage Accountu.
		/// </summary>
		protected internal BlobClient GetBlobClient(string blobName)
		{
			BlobContainerClient blobContainerClient = GetBlobContainerClient();
			return blobContainerClient.GetBlobClient(blobName);
		}

		/// <summary>
		/// Vrátí používaný container (BlobContainerClient) v Azure Storage Accountu.
		/// </summary>
		protected internal BlobContainerClient GetBlobContainerClient()
		{
			return new BlobContainerClient(blobStorageConnectionString, containerName);
		}

		/// <summary>
		/// Vytvoří kontejner, pokud ještě neexistuje.
		/// </summary>
		protected void EnsureContainer()
		{
			if (!containerAlreadyCreated)
			{
				GetBlobContainerClient().CreateIfNotExists(PublicAccessType.None);
				containerAlreadyCreated = true;
			}
		}

		/// <summary>
		/// Vytvoří kontejner, pokud ještě neexistuje.
		/// </summary>
		protected async Task EnsureContainerAsync()
		{
			if (!containerAlreadyCreated)
			{
				await GetBlobContainerClient().CreateIfNotExistsAsync(PublicAccessType.None).ConfigureAwait(false);
				containerAlreadyCreated = true;
			}
		}
	}
}
