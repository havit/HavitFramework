using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Havit.Diagnostics.Contracts;
using Havit.Services.FileStorage;
using Havit.Text.RegularExpressions;
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
	/// Nepodporuje transparentní šifrování z předka.
	/// (Použití šifrování beztak postrádá smysl.)
	/// </summary>
	/// <remarks>
	/// Negenerickou třídu držíme pro zpětnou kompatibilitu.
	/// </remarks>
	public class AzureFileStorageService : FileStorageServiceBase, IFileStorageService
	{
		private readonly AzureFileStorageServiceOptions options;
		private readonly string[] rootDirectoryNameSegments;
		private volatile bool fileShareAlreadyCreated = false;

		/// <summary>
		/// Konstruktor. Služba nebude šifrovat obsah.
		/// </summary>
		/// <param name="fileStorageConnectionString">Connection string pro připojení k Azure File Storage.</param>
		/// <param name="fileShareName">File Share ve File Storage pro práci se soubory.</param>
		public AzureFileStorageService(string fileStorageConnectionString, string fileShareName) : this(fileStorageConnectionString, fileShareName, null)
		{
			// NOOP
		}

		/// <summary>
		/// Konstruktor. Služba nebude šifrovat obsah.
		/// </summary>
		/// <param name="fileStorageConnectionString">Connection string pro připojení k Azure File Storage.</param>
		/// <param name="fileShareName">File Share ve File Storage pro práci se soubory.</param>
		/// <param name="rootDirectoryName">Název složky, která bude rootem pro použití.</param>
		public AzureFileStorageService(string fileStorageConnectionString, string fileShareName, string rootDirectoryName) : this(new AzureFileStorageServiceOptions
		{
			FileStorageConnectionString = fileStorageConnectionString,
			FileShareName = fileShareName,
			RootDirectoryName = rootDirectoryName
		})
		{
			// NOOP
		}

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public AzureFileStorageService(AzureFileStorageServiceOptions options) : base()
		{
			Contract.Requires<ArgumentNullException>(options != null, nameof(options));
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(options.FileStorageConnectionString), nameof(options.FileStorageConnectionString));
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(options.FileShareName), nameof(options.FileShareName));

			this.options = options;
			this.rootDirectoryNameSegments = options.RootDirectoryName?.Replace("\\", "/").Split('/').Where(item => !String.IsNullOrEmpty(item)).ToArray() ?? new string[0];
		}

		/// <summary>
		/// Vrátí true, pokud uložený soubor v úložišti existuje. Jinak false.
		/// </summary>
		public override bool Exists(string fileName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName), nameof(fileName));

			ShareFileClient shareFileClient = GetShareFileClient(fileName);
			return shareFileClient.Exists();
		}

		/// <summary>
		/// Vrátí true, pokud uložený soubor v úložišti existuje. Jinak false.
		/// </summary>
		public override async Task<bool> ExistsAsync(string fileName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName), nameof(fileName));

			ShareFileClient shareFileClient = GetShareFileClient(fileName); // nechceme zakládat složku, můžeme použít synchronní kód v asynchronní metodě
			return await shareFileClient.ExistsAsync().ConfigureAwait(false);
		}

		/// <summary>
		/// Zapíše obsah souboru z úložiště do streamu.
		/// </summary>
		protected override void PerformReadToStream(string fileName, System.IO.Stream stream)
		{
			ShareFileClient shareFileClient = GetShareFileClient(fileName);
			ShareFileDownloadInfo downloadInfo = shareFileClient.Download();
			downloadInfo.Content.CopyTo(stream);
		}

		/// <summary>
		/// Zapíše obsah souboru z úložiště do streamu.
		/// </summary>
		protected override async Task PerformReadToStreamAsync(string fileName, System.IO.Stream stream)
		{
			ShareFileClient file = GetShareFileClient(fileName); // nechceme zakládat složku, můžeme použít synchronní kód v asynchronní metodě
			ShareFileDownloadInfo downloadInfo = await file.DownloadAsync().ConfigureAwait(false);
			await downloadInfo.Content.CopyToAsync(stream).ConfigureAwait(false);
		}

		/// <summary>
		/// Vrátí stream s obsahem soubor z úložiště.
		/// </summary>
		protected override System.IO.Stream PerformRead(string fileName)
		{
			ShareFileClient shareFileClient = GetShareFileClient(fileName);
			ShareFileDownloadInfo downloadInfo = shareFileClient.Download();
			return downloadInfo.Content;
		}

		/// <summary>
		/// Vrátí stream s obsahem soubor z úložiště.
		/// </summary>
		protected override async Task<System.IO.Stream> PerformReadAsync(string fileName)
		{
			ShareFileClient shareFileClient = GetShareFileClient(fileName); // nechceme zakládat složku, můžeme použít synchronní kód v asynchronní metodě
			ShareFileDownloadInfo downloadInfo = await shareFileClient.DownloadAsync().ConfigureAwait(false);
			return downloadInfo.Content;
		}

		/// <summary>
		/// Uloží stream do úložiště.
		/// </summary>
		protected override void PerformSave(string fileName, System.IO.Stream fileContent, string contentType)
		{
			EnsureFileShare();

			ShareFileClient shareFileClient = GetShareFileClient(fileName, createDirectoryStructure: true);
			shareFileClient.Create(fileContent.Length);
			if (fileContent.Length > 0)
			{
				try
				{
					shareFileClient.Upload(fileContent);
				}
				catch
				{
					try
					{
						shareFileClient.Delete();
					}
					catch
					{
						// NOOP
					}
				}
			}
		}

		/// <summary>
		/// Uloží stream do úložiště.
		/// </summary>
		protected override async Task PerformSaveAsync(string fileName, System.IO.Stream fileContent, string contentType)
		{
			await EnsureFileShareAsync().ConfigureAwait(false);

			ShareFileClient shareFileClient = await GetShareFileClientAsync(fileName, createDirectoryStructure: true).ConfigureAwait(false);
			await shareFileClient.CreateAsync(fileContent.Length).ConfigureAwait(false);
			if (fileContent.Length > 0)
			{
				try
				{
					await shareFileClient.UploadAsync(fileContent).ConfigureAwait(false);
				}
				catch
				{
					try
					{
						await shareFileClient.DeleteAsync().ConfigureAwait(false);
					}
					catch
					{
						// NOOP
					}
				}
			}
		}

		/// <summary>
		/// Smaže soubor v úložišti.
		/// </summary>
		public override void Delete(string fileName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName));

			ShareFileClient shareFileClient = GetShareFileClient(fileName);
			shareFileClient.Delete();
		}

		/// <summary>
		/// Smaže soubor v úložišti.
		/// </summary>
		public override async Task DeleteAsync(string fileName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName));

			ShareFileClient shareFileClient = GetShareFileClient(fileName); // nechceme zakládat složku, můžeme použít synchronní kód v asynchronní metodě
			await shareFileClient.DeleteAsync().ConfigureAwait(false);
		}

		/// <summary>
		/// Vylistuje seznam souborů v úložišti.
		/// </summary>
		/// <remarks>
		/// Nepodporuje LastModified a ContentType (ve výsledku nejsou hodnoty nastaveny).
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

			foreach (FileInfo fileInfo in EnumerateFiles_ListFilesInHierarchyInternal(GetRootShareDirectoryClient(), "", prefix))
			{
				if (EnumerateFiles_FilterFileInfo(fileInfo, searchPattern))
				{
					yield return fileInfo;
				}
			}
		}

		/// <summary>
		/// Vylistuje seznam souborů v úložišti.
		/// </summary>
		/// <remarks>
		/// Nepodporuje LastModified a ContentType (ve výsledku nejsou hodnoty nastaveny).
		/// Při používání složek je výkonově neefektivní (REST API neumí lepší variantu).
		/// </remarks>
		public override async IAsyncEnumerable<FileInfo> EnumerateFilesAsync(string searchPattern = null)
		{
			if (!String.IsNullOrWhiteSpace(searchPattern))
			{
				// zamen souborova '\\' za '/', ktere lze pouzit v Azure blobu
				searchPattern = searchPattern.Replace("\\", "/");
			}

			// ziskej prefix, uvodni cast cesty, ve kterem nejsou pouzite znaky '*' a '?'
			string prefix = FileStorageServiceBase.EnumerableFilesGetPrefix(searchPattern);

			EnsureFileShare();

			await foreach (FileInfo fileInfo in EnumerateFiles_ListFilesInHierarchyInternalAsync(GetRootShareDirectoryClient(), "", prefix))
			{
				if (EnumerateFiles_FilterFileInfo(fileInfo, searchPattern))
				{
					yield return fileInfo;
				}
			}
		}

		private IEnumerable<FileInfo> EnumerateFiles_ListFilesInHierarchyInternal(ShareDirectoryClient shareDirectoryClient, string directoryPrefix, string searchPrefix)
		{
			// speed up
			if (!String.IsNullOrEmpty(searchPrefix) && !(directoryPrefix.StartsWith(searchPrefix) || searchPrefix.StartsWith(directoryPrefix)))
			{
				yield break;
			}

			Pageable<ShareFileItem> directoryItems = shareDirectoryClient.GetFilesAndDirectories();
			List<string> subdirectories = new List<string>();

			foreach (ShareFileItem item in directoryItems)
			{
				if (!item.IsDirectory)
				{
					yield return new FileInfo
					{
						Name = directoryPrefix + item.Name,
						LastModifiedUtc = default(DateTime),
						Size = item.FileSize ?? -1,
						ContentType = null
					};
				}
				else
				{
					subdirectories.Add(item.Name);
				}
			}

			foreach (string subdirectory in subdirectories)
			{
				var subdirectoryItems = EnumerateFiles_ListFilesInHierarchyInternal(shareDirectoryClient.GetSubdirectoryClient(subdirectory), directoryPrefix + subdirectory + '/', searchPrefix);

				foreach (var subdirectoryItem in subdirectoryItems)
				{
					yield return subdirectoryItem;
				}
			}
		}

		private async IAsyncEnumerable<FileInfo> EnumerateFiles_ListFilesInHierarchyInternalAsync(ShareDirectoryClient shareDirectoryClient, string directoryPrefix, string searchPrefix)
		{
			// speed up
			if (!String.IsNullOrEmpty(searchPrefix) && !(directoryPrefix.StartsWith(searchPrefix) || searchPrefix.StartsWith(directoryPrefix)))
			{
				yield break;
			}

			AsyncPageable<ShareFileItem> directoryItems = shareDirectoryClient.GetFilesAndDirectoriesAsync();
			List<string> subdirectories = new List<string>();

			await foreach (ShareFileItem item in directoryItems.ConfigureAwait(false))
			{
				if (!item.IsDirectory)
				{
					yield return new FileInfo
					{
						Name = directoryPrefix + item.Name,
						LastModifiedUtc = default(DateTime),
						Size = item.FileSize ?? -1,
						ContentType = null
					};
				}
				else
				{
					subdirectories.Add(item.Name);
				}
			}

			foreach (string subdirectory in subdirectories)
			{
				var subdirectoryItems = EnumerateFiles_ListFilesInHierarchyInternalAsync(shareDirectoryClient.GetSubdirectoryClient(subdirectory), directoryPrefix + subdirectory + '/', searchPrefix);

				await foreach (var subdirectoryItem in subdirectoryItems.ConfigureAwait(false))
				{
					yield return subdirectoryItem;
				}
			}
		}

		private bool EnumerateFiles_FilterFileInfo(FileInfo fileInfo, string searchPattern)
		{
			if ((searchPattern != null) && !RegexPatterns.IsFileWildcardMatch(fileInfo.Name, searchPattern))
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Vrátí čas poslední modifikace souboru v UTC timezone.
		/// </summary>
		public override DateTime? GetLastModifiedTimeUtc(string fileName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName));

			ShareFileClient shareFileClient = GetShareFileClient(fileName);			
			ShareFileProperties properties = shareFileClient.GetProperties();
			return properties.LastModified.UtcDateTime;
		}

		/// <summary>
		/// Vrátí čas poslední modifikace souboru v UTC timezone.
		/// </summary>
		public override async Task<DateTime?> GetLastModifiedTimeUtcAsync(string fileName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName));

			ShareFileClient file = GetShareFileClient(fileName); // nechceme zakládat složku, můžeme použít synchronní kód v asynchronní metodě
			ShareFileProperties properties = await file.GetPropertiesAsync().ConfigureAwait(false);
			return properties.LastModified.UtcDateTime;
		}

		/// <summary>
		/// Vrátí ShareFileClient pro daný soubor ve FileShare používaného Azure Storage Accountu.
		/// </summary>
		protected ShareFileClient GetShareFileClient(string fileName, bool createDirectoryStructure = false)
		{
			var shareDirectoryClient = GetRootShareDirectoryClient();

			string[] segments = fileName.Replace("\\", "/").Split('/');
			if (segments.Length > 1)
			{
				for (int i = 0; i < segments.Length - 1; i++)
				{
					shareDirectoryClient = shareDirectoryClient.GetSubdirectoryClient(segments[i]);
					if (createDirectoryStructure)
					{
						shareDirectoryClient.CreateIfNotExists();
					}
				}
			}
			return shareDirectoryClient.GetFileClient(segments.Last());
		}

		/// <summary>
		/// Vrátí ShareFileClient pro daný soubor ve FileShare používaného Azure Storage Accountu.
		/// </summary>
		protected async Task<ShareFileClient> GetShareFileClientAsync(string fileName, bool createDirectoryStructure = false)
		{
			var shareDirectoryClient = GetRootShareDirectoryClient();

			string[] segments = fileName.Replace("\\", "/").Split('/');
			if (segments.Length > 1)
			{
				for (int i = 0; i < segments.Length - 1; i++)
				{
					shareDirectoryClient = shareDirectoryClient.GetSubdirectoryClient(segments[i]);
					if (createDirectoryStructure)
					{
						await shareDirectoryClient.CreateIfNotExistsAsync().ConfigureAwait(false);
					}
				}
			}
			return shareDirectoryClient.GetFileClient(segments.Last());
		}

		/// <summary>
		/// Vrátí ShareDirectoryClient reprezentující rootovou složku pro práci s Azure File Storage.
		/// Pokud je požadována jiná root directory, je vracena tato složka.
		/// </summary>
		protected ShareDirectoryClient GetRootShareDirectoryClient()
		{
			var shareReference = GetShareClient();
			var result = shareReference.GetRootDirectoryClient();

			if (rootDirectoryNameSegments.Length > 0)
			{
				for (int i = 0; i < rootDirectoryNameSegments.Length; i++)
				{
					result = result.GetSubdirectoryClient(rootDirectoryNameSegments[i]);
				}
			}

			return result;
		}

		/// <summary>
		/// Vrátí používaný ShareClient v Azure Storage Accountu.
		/// </summary>
		protected internal ShareClient GetShareClient()
		{
			return new ShareClient(options.FileStorageConnectionString, options.FileShareName);			
		}

		/// <summary>
		/// Vytvoří úložiště souborů (a ev. root directory), pokud ještě neexistuje.
		/// </summary>
		protected void EnsureFileShare()
		{
			if (!fileShareAlreadyCreated)
			{
				var shareClient = GetShareClient();
				shareClient.CreateIfNotExists();

				var directory = shareClient.GetRootDirectoryClient();
				if (rootDirectoryNameSegments.Length > 0)
				{
					for (int i = 0; i < rootDirectoryNameSegments.Length; i++)
					{
						directory = directory.GetSubdirectoryClient(rootDirectoryNameSegments[i]);
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
				var shareClient = GetShareClient();
				await shareClient.CreateIfNotExistsAsync().ConfigureAwait(false);

				var directory = shareClient.GetRootDirectoryClient();
				if (rootDirectoryNameSegments.Length > 0)
				{
					for (int i = 0; i < rootDirectoryNameSegments.Length; i++)
					{
						directory = directory.GetSubdirectoryClient(rootDirectoryNameSegments[i]);
						await directory.CreateIfNotExistsAsync().ConfigureAwait(false);
					}
				}

				fileShareAlreadyCreated = true;
			}
		}
	}
}