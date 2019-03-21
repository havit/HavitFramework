using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Havit.Services.FileStorage;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using FileInfo = Havit.Services.FileStorage.FileInfo;
using Havit.Diagnostics.Contracts;
using Havit.Services.Azure.Storage.Blob;
using Havit.Text.RegularExpressions;

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

		private readonly BlobEncryptionPolicy encryptionPolicy;
		private readonly AzureBlobStorageServiceOptions options;

		private volatile bool containerAlreadyCreated = false;

		/// <summary>
		/// Konstruktor. Služba nebude šifrovat obsah.
		/// </summary>
		/// <param name="blobStorageConnectionString">Connection string pro připojení k Azure Blob Storage.</param>
		/// <param name="containerName">Container v Blob Storage pro práci se soubory.</param>
		public AzureBlobStorageService(string blobStorageConnectionString, string containerName) : this(blobStorageConnectionString, containerName, null, null, null)
		{
		}

		/// <summary>
		/// Konstruktor. Služba nebude šifrovat obsah.
		/// </summary>
		/// <param name="blobStorageConnectionString">Connection string pro připojení k Azure Blob Storage.</param>
		/// <param name="containerName">Container v Blob Storage pro práci se soubory.</param>
		/// <param name="options">Další nastavení.</param>
		public AzureBlobStorageService(string blobStorageConnectionString, string containerName, AzureBlobStorageServiceOptions options) : this(blobStorageConnectionString, containerName, options, null, null)
		{
		}

		/// <summary>
		/// Konstruktor. Služba bude šifrovat obsah funkcionalitou vestavěnou v Azure Storage klientu.
		/// </summary>
		/// <param name="blobStorageConnectionString">Connection string pro připojení k Azure Blob Storage.</param>
		/// <param name="containerName">Container v Blob Storage pro práci se soubory.</param>
		/// <param name="encryptionPolicy">Parametry šifrování.</param>
		public AzureBlobStorageService(string blobStorageConnectionString, string containerName, BlobEncryptionPolicy encryptionPolicy) : this(blobStorageConnectionString, containerName, null, encryptionPolicy, null)
		{
		}

		/// <summary>
		/// Konstruktor. Služba bude šifrovat obsah funkcionalitou vestavěnou v Azure Storage klientu.
		/// </summary>
		/// <param name="blobStorageConnectionString">Connection string pro připojení k Azure Blob Storage.</param>
		/// <param name="containerName">Container v Blob Storage pro práci se soubory.</param>
		/// <param name="encryptionPolicy">Parametry šifrování.</param>
		/// <param name="options">Další nastavení.</param>
		public AzureBlobStorageService(string blobStorageConnectionString, string containerName, AzureBlobStorageServiceOptions options, BlobEncryptionPolicy encryptionPolicy) : this(blobStorageConnectionString, containerName, options, encryptionPolicy, null)
		{
		}

		/// <summary>
		/// Konstruktor. Služba bude šifrovat obsah vlastní implementací.
		/// </summary>
		/// <param name="blobStorageConnectionString">Connection string pro připojení k Azure Blob Storage.</param>
		/// <param name="containerName">Container v Blob Storage pro práci se soubory.</param>
		/// <param name="encryptionOptions">Parametry šifrování.</param>
		public AzureBlobStorageService(string blobStorageConnectionString, string containerName, EncryptionOptions encryptionOptions) : this(blobStorageConnectionString, containerName, null, null, encryptionOptions)
		{			
		}

		/// <summary>
		/// Konstruktor. Služba bude šifrovat obsah vlastní implementací.
		/// </summary>
		/// <param name="blobStorageConnectionString">Connection string pro připojení k Azure Blob Storage.</param>
		/// <param name="containerName">Container v Blob Storage pro práci se soubory.</param>
		/// <param name="encryptionOptions">Parametry šifrování.</param>
		/// <param name="options">Další nastavení.</param>
		public AzureBlobStorageService(string blobStorageConnectionString, string containerName, AzureBlobStorageServiceOptions options, EncryptionOptions encryptionOptions) : this(blobStorageConnectionString, containerName, options, null, encryptionOptions)
		{
		}

		/// <summary>
		/// Konstruktor. Služba bude šifrovat obsah vlastní implementací.
		/// </summary>
		protected AzureBlobStorageService(string blobStorageConnectionString, string containerName, AzureBlobStorageServiceOptions options, BlobEncryptionPolicy encryptionPolicy, EncryptionOptions encryptionOptions) : base(encryptionOptions)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(blobStorageConnectionString), nameof(blobStorageConnectionString));
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(containerName), nameof(containerName));

			this.blobStorageConnectionString = blobStorageConnectionString;
			this.containerName = containerName;
			this.encryptionPolicy = encryptionPolicy;
			this.options = options;
		}

		/// <summary>
		/// Vrátí true, pokud uložený soubor v úložišti existuje. Jinak false.
		/// </summary>
		public override bool Exists(string fileName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName), nameof(fileName));

			CloudBlockBlob blob = GetBlobReference(fileName);
			return blob.Exists();
		}

		/// <summary>
		/// Vrátí true, pokud uložený soubor v úložišti existuje. Jinak false.
		/// </summary>
		public override async Task<bool> ExistsAsync(string fileName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName), nameof(fileName));

			CloudBlockBlob blob = GetBlobReference(fileName);
			return await blob.ExistsAsync().ConfigureAwait(false);
		}

		/// <summary>
		/// Zapíše obsah souboru z úložiště do streamu.
		/// </summary>
		protected override void PerformReadToStream(string fileName, Stream stream)
		{
			CloudBlockBlob blob = GetBlobReference(fileName);
			blob.DownloadToStream(stream, options: GetBlobRequestOptions());
		}

		/// <summary>
		/// Zapíše obsah souboru z úložiště do streamu.
		/// </summary>
		protected override async Task PerformReadToStreamAsync(string fileName, Stream stream)
		{
			CloudBlockBlob blob = GetBlobReference(fileName);
			await blob.DownloadToStreamAsync(stream, options: GetBlobRequestOptions(), accessCondition: null, operationContext: null).ConfigureAwait(false);
		}

		/// <summary>
		/// Vrátí stream s obsahem soubor z úložiště.
		/// </summary>
		protected override Stream PerformRead(string fileName)
		{
			return GetBlobReference(fileName).OpenRead(options: GetBlobRequestOptions());
		}

		/// <summary>
		/// Vrátí stream s obsahem soubor z úložiště.
		/// </summary>
		protected override async Task<Stream> PerformReadAsync(string fileName)
		{
			return await GetBlobReference(fileName).OpenReadAsync(options: GetBlobRequestOptions(), accessCondition: null, operationContext: null).ConfigureAwait(false);
		}

		/// <summary>
		/// Uloží stream do úložiště.
		/// </summary>
		protected override void PerformSave(string fileName, Stream fileContent, string contentType)
		{
			EnsureContainer();

			CloudBlockBlob blob = GetBlobReference(fileName);
			blob.Properties.ContentType = contentType;
			PerformSave_SetProperties(blob);
			blob.UploadFromStream(fileContent, options: GetBlobRequestOptions());
		}

		/// <summary>
		/// Uloží stream do úložiště.
		/// </summary>
		protected override async Task PerformSaveAsync(string fileName, Stream fileContent, string contentType)
		{
			await EnsureContainerAsync().ConfigureAwait(false);

			CloudBlockBlob blob = GetBlobReference(fileName);
			blob.Properties.ContentType = contentType;
			PerformSave_SetProperties(blob);
			await blob.UploadFromStreamAsync(fileContent, options: GetBlobRequestOptions(), accessCondition: null, operationContext: null).ConfigureAwait(false);
		}

		private void PerformSave_SetProperties(CloudBlockBlob blob)
		{
			if (!String.IsNullOrEmpty(options?.CacheControl))
			{
				blob.Properties.CacheControl = options.CacheControl;
			}
		}

		/// <summary>
		/// Smaže soubor v úložišti.
		/// </summary>
		public override void Delete(string fileName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName));

			CloudBlockBlob blob = GetBlobReference(fileName);
			blob.Delete();			
		}

		/// <summary>
		/// Smaže soubor v úložišti.
		/// </summary>
		public override async Task DeleteAsync(string fileName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName));

			CloudBlockBlob blob = GetBlobReference(fileName);
			await blob.DeleteAsync().ConfigureAwait(false);
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
			string prefix = EnumerableFilesGetPrefix(searchPattern);

			EnsureContainer();

			// nacti soubory s danym prefixem - optimalizace na rychlost
			IEnumerable<IListBlobItem> listBlobItems = GetContainerReference().ListBlobs(prefix, true);

			// filtruj soubory podle masky
			return EnumerateFiles_FilterAndProjectCloudBlobs(listBlobItems, searchPattern);
		}

		/// <summary>
		/// Vylistuje seznam souborů v úložišti.
		/// </summary>
		public override async Task<IEnumerable<FileInfo>> EnumerateFilesAsync(string searchPattern = null)
		{
			if (!String.IsNullOrWhiteSpace(searchPattern))
			{
				// zamen souborova '\\' za '/', ktere lze pouzit v Azure blobu
				searchPattern = searchPattern.Replace("\\", "/");
			}

			// ziskej prefix, uvodni cast cesty, ve kterem nejsou pouzite znaky '*' a '?'
			string prefix = EnumerableFilesGetPrefix(searchPattern);

			await EnsureContainerAsync().ConfigureAwait(false);

			// nacti soubory s danym prefixem - optimalizace na rychlost
			List<IListBlobItem> listBlobItems = (await GetContainerReference().ListBlobsAsync(prefix, true).ConfigureAwait(false));

			// filtruj soubory podle masky
			return EnumerateFiles_FilterAndProjectCloudBlobs(listBlobItems, searchPattern);
		}

		private IEnumerable<FileInfo> EnumerateFiles_FilterAndProjectCloudBlobs(IEnumerable<IListBlobItem> listBlobItems, string searchPattern)
		{
			IEnumerable<CloudBlob> cloudBlobs = listBlobItems.OfType<CloudBlob>();

			if (searchPattern != null)
			{
				cloudBlobs = cloudBlobs.Where(item => RegexPatterns.IsFileWildcardMatch(item.Name, searchPattern));
			}

			return cloudBlobs.Select(blob => new FileInfo
			{
				Name = blob.Name,
				LastModifiedUtc = blob.Properties.LastModified?.UtcDateTime ?? default(DateTime),
				Size = blob.Properties.Length,
				ContentType = blob.Properties.ContentType
			}).ToList();
		}

		/// <summary>
		/// Vrátí čas poslední modifikace souboru v UTC timezone.
		/// </summary>
		public override DateTime? GetLastModifiedTimeUtc(string fileName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName));

			CloudBlobContainer container = GetContainerReference();
			ICloudBlob blob = container.GetBlobReferenceFromServer(fileName);
			return blob.Properties.LastModified?.UtcDateTime;
		}

		/// <summary>
		/// Vrátí čas poslední modifikace souboru v UTC timezone.
		/// </summary>
		public override async Task<DateTime?> GetLastModifiedTimeUtcAsync(string fileName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName));

			CloudBlobContainer container =  GetContainerReference();
			ICloudBlob blob = await container.GetBlobReferenceFromServerAsync(fileName).ConfigureAwait(false);
			return blob.Properties.LastModified?.UtcDateTime;
		}

		/// <summary>
		/// Vrátí CloudBlockBlob pro daný blob v containeru používaného Azure Storage Accountu.
		/// </summary>
		protected internal CloudBlockBlob GetBlobReference(string blobName)
		{
			var container = GetContainerReference();
			return container.GetBlockBlobReference(blobName);
		}

		/// <summary>
		/// Vrátí používaný container (CloudBlobContainer) v Azure Storage Accountu.
		/// </summary>
		protected CloudBlobContainer GetContainerReference()
		{
			CloudStorageAccount storageAccount = CloudStorageAccount.Parse(blobStorageConnectionString);
			CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
			CloudBlobContainer container = blobClient.GetContainerReference(containerName);
			
			return container;
		}

		/// <summary>
		/// Vytvoří kontejner, pokud ještě neexistuje.
		/// </summary>
		protected void EnsureContainer()
		{
			if (!containerAlreadyCreated)
			{
				GetContainerReference().CreateIfNotExists(BlobContainerPublicAccessType.Off);
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
				await GetContainerReference().CreateIfNotExistsAsync(BlobContainerPublicAccessType.Off, options: null, operationContext: null).ConfigureAwait(false);
				containerAlreadyCreated = true;
			}
		}

		/// <summary>
		/// Vrátí BlobRequestOptions pro Azure Storage API.
		/// </summary>
		protected BlobRequestOptions GetBlobRequestOptions()
		{
			return (this.encryptionPolicy == null) ? null : new BlobRequestOptions { EncryptionPolicy = this.encryptionPolicy };
		}
	}
}
