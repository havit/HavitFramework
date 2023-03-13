using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Havit.Diagnostics.Contracts;
using Havit.Services.FileStorage;
using Havit.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
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

		private readonly Lazy<ShareClient> shareClientLazy;
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
			this.shareClientLazy = new Lazy<ShareClient>(CreateShareClient, System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
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
		public override async Task<bool> ExistsAsync(string fileName, CancellationToken cancellationToken = default)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName), nameof(fileName));

			ShareFileClient shareFileClient = GetShareFileClient(fileName); // nechceme zakládat složku, můžeme použít synchronní kód v asynchronní metodě
			return await shareFileClient.ExistsAsync(cancellationToken).ConfigureAwait(false);
		}

		/// <summary>
		/// Zapíše obsah souboru z úložiště do streamu.
		/// </summary>
		protected override void PerformReadToStream(string fileName, System.IO.Stream stream)
		{
			ShareFileClient shareFileClient = GetShareFileClient(fileName);
			using (System.IO.Stream azureFileStream = shareFileClient.OpenRead())
			{
				azureFileStream.CopyTo(stream);
			}
		}

		/// <summary>
		/// Zapíše obsah souboru z úložiště do streamu.
		/// </summary>
		protected override async Task PerformReadToStreamAsync(string fileName, System.IO.Stream stream, CancellationToken cancellationToken = default)
		{
			ShareFileClient shareFileClient = GetShareFileClient(fileName); // nechceme zakládat složku, můžeme použít synchronní kód v asynchronní metodě
			using (System.IO.Stream azureFileStream = await shareFileClient.OpenReadAsync(cancellationToken: cancellationToken).ConfigureAwait(false))
			{
				await azureFileStream.CopyToAsync(stream, 81920 /* default*/, cancellationToken).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Vrátí stream s obsahem soubor z úložiště.
		/// </summary>
		protected override System.IO.Stream PerformOpenRead(string fileName)
		{
			ShareFileClient shareFileClient = GetShareFileClient(fileName);
			return shareFileClient.OpenRead();
		}

		/// <summary>
		/// Vrátí stream s obsahem soubor z úložiště.
		/// </summary>
		protected override async Task<System.IO.Stream> PerformOpenReadAsync(string fileName, CancellationToken cancellationToken = default)
		{
			ShareFileClient shareFileClient = GetShareFileClient(fileName); // nechceme zakládat složku, můžeme použít synchronní kód v asynchronní metodě
			return await shareFileClient.OpenReadAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
		}

		/// <summary>
		/// Uloží stream do úložiště.
		/// </summary>
		protected override void PerformSave(string fileName, System.IO.Stream fileContent, string contentType)
		{
			EnsureFileShare();

			ShareFileClient shareFileClient = GetShareFileClient(fileName, createDirectoryStructure: options.AutoCreateDirectories);

			System.IO.Stream seekableFileContent;
			bool seekableFileContentNeedsDispose;

			if (fileContent.CanSeek)
			{
				seekableFileContent = fileContent;
				seekableFileContentNeedsDispose = false;
			}
			else
			{
				seekableFileContent = new System.IO.MemoryStream();
				fileContent.CopyTo(seekableFileContent);
				seekableFileContentNeedsDispose = true;
			}

			try // jen pro finally + Dispose
			{
				long fileContentLength = seekableFileContent.Length; // zde nastavujeme velikost souboru, abychom mohli přečíst vlastnost Length, potřebujeme seekovatelný stream...
				shareFileClient.Create(fileContentLength); // zde nastavujeme velikost souboru (ačkoliv se to jmenuje maxsize)
				if (fileContentLength > 0) // upload contentu nemůžeme provádět pro prázdný stream
				{
					try
					{
						shareFileClient.Upload(seekableFileContent);
					}
					catch
					{
						try
						{
							shareFileClient.Delete(); // pokud se upload contentu nezdařil, odstraníme soubor založený přes Create.
						}
						catch
						{
							// NOOP
						}

						throw;
					}
				}
			}
			finally
			{
				if (seekableFileContentNeedsDispose)
				{
					seekableFileContent.Dispose();
				}
			}
		}

		/// <summary>
		/// Uloží stream do úložiště.
		/// </summary>
		protected override async Task PerformSaveAsync(string fileName, System.IO.Stream fileContent, string contentType, CancellationToken cancellationToken = default)
		{
			await EnsureFileShareAsync(cancellationToken).ConfigureAwait(false);

			ShareFileClient shareFileClient = await GetShareFileClientAsync(fileName, createDirectoryStructure: options.AutoCreateDirectories, cancellationToken: cancellationToken).ConfigureAwait(false);

			System.IO.Stream seekableFileContent;
			bool seekableFileContentNeedsDispose;

			if (fileContent.CanSeek)
			{
				seekableFileContent = fileContent;
				seekableFileContentNeedsDispose = false;
			}
			else
			{
				seekableFileContent = new System.IO.MemoryStream();
				await fileContent.CopyToAsync(seekableFileContent, 81920 /* default */, cancellationToken).ConfigureAwait(false);
				seekableFileContentNeedsDispose = true;
			}

			try // jen pro finally + Dispose
			{
				long fileContentLength = seekableFileContent.Length; // zde nastavujeme velikost souboru, abychom mohli přečíst vlastnost Length, potřebujeme seekovatelný stream...

				await shareFileClient.CreateAsync(fileContentLength, cancellationToken: cancellationToken).ConfigureAwait(false);
				if (fileContentLength > 0)
				{
					try
					{
						await shareFileClient.UploadAsync(seekableFileContent, cancellationToken: cancellationToken).ConfigureAwait(false);
					}
					catch
					{
						try
						{
							await shareFileClient.DeleteAsync(cancellationToken).ConfigureAwait(false);
						}
						catch
						{
							// NOOP
						}

						throw;
					}
				}
			}
			finally
			{
				if (seekableFileContentNeedsDispose)
				{
					seekableFileContent.Dispose();
				}
			}
		}

		/// <inheritdoc />
		protected override void PerformMove(string sourceFileName, string targetFileName)
		{
			ShareFileClient shareSourceFileClient = GetShareFileClient(sourceFileName);
			ShareFileClient shareTargetFileClient = GetShareFileClient(targetFileName, createDirectoryStructure: options.AutoCreateDirectories);

			shareSourceFileClient.Rename(shareTargetFileClient.Path, new ShareFileRenameOptions { ReplaceIfExists = true });
		}

		/// <inheritdoc />
		protected override async Task PerformMoveAsync(string sourceFileName, string targetFileName, CancellationToken cancellationToken)
		{
			ShareFileClient shareSourceFileClient = GetShareFileClient(sourceFileName);
			ShareFileClient shareTargetFileClient = await GetShareFileClientAsync(targetFileName, createDirectoryStructure: options.AutoCreateDirectories, cancellationToken).ConfigureAwait(false);

			await shareSourceFileClient.RenameAsync(shareTargetFileClient.Path, new ShareFileRenameOptions { ReplaceIfExists = true }, cancellationToken).ConfigureAwait(false);
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
		public override async Task DeleteAsync(string fileName, CancellationToken cancellationToken = default)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName));

			ShareFileClient shareFileClient = GetShareFileClient(fileName); // nechceme zakládat složku, můžeme použít synchronní kód v asynchronní metodě
			await shareFileClient.DeleteAsync(cancellationToken).ConfigureAwait(false);
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
		public override async IAsyncEnumerable<FileInfo> EnumerateFilesAsync(string searchPattern = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			if (!String.IsNullOrWhiteSpace(searchPattern))
			{
				// zamen souborova '\\' za '/', ktere lze pouzit v Azure blobu
				searchPattern = searchPattern.Replace("\\", "/");
			}

			// ziskej prefix, uvodni cast cesty, ve kterem nejsou pouzite znaky '*' a '?'
			string prefix = FileStorageServiceBase.EnumerableFilesGetPrefix(searchPattern);

			await EnsureFileShareAsync(cancellationToken).ConfigureAwait(false);

			await foreach (FileInfo fileInfo in EnumerateFiles_ListFilesInHierarchyInternalAsync(GetRootShareDirectoryClient(), "", prefix, cancellationToken).ConfigureAwait(false))
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

			Pageable<ShareFileItem> directoryItems = shareDirectoryClient.GetFilesAndDirectories(new ShareDirectoryGetFilesAndDirectoriesOptions { Traits = ShareFileTraits.Timestamps });
			List<string> subdirectories = new List<string>();

			foreach (ShareFileItem item in directoryItems)
			{
				if (!item.IsDirectory)
				{
					yield return new FileInfo
					{
						Name = directoryPrefix + item.Name,
						LastModifiedUtc = item.Properties?.LastModified?.UtcDateTime ?? default,
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

		private async IAsyncEnumerable<FileInfo> EnumerateFiles_ListFilesInHierarchyInternalAsync(ShareDirectoryClient shareDirectoryClient, string directoryPrefix, string searchPrefix, [EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			// speed up
			if (!String.IsNullOrEmpty(searchPrefix) && !(directoryPrefix.StartsWith(searchPrefix) || searchPrefix.StartsWith(directoryPrefix)))
			{
				yield break;
			}

			AsyncPageable<ShareFileItem> directoryItems = shareDirectoryClient.GetFilesAndDirectoriesAsync(new ShareDirectoryGetFilesAndDirectoriesOptions { Traits = ShareFileTraits.Timestamps }, cancellationToken: cancellationToken);
			List<string> subdirectories = new List<string>();

			await foreach (ShareFileItem item in directoryItems.ConfigureAwait(false))
			{
				if (!item.IsDirectory)
				{
					yield return new FileInfo
					{
						Name = directoryPrefix + item.Name,
						LastModifiedUtc = item.Properties?.LastModified?.UtcDateTime ?? default,
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
				var subdirectoryItems = EnumerateFiles_ListFilesInHierarchyInternalAsync(shareDirectoryClient.GetSubdirectoryClient(subdirectory), directoryPrefix + subdirectory + '/', searchPrefix, cancellationToken);

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
		public override async Task<DateTime?> GetLastModifiedTimeUtcAsync(string fileName, CancellationToken cancellationToken = default)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName));

			ShareFileClient file = GetShareFileClient(fileName); // nechceme zakládat složku, můžeme použít synchronní kód v asynchronní metodě
			ShareFileProperties properties = await file.GetPropertiesAsync(cancellationToken).ConfigureAwait(false);
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
		protected async Task<ShareFileClient> GetShareFileClientAsync(string fileName, bool createDirectoryStructure = false, CancellationToken cancellationToken = default)
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
						await shareDirectoryClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
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
			return shareClientLazy.Value;
		}

		/// <summary>
		/// Vytvoří používaný ShareClient v Azure Storage Accountu.
		/// </summary>
		private ShareClient CreateShareClient()
		{
			return new ShareClient(options.FileStorageConnectionString, options.FileShareName);
		}

		/// <summary>
		/// Vytvoří úložiště souborů (a ev. root directory), pokud ještě neexistuje (a je povoleno vytváření složek).
		/// </summary>
		protected void EnsureFileShare()
		{
			if (!fileShareAlreadyCreated)
			{
				var shareClient = GetShareClient();
				if (options.AutoCreateFileShare)
				{
					shareClient.CreateIfNotExists();
				}

				var directory = shareClient.GetRootDirectoryClient();
				if (rootDirectoryNameSegments.Length > 0)
				{
					for (int i = 0; i < rootDirectoryNameSegments.Length; i++)
					{
						directory = directory.GetSubdirectoryClient(rootDirectoryNameSegments[i]);
						if (options.AutoCreateDirectories)
						{
							directory.CreateIfNotExists();
						}
					}
				}

				fileShareAlreadyCreated = true;
			}
		}

		/// <summary>
		/// Vytvoří úložiště souborů (a ev. root directory), pokud ještě neexistuje (a je povoleno vytváření složek).
		/// </summary>
		protected async Task EnsureFileShareAsync(CancellationToken cancellationToken = default)
		{
			if (!fileShareAlreadyCreated && options.AutoCreateFileShare)
			{
				var shareClient = GetShareClient();
				if (options.AutoCreateFileShare)
				{
					await shareClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
				}

				var directory = shareClient.GetRootDirectoryClient();
				if (rootDirectoryNameSegments.Length > 0)
				{
					for (int i = 0; i < rootDirectoryNameSegments.Length; i++)
					{
						directory = directory.GetSubdirectoryClient(rootDirectoryNameSegments[i]);
						if (options.AutoCreateDirectories)
						{
							await directory.CreateIfNotExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
						}
					}
				}

				fileShareAlreadyCreated = true;
			}
		}

		/// <inheritdoc />
		protected override string GetContentType(string sourceFileName)
		{
			return GetShareFileClient(sourceFileName).GetProperties().Value.ContentType;
		}

		/// <inheritdoc />
		protected override async ValueTask<string> GetContentTypeAsync(string sourceFileName, CancellationToken cancellationToken)
		{
			return (await GetShareFileClient(sourceFileName).GetPropertiesAsync(cancellationToken).ConfigureAwait(false)).Value.ContentType;
		}

		/// <inheritdoc />
		protected override System.IO.Stream PerformOpenWrite(string fileName, string contentType)
		{
			return GetShareFileClient(fileName, createDirectoryStructure: options.AutoCreateDirectories).OpenWrite(true, 0);
		}

		/// <inheritdoc />
		protected override async Task<System.IO.Stream> PerformOpenWriteAsync(string fileName, string contentType, CancellationToken cancellationToken = default)
		{
			var shareFileClient = await GetShareFileClientAsync(fileName, createDirectoryStructure: options.AutoCreateDirectories, cancellationToken).ConfigureAwait(false);
			return await shareFileClient.OpenWriteAsync(true, 0, cancellationToken: cancellationToken).ConfigureAwait(false);
		}
	}
}