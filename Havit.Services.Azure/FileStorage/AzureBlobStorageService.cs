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
using Azure.Core;

namespace Havit.Services.Azure.FileStorage
{
	/// <summary>
	/// Úložiště souborů jako Azure Blob Storage.
	/// Podporuje client-side šifrování (z předka FileStorageServiceBase).
	/// </summary>
	/// <remarks>
	/// Negenerickou třídu držíme pro zpětnou kompatibilitu.
	/// </remarks>
	public class AzureBlobStorageService : FileStorageServiceBase, IFileStorageService
	{
		private readonly AzureBlobStorageServiceOptions options;

		private volatile bool containerAlreadyCreated = false;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="blobStorageConnectionString">Connection string pro připojení k Azure Blob Storage.</param>
		/// <param name="containerName">Container v Blob Storage pro práci se soubory.</param>
		public AzureBlobStorageService(string blobStorageConnectionString, string containerName) : this(blobStorageConnectionString, containerName, null)
		{
		}

		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="blobStorageConnectionString">Connection string pro připojení k Azure Blob Storage.</param>
		/// <param name="containerName">Container v Blob Storage pro práci se soubory.</param>
		/// <param name="encryptionOptions">Parametry šifrování.</param>
		public AzureBlobStorageService(string blobStorageConnectionString, string containerName, EncryptionOptions encryptionOptions) : this(new AzureBlobStorageServiceOptions
		{
			BlobStorageConnectionString = blobStorageConnectionString,
			ContainerName = containerName,
			EncryptionOptions = encryptionOptions
		})
		{
			// NOOP
		}

		/// <summary>
		/// Konstruktor.
		/// Bez šifrování.
		/// Bez použití IOptions&lt;&gt;, volitelně v případě potřeby doplníme.
		/// </summary>
		/// <param name="options">Konfigurace služby.</param>
		public AzureBlobStorageService(AzureBlobStorageServiceOptions options) : base(options?.EncryptionOptions)
		{
			Contract.Requires<ArgumentNullException>(options != null, nameof(options));
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(options.BlobStorageConnectionString) || !String.IsNullOrEmpty(options.BlobStorageAccountName), $"{nameof(options.BlobStorageConnectionString)} or {nameof(options.BlobStorageAccountName)} must be specified.");
			Contract.Requires<ArgumentException>(String.IsNullOrEmpty(options.BlobStorageConnectionString) || String.IsNullOrEmpty(options.BlobStorageAccountName), $"Cannot specify both {nameof(options.BlobStorageConnectionString)} or {nameof(options.BlobStorageAccountName)}.");
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(options.ContainerName), nameof(options.ContainerName));

			// contracts v této podobě se obtížně čtou, proto alternativně
			if (String.IsNullOrEmpty(options.BlobStorageAccountName) && (options.TokenCredential != null))
			{
				throw new InvalidOperationException($"{nameof(options.TokenCredential)} can be used only when ${nameof(options.BlobStorageAccountName)} is specified.");
			}

			if (!String.IsNullOrEmpty(options.BlobStorageAccountName) && (options.TokenCredential == null))
			{
				throw new InvalidOperationException($"When ${nameof(options.BlobStorageAccountName)} is specified there must be specified also {nameof(options.TokenCredential)}");
			}

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
			if (!String.IsNullOrEmpty(options.CacheControl))
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
			if (!String.IsNullOrEmpty(options.BlobStorageConnectionString))
			{
				return new BlobContainerClient(options.BlobStorageConnectionString, options.ContainerName);
			}
			else
			{
				// pro podporu Managed Identity
				string containerEndpoint = $"https://{options.BlobStorageAccountName}.blob.core.windows.net/{options.ContainerName}";
				return new BlobContainerClient(new Uri(containerEndpoint), options.TokenCredential);
			}
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
