using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Services.FileServices;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Havit.Services.Azure.FileServices
{
	/// <summary>
	/// Úložiště souborů jako Azure Blob Storage
	/// </summary>
	public class AzureBlobStorageService : IFileStorageService
	{
		#region Private fields
		private readonly string blobStorageConnectionString;
		private readonly string containerName;
		#endregion

		#region Constructor
		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="blobStorageConnectionString">Connection string pro připojení k Azure Blob Storage.</param>
		/// <param name="containerName">Container v Blob Storage pro práci se soubory.</param>
		public AzureBlobStorageService(string blobStorageConnectionString, string containerName)
		{
			this.blobStorageConnectionString = blobStorageConnectionString;
			this.containerName = containerName;
		}
		#endregion

		#region Exists
		/// <summary>
		/// Vrátí true, pokud uložený soubor v úložišti existuje. Jinak false.
		/// </summary>
		public bool Exists(string fileName)
		{
			CloudBlockBlob blob = GetBlobReference(fileName);
			return blob.Exists();
		}
		#endregion

		#region ReadToStream
		public void ReadToStream(string fileName, Stream stream)
		{
			CloudBlockBlob blob = GetBlobReference(fileName);
			blob.DownloadToStream(stream);			
		}
		#endregion

		#region Read
		/// <summary>
		/// Vrátí stream s obsahem soubor z úložiště.
		/// </summary>
		public Stream Read(string fileName)
		{
			MemoryStream memoryStream = new MemoryStream();
			ReadToStream(fileName, memoryStream);
			memoryStream.Seek(0, SeekOrigin.Begin);
			
			return memoryStream;
		}
		#endregion

		#region Save
		/// <summary>
		/// Uloží stream do úložiště.
		/// </summary>
		public void Save(string fileName, Stream fileContent, string contentType)
		{
			CloudBlockBlob blob = GetBlobReference(fileName, true);
			blob.Properties.ContentType = contentType;
			blob.UploadFromStream(fileContent);
		}
		#endregion

		#region Delete
		/// <summary>
		/// Smaže soubor v úložišti.
		/// </summary>>
		public void Delete(string fileName)
		{
			CloudBlockBlob blob = GetBlobReference(fileName);
			blob.Delete();
		}
		#endregion

		#region GetBlobReference
		private CloudBlockBlob GetBlobReference(string blobName, bool createContainerWhenNotExists = false)
		{
			CloudStorageAccount storageAccount = CloudStorageAccount.Parse(blobStorageConnectionString);
			// Create the blob client.
			CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
			// Retrieve reference to a previously created container.
			CloudBlobContainer container = blobClient.GetContainerReference(containerName);
			if (createContainerWhenNotExists && !container.Exists())
			{
				container.CreateIfNotExists(BlobContainerPublicAccessType.Off);
			}
			return container.GetBlockBlobReference(blobName);
		}
		#endregion
	}
}
