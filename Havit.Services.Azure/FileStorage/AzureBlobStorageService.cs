using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Havit.Services.FileStorage;
using FileInfo = Havit.Services.FileStorage.FileInfo;
using Havit.Diagnostics.Contracts;
using Havit.Text.RegularExpressions;
using Azure.Storage.Blobs;
using Azure;
using Azure.Storage.Blobs.Models;
using Havit.Threading;
using System.Runtime.CompilerServices;

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
		private readonly CriticalSection<int> ensureContainerCriticalSection = new CriticalSection<int>();
		private readonly Lazy<BlobContainerClient> blobContainerClientLazy;

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
			BlobStorage = blobStorageConnectionString,
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
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(options.BlobStorage), nameof(options.BlobStorage));

			var blobStorageValueType = GetBlobStorageValueType(options.BlobStorage);
			switch (blobStorageValueType)
			{
				case BlobStorageValueType.ConnectionString:
					Contract.Requires<InvalidOperationException>(!String.IsNullOrEmpty(options.ContainerName), $"When {nameof(options.BlobStorage)} contains connection string then {nameof(options.ContainerName)} must be set.");
					break;

				case BlobStorageValueType.UrlWithSas:
					Contract.Requires<InvalidOperationException>(String.IsNullOrEmpty(options.ContainerName), $"When {nameof(options.BlobStorage)} contains URL with SAS token then {nameof(options.ContainerName)} must NOT be set (put the container to the URL).");
					break;

				case BlobStorageValueType.StorageName:
					Contract.Requires<InvalidOperationException>(options.TokenCredential != null, $"When {nameof(options.BlobStorage)} contains storage name (not connection string) then {nameof(options.TokenCredential)} must be set.");
					break;

				default: throw new InvalidOperationException(blobStorageValueType.ToString());
			}

			this.options = options;
			this.blobContainerClientLazy = new Lazy<BlobContainerClient>(() => CreateBlobContainerClient(this.options), LazyThreadSafetyMode.ExecutionAndPublication);
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
		public override async Task<bool> ExistsAsync(string fileName, CancellationToken cancellationToken = default)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName), nameof(fileName));

			BlobClient blobClient = GetBlobClient(fileName);
			return await blobClient.ExistsAsync(cancellationToken).ConfigureAwait(false);
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
		protected override async Task PerformReadToStreamAsync(string fileName, Stream stream, CancellationToken cancellationToken = default)
		{
			BlobClient blobClient = GetBlobClient(fileName);
			await blobClient.DownloadToAsync(stream, cancellationToken).ConfigureAwait(false);
		}

		/// <summary>
		/// Vrátí stream s obsahem soubor z úložiště.
		/// </summary>
		protected override Stream PerformRead(string fileName)
		{
			BlobClient blobClient = GetBlobClient(fileName);
			return blobClient.OpenRead();
		}

		/// <summary>
		/// Vrátí stream s obsahem soubor z úložiště.
		/// </summary>
		protected override async Task<Stream> PerformReadAsync(string fileName, CancellationToken cancellationToken = default)
		{
			BlobClient blobClient = GetBlobClient(fileName);
			return await blobClient.OpenReadAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
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
		protected override async Task PerformSaveAsync(string fileName, Stream fileContent, string contentType, CancellationToken cancellationToken = default)
		{
			await EnsureContainerAsync(cancellationToken).ConfigureAwait(false);

			BlobClient blobClient = GetBlobClient(fileName);

			BlobHttpHeaders blobHttpHeaders = new BlobHttpHeaders();
			blobHttpHeaders.ContentType = contentType;
			PerformSave_SetProperties(blobHttpHeaders);

			await blobClient.UploadAsync(fileContent, httpHeaders: blobHttpHeaders, cancellationToken: cancellationToken).ConfigureAwait(false);
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
		public override async Task DeleteAsync(string fileName, CancellationToken cancellationToken = default)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName));

			BlobClient blobClient = GetBlobClient(fileName);
			await blobClient.DeleteAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
		}

		/// <summary>
		/// Vylistuje seznam souborů v úložišti.
		/// </summary>
		public override IEnumerable<FileInfo> EnumerateFiles(string searchPattern = null)
		{
			if (!String.IsNullOrWhiteSpace(searchPattern))
			{
				// zamen souborova '\\' za '/', ktere lze pouzit v Azure blobu (azure blob storage je sám také zaměňuje)
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
		public override async IAsyncEnumerable<FileInfo> EnumerateFilesAsync(string searchPattern = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			if (!String.IsNullOrWhiteSpace(searchPattern))
			{
				// zamen souborova '\\' za '/', ktere lze pouzit v Azure blobu
				searchPattern = searchPattern.Replace("\\", "/");
			}

			// ziskej prefix, uvodni cast cesty, ve kterem nejsou pouzite znaky '*' a '?'
			string prefix = FileStorageServiceBase.EnumerableFilesGetPrefix(searchPattern);

			await EnsureContainerAsync(cancellationToken).ConfigureAwait(false);

			// nacti soubory s danym prefixem - optimalizace na rychlost
			AsyncPageable<BlobItem> blobItems = GetBlobContainerClient().GetBlobsAsync(prefix: prefix, cancellationToken: cancellationToken);

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

		/// <inheritdoc />
		protected override void PerformCopy(string sourceFileName, IFileStorageService targetFileStorageService, string targetFileName)
		{
			if (targetFileStorageService is AzureBlobStorageService targetAzureBlobStorageService)
			{
				BlobClient sourceBlobClient = this.GetBlobClient(sourceFileName);
				BlobClient targetBlobClient = targetAzureBlobStorageService.GetBlobClient(targetFileName);
				targetBlobClient.StartCopyFromUri(sourceBlobClient.Uri).WaitForCompletion();
			}
			else
			{
				base.PerformCopy(sourceFileName, targetFileStorageService, targetFileName);
			}
		}

		/// <inheritdoc />
		protected override async Task PerformCopyAsync(string sourceFileName, IFileStorageService targetFileStorageService, string targetFileName, CancellationToken cancellationToken)
		{
			if (targetFileStorageService is AzureBlobStorageService targetAzureBlobStorageService)
			{
				BlobClient sourceBlobClient = this.GetBlobClient(sourceFileName);
				BlobClient targetBlobClient = targetAzureBlobStorageService.GetBlobClient(targetFileName);
				CopyFromUriOperation operation = await targetBlobClient.StartCopyFromUriAsync(sourceBlobClient.Uri, null, cancellationToken).ConfigureAwait(false);
				await operation.WaitForCompletionAsync(cancellationToken).ConfigureAwait(false);
			}
			else
			{
				base.PerformCopy(sourceFileName, targetFileStorageService, targetFileName);
			}
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
		public override async Task<DateTime?> GetLastModifiedTimeUtcAsync(string fileName, CancellationToken cancellationToken = default)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName));

			BlobClient blobClient = GetBlobClient(fileName);
			BlobProperties properties = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
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
			return blobContainerClientLazy.Value;
		}

		/// <summary>
		/// Vytvoří používaný container (BlobContainerClient) v Azure Storage Accountu.
		/// </summary>
		public static BlobContainerClient CreateBlobContainerClient(AzureBlobStorageServiceOptions options)
		{
			var blobStorageValueType = GetBlobStorageValueType(options.BlobStorage);

			switch (blobStorageValueType)
			{
				case BlobStorageValueType.ConnectionString:
					return new BlobContainerClient(options.BlobStorage, options.ContainerName);

				case BlobStorageValueType.UrlWithSas:
					return new BlobContainerClient(new Uri(options.BlobStorage));

				case BlobStorageValueType.StorageName:
					string containerEndpoint = $"https://{options.BlobStorage}.blob.core.windows.net/{options.ContainerName}";
					return new BlobContainerClient(new Uri(containerEndpoint), options.TokenCredential);

				default: throw new InvalidOperationException(blobStorageValueType.ToString());
			}
		}

		/// <summary>
		/// Vrací Uri se SAS tokenem s danými oprávněními a danou expirací.
		/// Nelze použít při přístupu k storage pomocí Managed Identity, je třeba použít AccessKey.
		/// </summary>
		/// <remarks>
		/// Vzhledem k tomu, že je potřeba použít DateTimeOffset.UtcNow, nikoliv DateTimeOffset.Now
		/// a že si dovedu představit, jak do téhle pasti spadne úplně každý,
		/// raději volím do parametru datový typ TimeSpan s dobou platnosti. Použití správného UtcNow nechám zde v implementaci.
		/// </remarks>
		public Uri GenerateSasUri(string fileName, global::Azure.Storage.Sas.BlobSasPermissions permissions, TimeSpan expiration)
		{
			return GetBlobClient(fileName).GenerateSasUri(permissions, DateTimeOffset.UtcNow.Add(expiration));
		}

		/// <summary>
		/// Vytvoří kontejner, pokud ještě neexistuje.
		/// </summary>
		protected void EnsureContainer()
		{
			if (!containerAlreadyCreated && options.AutoCreateBlobContainer)
			{
				ensureContainerCriticalSection.ExecuteAction(0, () =>
				{
					if (!containerAlreadyCreated)
					{
						GetBlobContainerClient().CreateIfNotExists(PublicAccessType.None);
						containerAlreadyCreated = true;
					}
				});
			}
		}

		/// <summary>
		/// Vytvoří kontejner, pokud ještě neexistuje.
		/// </summary>
		protected async Task EnsureContainerAsync(CancellationToken cancellationToken = default)
		{
			if (!containerAlreadyCreated && options.AutoCreateBlobContainer)
			{
				await ensureContainerCriticalSection.ExecuteActionAsync(0, async () =>
				{
					if (!containerAlreadyCreated)
					{
						await GetBlobContainerClient().CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken).ConfigureAwait(false);
						containerAlreadyCreated = true;
					}
				}, cancellationToken).ConfigureAwait(false);
			}
		}

		/// <inheritdoc />
		protected override string GetContentType(string fileName)
		{
			return GetBlobClient(fileName).GetProperties().Value.ContentType;
		}

		/// <inheritdoc />
		protected override async ValueTask<string> GetContentTypeAsync(string fileName, CancellationToken cancellationToken)
		{
			return (await GetBlobClient(fileName).GetPropertiesAsync(cancellationToken: cancellationToken).ConfigureAwait(false)).Value.ContentType;
		}

		/// <inheritdoc />
		protected override void PerformMove(string sourceFileName, string targetFileName)
		{
			PerformCopy(sourceFileName, this, targetFileName);
			Delete(sourceFileName);
		}

		/// <inheritdoc />
		protected override async Task PerformMoveAsync(string sourceFileName, string targetFileName, CancellationToken cancellationToken)
		{
			await PerformCopyAsync(sourceFileName, this, targetFileName, cancellationToken).ConfigureAwait(false);
			await DeleteAsync(sourceFileName, cancellationToken).ConfigureAwait(false);
		}

		private static BlobStorageValueType GetBlobStorageValueType(string value)
		{
			if (value.Contains(";"))
			{
				return BlobStorageValueType.ConnectionString;
			}

			if (value.Contains("?") && value.ToLower().Contains("sig="))
			{
				// option.BlobStorage obsahuje URL se SAS tokenem (https://...blob.core.windows.net/demo?sp=...&st=...&se=...&spr=...&sv=...&sr=...&sig=...)
				return BlobStorageValueType.UrlWithSas;
			}

			// option.BlobStorage obsahuje jen název blob storage -> pak použijeme TokenCredential (zamýšleno pro Managed Identity)
			return BlobStorageValueType.StorageName;
		}

		private enum BlobStorageValueType
		{
			ConnectionString,
			UrlWithSas,
			StorageName
		}
	}
}
