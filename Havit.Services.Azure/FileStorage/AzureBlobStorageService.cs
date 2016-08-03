﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Services.FileStorage;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using FileInfo = Havit.Services.FileStorage.FileInfo;

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
	public class AzureBlobStorageService : FileStorageServiceBase
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
			this.blobStorageConnectionString = blobStorageConnectionString;
			this.containerName = containerName;
			this.encryptionPolicy = encryptionPolicy;
		}

		/// <summary>
		/// Vrátí true, pokud uložený soubor v úložišti existuje. Jinak false.
		/// </summary>
		public override bool Exists(string fileName)
		{
			CloudBlockBlob blob = GetBlobReference(fileName);
			return blob.Exists();
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
		/// Vrátí stream s obsahem soubor z úložiště.
		/// </summary>
		protected override Stream PerformRead(string fileName)
		{
			MemoryStream memoryStream = new MemoryStream();
			PerformReadToStream(fileName, memoryStream);
			memoryStream.Seek(0, SeekOrigin.Begin);
			
			return memoryStream;
		}

		/// <summary>
		/// Uloží stream do úložiště.
		/// </summary>
		protected override void PerformSave(string fileName, Stream fileContent, string contentType)
		{
			CloudBlockBlob blob = GetBlobReference(fileName, createContainerWhenNotExists: true);
			blob.Properties.ContentType = contentType;
			blob.UploadFromStream(fileContent, options: GetBlobRequestOptions());			
		}

		/// <summary>
		/// Smaže soubor v úložišti.
		/// </summary>
		public override void Delete(string fileName)
		{
			CloudBlockBlob blob = GetBlobReference(fileName);
			blob.Delete();			
		}

		/// <summary>
		/// Vylistuje seznam souborů v úložišti.
		/// </summary>
		public override IEnumerable<FileInfo> EnumerateFiles(string searchPattern = null)
		{
			var blobsEnumerable = GetContainerReference(true).ListBlobs().OfType<CloudBlob>();
			if (!String.IsNullOrEmpty(searchPattern))
			{
				// Operators.Like 
				// - viz http://stackoverflow.com/questions/652037/how-do-i-check-if-a-filename-matches-a-wildcard-pattern
				// - viz https://msdn.microsoft.com/cs-cz/library/swf8kaxw.aspx
				string normalizedSearchPatterns = searchPattern
					.Replace("[", "[[]") // pozor, zálěží na pořadí náhrad
					.Replace("#", "[#]");
				blobsEnumerable = blobsEnumerable.Where(item => Operators.LikeString(item.Name, normalizedSearchPatterns, CompareMethod.Text));
			}

			return blobsEnumerable.Select(blob => new FileInfo
			{
				Name = blob.Name,
				LastModifiedUtc = blob.Properties.LastModified?.UtcDateTime ?? default(DateTime),
				Size = blob.Properties.Length
			});
		}

		/// <summary>
		/// Vrátí čas poslední modifikace souboru v UTC timezone
		/// </summary>
		public override DateTime? GetLastModifiedTimeUtc(string fileName)
		{
			CloudBlockBlob blob = GetBlobReference(fileName, fromServer: true);
			return blob.Properties.LastModified?.UtcDateTime;
		}

		/// <summary>
		/// Vrátí CloudBlockBlob pro daný blob v containeru používaného Azure Storage Accountu.
		/// </summary>
		protected CloudBlockBlob GetBlobReference(string blobName, bool createContainerWhenNotExists = false, bool fromServer = false)
		{
			var container = GetContainerReference(createContainerWhenNotExists);

			if (fromServer)
			{
				return (CloudBlockBlob)container.GetBlobReferenceFromServer(blobName);
			}
			else
			{
				return container.GetBlockBlobReference(blobName);
			}
		}

		/// <summary>
		/// Vrátí používaný container (CloudBlobContainer) v Azure Storage Accountu.
		/// </summary>
		protected CloudBlobContainer GetContainerReference(bool createContainerWhenNotExists)
		{
			CloudStorageAccount storageAccount = CloudStorageAccount.Parse(blobStorageConnectionString);
			// Create the blob client.
			CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
			// Retrieve reference to a previously created container.
			CloudBlobContainer container = blobClient.GetContainerReference(containerName);

			if (createContainerWhenNotExists && !containerAlreadyCreated)
			{
				container.CreateIfNotExists(BlobContainerPublicAccessType.Off);
				containerAlreadyCreated = true;
			}
			return container;
		}

		/// <summary>
		/// Vrátí BlobRequestOptions pro Azure Storage API.
		/// </summary>
		protected BlobRequestOptions GetBlobRequestOptions()
		{
			return (this.encryptionPolicy == null) ? null : new BlobRequestOptions { EncryptionPolicy = this.encryptionPolicy };
		}

		/// <summary>
		/// Zašifruje všechny soubory v úložišti.
		/// </summary>
		/// <remarks>
		/// Určeno pro zašifrování souborů na Chesteru.
		/// </remarks>
		public void EncryptAllFiles()
		{
			Contract.Assert(SupportsBasicEncryption);

			string[] filesToEncrypt = null;
			List<string> encryptedFiles = new List<string>();

			try
			{
				List<CloudBlockBlob> blobs = new List<CloudBlockBlob>();
				// ochrana pro případ přidání souboru během šifrování
				// současné běžící přidání souboru během šifrování se neřeší
#pragma warning disable S1121 // Assignments should not be made from within sub-expressions
				while ((filesToEncrypt = EnumerateFiles().Select(item => item.Name).OrderBy(item => item).Except(encryptedFiles).ToArray()).Length > 0)
#pragma warning restore S1121 // Assignments should not be made from within sub-expressions
				{
					foreach (string fileName in filesToEncrypt)
					{
						CloudBlockBlob blob = GetBlobReference(fileName, false, true);
						blobs.Add(blob);
						blob.CreateSnapshot();
						// Save - zapíšeme s šifrováním
						// Perform read - čteme skutečný obsah (čištý, nešifrovaný)
						Save(fileName, PerformRead(fileName), blob.Properties.ContentType);
						encryptedFiles.Add(fileName);
					}
				}

				blobs.ForEach(blob => blob.Delete(DeleteSnapshotsOption.DeleteSnapshotsOnly));
			}
			catch (Exception exception)
			{
				throw new EncryptDecryptAllFilesException("All files encryption failed.", exception, encryptedFiles.ToArray(), filesToEncrypt);
			}
		}

		/// <summary>
		/// Dešifruje všechny soubory v úložišti.
		/// </summary>
		public void DecryptAllFiles()
		{
			Contract.Assert(SupportsBasicEncryption);

			string[] filesToDecrypt = null;
			List<string> decryptedFiles = new List<string>();

			try
			{
				List<CloudBlockBlob> blobs = new List<CloudBlockBlob>();
#pragma warning disable S1121 // Assignments should not be made from within sub-expressions
				while ((filesToDecrypt = EnumerateFiles().Select(item => item.Name).OrderBy(item => item).Except(decryptedFiles).ToArray()).Length > 0)
#pragma warning restore S1121 // Assignments should not be made from within sub-expressions
				{
					foreach (string fileName in filesToDecrypt)
					{
						CloudBlockBlob blob = GetBlobReference(fileName, false, true);
						blobs.Add(blob);
						blob.CreateSnapshot();
						// PerformSave - zapíšeme bez šifrování
						// Read - čteme šifrovaný obsah a dešifrujeme jej
						PerformSave(fileName, Read(fileName), blob.Properties.ContentType);
						decryptedFiles.Add(fileName);
					}
					blobs.ForEach(blob => blob.Delete(DeleteSnapshotsOption.DeleteSnapshotsOnly));
				}
			}
			catch (Exception exception)
			{
				throw new EncryptDecryptAllFilesException("All files decryption failed.", exception, decryptedFiles.ToArray(), filesToDecrypt);
			}
		}
	}
}
