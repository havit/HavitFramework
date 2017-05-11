using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Havit.Services.FileStorage;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using FileInfo = Havit.Services.FileStorage.FileInfo;
using Havit.Diagnostics.Contracts;
using Havit.Services.Azure.Storage.Blob;

namespace Havit.Services.Azure.FileStorage
{
	/// <summary>
	/// Úložiště souborů jako Azure Blob Storage.
	/// Podporuje šifrování a to jak transparentní šifrování z předka (FileStorageServiceBase), 
	/// tak šifrování vestavěné v Azure Storage klientu (https://azure.microsoft.com/en-us/documentation/articles/storage-client-side-encryption/).
	/// 
	/// Pro jednoduché šifrování se používá konstruktor s encryptionOptions (EncryptionOptions),
	/// pro šifrování pomocí Azure Storage klienta se použije kontruktor s encyptionPolicy (BlobEnctyptionPolicy).
	/// </summary>
	public class AzureBlobStorageService : FileStorageServiceBase, IFileStorageService, IFileStorageServiceAsync
	{
		private readonly string blobStorageConnectionString;
		private readonly string containerName;

		private readonly BlobEncryptionPolicy encryptionPolicy;

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
		/// Konstruktor. Služba bude šifrovat obsah funkcionalitou vestavěnou v Azure Storage klientu.
		/// </summary>
		/// <param name="blobStorageConnectionString">Connection string pro připojení k Azure Blob Storage.</param>
		/// <param name="containerName">Container v Blob Storage pro práci se soubory.</param>
		/// <param name="encryptionPolicy">Parametry šifrování.</param>
		public AzureBlobStorageService(string blobStorageConnectionString, string containerName, BlobEncryptionPolicy encryptionPolicy) : this(blobStorageConnectionString, containerName, encryptionPolicy, null)
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
		protected AzureBlobStorageService(string blobStorageConnectionString, string containerName, BlobEncryptionPolicy encryptionPolicy, EncryptionOptions encryptionOptions) : base(encryptionOptions)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(blobStorageConnectionString));
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(containerName));

			this.blobStorageConnectionString = blobStorageConnectionString;
			this.containerName = containerName;
			this.encryptionPolicy = encryptionPolicy;
		}

		/// <summary>
		/// Vrátí true, pokud uložený soubor v úložišti existuje. Jinak false.
		/// </summary>
		public override bool Exists(string fileName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName));

			CloudBlockBlob blob = GetBlobReference(fileName);
			return blob.Exists();
		}

		/// <summary>
		/// Vrátí true, pokud uložený soubor v úložišti existuje. Jinak false.
		/// </summary>
		public override async Task<bool> ExistsAsync(string fileName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName));

			CloudBlockBlob blob = GetBlobReference(fileName);
			return await blob.ExistsAsync();
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
			await blob.DownloadToStreamAsync(stream, options: GetBlobRequestOptions(), accessCondition: null, operationContext: null);
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
			return await GetBlobReference(fileName).OpenReadAsync(options: GetBlobRequestOptions(), accessCondition: null, operationContext: null);
		}

		/// <summary>
		/// Uloží stream do úložiště.
		/// </summary>
		protected override void PerformSave(string fileName, Stream fileContent, string contentType)
		{
			EnsureContainer();

			CloudBlockBlob blob = GetBlobReference(fileName);
			blob.Properties.ContentType = contentType;
			blob.UploadFromStream(fileContent, options: GetBlobRequestOptions());
		}

		protected override async Task PerformSaveAsync(string fileName, Stream fileContent, string contentType)
		{
			await EnsureContainerAsync();

			CloudBlockBlob blob = GetBlobReference(fileName);
			blob.Properties.ContentType = contentType;
			await blob.UploadFromStreamAsync(fileContent, options: GetBlobRequestOptions(), accessCondition: null, operationContext: null);
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
			await blob.DeleteAsync();
		}

		/// <summary>
		/// Vylistuje seznam souborů v úložišti.
		/// </summary>
		public override IEnumerable<FileInfo> EnumerateFiles(string searchPattern = null)
		{
			string prefix;
			string newSearchPattern;
			EnumerableFiles_GetPrefixAndSearchPattern(searchPattern, out prefix, out newSearchPattern);

			EnsureContainer();

			IEnumerable<IListBlobItem> listBlobItems = GetContainerReference().ListBlobs(prefix, true);
			return EnumerateFiles_FilterAndProjectCloudBlobs(listBlobItems, newSearchPattern);
		}

		/// <summary>
		/// Vylistuje seznam souborů v úložišti.
		/// </summary>
		public override async Task<IEnumerable<FileInfo>> EnumerateFilesAsync(string searchPattern = null)
		{
			string prefix;
			string newSearchPattern;
			EnumerableFiles_GetPrefixAndSearchPattern(searchPattern, out prefix, out newSearchPattern);

			await EnsureContainerAsync();

			List<IListBlobItem> listBlobItems = (await GetContainerReference().ListBlobsAsync(prefix, true));
			return EnumerateFiles_FilterAndProjectCloudBlobs(listBlobItems, newSearchPattern);
		}

		private void EnumerableFiles_GetPrefixAndSearchPattern(string searchPattern, out string prefix, out string newSearchPattern)
		{
			prefix = null;
			newSearchPattern = searchPattern;

			if ((searchPattern != null) && searchPattern.Contains('/'))
			{
				int delimiter = searchPattern.LastIndexOf('/');
				prefix = searchPattern.Substring(0, delimiter);
				newSearchPattern = searchPattern.Remove(0, delimiter + 1);
			}
		}

		private IEnumerable<FileInfo> EnumerateFiles_FilterAndProjectCloudBlobs(IEnumerable<IListBlobItem> listBlobItems, string searchPattern)
		{
			IEnumerable<CloudBlob> cloudBlobs = listBlobItems.OfType<CloudBlob>();
			if (!String.IsNullOrEmpty(searchPattern))
			{
				// Operators.Like 
				// - viz http://stackoverflow.com/questions/652037/how-do-i-check-if-a-filename-matches-a-wildcard-pattern
				// - viz https://msdn.microsoft.com/cs-cz/library/swf8kaxw.aspx
				string normalizedSearchPatterns = searchPattern
					.Replace("[", "[[]") // pozor, zálěží na pořadí náhrad
					.Replace("#", "[#]");
				cloudBlobs = cloudBlobs.Where(item => Operators.LikeString(item.Name, normalizedSearchPatterns, CompareMethod.Text));
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
		/// Vrátí čas poslední modifikace souboru v UTC timezone
		/// </summary>
		public override DateTime? GetLastModifiedTimeUtc(string fileName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName));

			CloudBlobContainer container = GetContainerReference();
			ICloudBlob blob = container.GetBlobReferenceFromServer(fileName);
			return blob.Properties.LastModified?.UtcDateTime;
		}

		public override async Task<DateTime?> GetLastModifiedTimeUtcAsync(string fileName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName));

			CloudBlobContainer container =  GetContainerReference();
			ICloudBlob blob = await container.GetBlobReferenceFromServerAsync(fileName);			
			return blob.Properties.LastModified?.UtcDateTime;
		}

		/// <summary>
		/// Vrátí CloudBlockBlob pro daný blob v containeru používaného Azure Storage Accountu.
		/// </summary>
		protected CloudBlockBlob GetBlobReference(string blobName)
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

		protected void EnsureContainer()
		{
			if (!containerAlreadyCreated)
			{
				GetContainerReference().CreateIfNotExists(BlobContainerPublicAccessType.Off);
				containerAlreadyCreated = true;
			}
		}

		protected async Task EnsureContainerAsync()
		{
			if (!containerAlreadyCreated)
			{
				await GetContainerReference().CreateIfNotExistsAsync(BlobContainerPublicAccessType.Off, options: null, operationContext: null);
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
