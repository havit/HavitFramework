using Havit.Diagnostics.Contracts;
using Havit.Services.FileStorage;
using Havit.Text.RegularExpressions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services.Azure.FileStorage
{
	/// <summary>
	/// Úložiště souborů jako Azure File Storage. Pro velmi specifické použití! V Azure je obvykle používán <see cref="AzureBlobStorageService" />.
	/// Umožňuje jako svůj root použít specifickou složku ve FileShare.
	/// 
	/// Nepodporuje transparentní šifrování z předka, protože implementace Microsoft.Azure.Storage.File.CloudFile vyžaduje,
	/// aby byl používaný stream seekovatelný, což InternalCryptoStream není.
	/// Viz https://github.com/Azure/azure-storage-net/blob/master/Lib/ClassLibraryCommon/File/CloudFile.cs	
	/// (Použití šifrování beztak postrádá smysl.)
	/// </summary>
	public class AzureFileStorageService : FileStorageServiceBase, IFileStorageService, IFileStorageServiceAsync
	{
		private readonly string fileStorageConnectionString;
		private readonly string fileShareName;
		private readonly string[] rootDirectoryNameSegments;

		private volatile bool fileShareAlreadyCreated = false;

		/// <summary>
		/// Konstruktor. Služba nebude šifrovat obsah.
		/// </summary>
		/// <param name="fileStorageConnectionString">Connection string pro připojení k Azure File Storage.</param>
		/// <param name="fileShareName">File Share ve File Storage pro práci se soubory.</param>
		public AzureFileStorageService(string fileStorageConnectionString, string fileShareName) : this(fileStorageConnectionString, fileShareName, null)
		{
		}

		/// <summary>
		/// Konstruktor. Služba nebude šifrovat obsah.
		/// </summary>
		/// <param name="fileStorageConnectionString">Connection string pro připojení k Azure File Storage.</param>
		/// <param name="fileShareName">File Share ve File Storage pro práci se soubory.</param>
		/// <param name="rootDirectoryName">Název složky, která bude rootem pro použití.</param>
		public AzureFileStorageService(string fileStorageConnectionString, string fileShareName, string rootDirectoryName) : base()
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileStorageConnectionString), nameof(fileStorageConnectionString));
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileShareName), nameof(fileShareName));

			this.fileStorageConnectionString = fileStorageConnectionString;
			this.fileShareName = fileShareName;
			this.rootDirectoryNameSegments = rootDirectoryName?.Replace("\\", "/").Split('/').Where(item => !String.IsNullOrEmpty(item)).ToArray() ?? new string[0];
		}

		/// <summary>
		/// Vrátí true, pokud uložený soubor v úložišti existuje. Jinak false.
		/// </summary>
		public override bool Exists(string fileName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName), nameof(fileName));

			CloudFile file = GetFileReference(fileName);
			return file.Exists();
		}

		/// <summary>
		/// Vrátí true, pokud uložený soubor v úložišti existuje. Jinak false.
		/// </summary>
		public override async Task<bool> ExistsAsync(string fileName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName), nameof(fileName));

			CloudFile file = GetFileReference(fileName); // nechceme zakládat složku, můžeme použít synchronní kód v asynchronní metodě
			return await file.ExistsAsync().ConfigureAwait(false);
		}

		/// <summary>
		/// Zapíše obsah souboru z úložiště do streamu.
		/// </summary>
		protected override void PerformReadToStream(string fileName, System.IO.Stream stream)
		{
			CloudFile file = GetFileReference(fileName);
			file.DownloadToStream(stream);
		}

		/// <summary>
		/// Zapíše obsah souboru z úložiště do streamu.
		/// </summary>
		protected override async Task PerformReadToStreamAsync(string fileName, System.IO.Stream stream)
		{
			CloudFile file = GetFileReference(fileName); // nechceme zakládat složku, můžeme použít synchronní kód v asynchronní metodě
			await file.DownloadToStreamAsync(stream).ConfigureAwait(false);
		}

		/// <summary>
		/// Vrátí stream s obsahem soubor z úložiště.
		/// </summary>
		protected override System.IO.Stream PerformRead(string fileName)
		{
			CloudFile file = GetFileReference(fileName);
			return file.OpenRead();
		}

		/// <summary>
		/// Vrátí stream s obsahem soubor z úložiště.
		/// </summary>
		protected override async Task<System.IO.Stream> PerformReadAsync(string fileName)
		{
			CloudFile file = GetFileReference(fileName); // nechceme zakládat složku, můžeme použít synchronní kód v asynchronní metodě
			return await file.OpenReadAsync().ConfigureAwait(false);
		}

		/// <summary>
		/// Uloží stream do úložiště.
		/// </summary>
		protected override void PerformSave(string fileName, System.IO.Stream fileContent, string contentType)
		{
			EnsureFileShare();

			CloudFile file = GetFileReference(fileName, createDirectoryStructure: true);
			file.UploadFromStream(fileContent);
		}

		/// <summary>
		/// Uloží stream do úložiště.
		/// </summary>
		protected override async Task PerformSaveAsync(string fileName, System.IO.Stream fileContent, string contentType)
		{
			await EnsureFileShareAsync().ConfigureAwait(false);

			CloudFile file = await GetFileReferenceAsync(fileName, createDirectoryStructure: true).ConfigureAwait(false);
			await file.UploadFromStreamAsync(fileContent).ConfigureAwait(false);
		}

		/// <summary>
		/// Smaže soubor v úložišti.
		/// </summary>
		public override void Delete(string fileName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName));

			CloudFile file = GetFileReference(fileName);
			file.Delete();
		}

		/// <summary>
		/// Smaže soubor v úložišti.
		/// </summary>
		public override async Task DeleteAsync(string fileName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName));

			CloudFile file = GetFileReference(fileName); // nechceme zakládat složku, můžeme použít synchronní kód v asynchronní metodě
			await file.DeleteAsync().ConfigureAwait(false);
		}

		/// <summary>
		/// Vylistuje seznam souborů v úložišti.
		/// </summary>
		/// <remarks>
		/// Nepodporuje LastModified (ve výsledku není hodnota nastavena).
		/// Při používání složek je výkonově neefektivní (REST API neumí lepší variantu).
		/// </remarks>
		public override IEnumerable<FileInfo> EnumerateFiles(string searchPattern = null)
		{
			if (!String.IsNullOrWhiteSpace(searchPattern))
			{
				// zamen souborova '\\' za '/', ktere lze pouzit v Azure blobu
				searchPattern = searchPattern.Replace("\\", "/");
			}

			// ziskej prefix, uvodni cast cesty, ve kterem nejsou pouzite znaky '*' a '?'
			string prefix = FileStorageServiceBase.EnumerableFilesGetPrefix(searchPattern);

			EnsureFileShare();

			var items = EnumerateFiles_ListFilesInHierarchyInternal(GetRootDirectoryReference(), "", prefix);
			return EnumerateFiles_FilterAndProjectCloudBlobs(items, searchPattern);
		}

		/// <summary>
		/// Vylistuje seznam souborů v úložišti.
		/// </summary>
		/// <remarks>
		/// Nepodporuje LastModified (ve výsledku není hodnota nastavena).
		/// Při používání složek je výkonově neefektivní (REST API neumí lepší variantu).
		/// </remarks>
		public override async Task<IEnumerable<FileInfo>> EnumerateFilesAsync(string searchPattern = null)
		{
			if (!String.IsNullOrWhiteSpace(searchPattern))
			{
				// zamen souborova '\\' za '/', ktere lze pouzit v Azure blobu
				searchPattern = searchPattern.Replace("\\", "/");
			}

			// ziskej prefix, uvodni cast cesty, ve kterem nejsou pouzite znaky '*' a '?'
			string prefix = FileStorageServiceBase.EnumerableFilesGetPrefix(searchPattern);

			EnsureFileShare();

			var items = await EnumerateFiles_ListFilesInHierarchyInternalAsync(GetRootDirectoryReference(), "", prefix).ConfigureAwait(false);
			return EnumerateFiles_FilterAndProjectCloudBlobs(items, searchPattern);
		}

		private IEnumerable<FileInfo> EnumerateFiles_ListFilesInHierarchyInternal(CloudFileDirectory directoryReference, string directoryPrefix, string searchPrefix)
		{
			// speed up
			if (!String.IsNullOrEmpty(searchPrefix) && !(directoryPrefix.StartsWith(searchPrefix) || searchPrefix.StartsWith(directoryPrefix)))
			{
				yield break;
			}

			FileContinuationToken token = null;
			do
			{
				FileResultSegment resultSegment = directoryReference.ListFilesAndDirectoriesSegmented(token);
				var directoryItems = resultSegment.Results.ToList();

				foreach (CloudFile file in directoryItems.OfType<CloudFile>())
				{
					yield return new FileInfo
					{
						Name = directoryPrefix + file.Name,
						LastModifiedUtc = file.Properties.LastModified?.UtcDateTime ?? default(DateTime),
						Size = file.Properties.Length,
						ContentType = file.Properties.ContentType
					};
				}

				foreach (CloudFileDirectory subdirectory in directoryItems.OfType<CloudFileDirectory>())
				{
					var subdirectoryItems = EnumerateFiles_ListFilesInHierarchyInternal(directoryReference.GetDirectoryReference(subdirectory.Name), directoryPrefix + subdirectory.Name + '/', searchPrefix);

					foreach (var subdirectoryItem in subdirectoryItems)
					{
						yield return subdirectoryItem;
					}
				}

				token = resultSegment.ContinuationToken;
			}
			while (token != null);
		}

		private async Task<List<FileInfo>> EnumerateFiles_ListFilesInHierarchyInternalAsync(CloudFileDirectory directoryReference, string directoryPrefix, string searchPrefix)
		{
			// no yield return? 
			// The feature 'async streams' is currently in Preview and *unsupported *.To use Preview features, use the 'preview' language version.

			// speed up
			if (!String.IsNullOrEmpty(searchPrefix) && !(directoryPrefix.StartsWith(searchPrefix) || searchPrefix.StartsWith(directoryPrefix)))
			{
				return new List<FileInfo>();
			}
			
			List<FileInfo> result = new List<FileInfo>();
			
			FileContinuationToken token = null;
			do
			{
				FileResultSegment resultSegment = await directoryReference.ListFilesAndDirectoriesSegmentedAsync(token).ConfigureAwait(false);
				var directoryItems = resultSegment.Results.ToList();


				result.AddRange(directoryItems.OfType<CloudFile>().Select(file => new FileInfo
				{
					Name = directoryPrefix + file.Name,
					LastModifiedUtc = file.Properties.LastModified?.UtcDateTime ?? default(DateTime),
					Size = file.Properties.Length,
					ContentType = file.Properties.ContentType
				}));

				foreach (CloudFileDirectory subdirectory in directoryItems.OfType<CloudFileDirectory>())
				{
					var subdirectoryItems = await EnumerateFiles_ListFilesInHierarchyInternalAsync(directoryReference.GetDirectoryReference(subdirectory.Name), directoryPrefix + subdirectory.Name + '/', searchPrefix).ConfigureAwait(false);
					result.AddRange(subdirectoryItems);
				}

				token = resultSegment.ContinuationToken;
			}
			while (token != null);

			return result;
		}

		private IEnumerable<FileInfo> EnumerateFiles_FilterAndProjectCloudBlobs(IEnumerable<FileInfo> items, string searchPattern)
		{
			if (searchPattern != null)
			{
				items = items.Where(item => RegexPatterns.IsFileWildcardMatch(item.Name, searchPattern));
			}

			return items;
		}

		/// <summary>
		/// Vrátí čas poslední modifikace souboru v UTC timezone.
		/// </summary>
		public override DateTime? GetLastModifiedTimeUtc(string fileName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName));

			CloudFile file = GetFileReference(fileName);
			file.FetchAttributes();
			return file.Properties.LastModified?.UtcDateTime;
		}

		/// <summary>
		/// Vrátí čas poslední modifikace souboru v UTC timezone.
		/// </summary>
		public override async Task<DateTime?> GetLastModifiedTimeUtcAsync(string fileName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName));

			CloudFile file = GetFileReference(fileName); // nechceme zakládat složku, můžeme použít synchronní kód v asynchronní metodě
			await file.FetchAttributesAsync().ConfigureAwait(false);
			return file.Properties.LastModified?.UtcDateTime;
		}

		/// <summary>
		/// Vrátí CloudFile pro daný soubor ve FileShare používaného Azure Storage Accountu.
		/// </summary>
		protected CloudFile GetFileReference(string fileName, bool createDirectoryStructure = false)
		{
			var directoryReference = GetRootDirectoryReference();

			string[] segments = fileName.Replace("\\", "/").Split('/');
			if (segments.Length > 1)
			{
				for (int i = 0; i < segments.Length - 1; i++)
				{
					directoryReference = directoryReference.GetDirectoryReference(segments[i]);
					if (createDirectoryStructure)
					{
						directoryReference.CreateIfNotExists();
					}
				}
			}
			return directoryReference.GetFileReference(segments.Last());
		}

		/// <summary>
		/// Vrátí CloudFile pro daný soubor ve FileShare používaného Azure Storage Accountu.
		/// </summary>
		protected async Task<CloudFile> GetFileReferenceAsync(string fileName, bool createDirectoryStructure = false)
		{
			var directoryReference = GetRootDirectoryReference();

			string[] segments = fileName.Replace("\\", "/").Split('/');
			if (segments.Length > 1)
			{
				for (int i = 0; i < segments.Length - 1; i++)
				{
					directoryReference = directoryReference.GetDirectoryReference(segments[i]);
					if (createDirectoryStructure)
					{
						await directoryReference.CreateIfNotExistsAsync().ConfigureAwait(false);
					}
				}
			}
			return directoryReference.GetFileReference(segments.Last());
		}

		/// <summary>
		/// Vrátí CloudFileDirectory reprezentující rootovou složku pro práci s Azure File Storage.
		/// Pokud je požadována jiná root directory, je vracena tato složka.
		/// </summary>
		protected CloudFileDirectory GetRootDirectoryReference()
		{
			var shareReference = GetFileShareReference();
			var result = shareReference.GetRootDirectoryReference();

			if (rootDirectoryNameSegments.Length > 0)
			{
				for (int i = 0; i < rootDirectoryNameSegments.Length; i++)
				{
					result = result.GetDirectoryReference(rootDirectoryNameSegments[i]);
				}
			}

			return result;

		}

		/// <summary>
		/// Vrátí používaný CloudFileShare v Azure Storage Accountu.
		/// </summary>
		protected internal CloudFileShare GetFileShareReference()
		{
			CloudStorageAccount storageAccount = CloudStorageAccount.Parse(fileStorageConnectionString);
			CloudFileClient fileClient = storageAccount.CreateCloudFileClient();
			return fileClient.GetShareReference(fileShareName);
		}

		/// <summary>
		/// Vytvoří úložiště souborů (a ev. root directory), pokud ještě neexistuje.
		/// </summary>
		protected void EnsureFileShare()
		{
			if (!fileShareAlreadyCreated)
			{
				var shareReference = GetFileShareReference();
				shareReference.CreateIfNotExists();

				var directory = shareReference.GetRootDirectoryReference();
				if (rootDirectoryNameSegments.Length > 0)
				{
					for (int i = 0; i < rootDirectoryNameSegments.Length; i++)
					{
						directory = directory.GetDirectoryReference(rootDirectoryNameSegments[i]);
						directory.CreateIfNotExists();
					}
				}

				fileShareAlreadyCreated = true;
			}			
		}

		/// <summary>
		/// Vytvoří úložiště souborů (a ev. root directory), pokud ještě neexistuje.
		/// </summary>
		protected async Task EnsureFileShareAsync()
		{
			if (!fileShareAlreadyCreated)
			{
				var shareReference = GetFileShareReference();
				await shareReference.CreateIfNotExistsAsync().ConfigureAwait(false);

				var directory = shareReference.GetRootDirectoryReference();
				if (rootDirectoryNameSegments.Length > 0)
				{
					for (int i = 0; i < rootDirectoryNameSegments.Length; i++)
					{
						directory = directory.GetDirectoryReference(rootDirectoryNameSegments[i]);
						await directory.CreateIfNotExistsAsync().ConfigureAwait(false);
					}
				}

				fileShareAlreadyCreated = true;
			}
		}
	}
}