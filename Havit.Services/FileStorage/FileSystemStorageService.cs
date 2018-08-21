using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Havit.Diagnostics.Contracts;
using Havit.Text.RegularExpressions;

namespace Havit.Services.FileStorage
{
	/// <summary>
	/// IFileStorageService a IFileStorageServiceAsync s file systémem pro datové úložiště.
	/// Některé asynchronní metody pod pokličkou nejsou asynchronní, viz dokumentace jednotlivých metod (jejichž název končí Async).
	/// </summary>
	public class FileSystemStorageService : FileStorageServiceBase, IFileStorageService, IFileStorageServiceAsync
	{
		private readonly string storagePath;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="storagePath">Cesta k "rootu" použitého úložiště ve file systému.</param>
		public FileSystemStorageService(string storagePath) : this(storagePath, null)
		{
		}

		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="storagePath">Cesta k "rootu" použitého úložiště ve file systému.</param>
		/// <param name="encryptionOptions">Parametry pro šifrování storage. Nepovinné.</param>
		public FileSystemStorageService(string storagePath, EncryptionOptions encryptionOptions) : base(encryptionOptions)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(storagePath));
			this.storagePath = storagePath;
		}

		/// <summary>
		/// Vrátí true, pokud uložený soubor v úložišti existuje. Jinak false.
		/// </summary>
		public override bool Exists(string fileName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName));
			return File.Exists(GetFullPath(fileName));
		}

		/// <summary>
		/// Vrátí true, pokud uložený soubor v úložišti existuje. Jinak false.
		/// Nemá asynchronní implementaci, spouští synchronní Exists.
		/// </summary>
		public override Task<bool> ExistsAsync(string fileName)
		{
			return Task.FromResult(Exists(fileName));
		}

		/// <summary>
		/// Vrátí stream s obsahem soubor z úložiště.
		/// </summary>
		protected override Stream PerformRead(string fileName)
		{
			return File.OpenRead(GetFullPath(fileName));
		}

		/// <summary>
		/// Vrátí stream s obsahem soubor z úložiště.
		/// Nemá asynchronní implementaci, spouští synchronní PerformRead.
		/// </summary>
		protected override Task<Stream> PerformReadAsync(string fileName)
		{			
			return Task.FromResult(PerformRead(fileName));
		}

		/// <summary>
		/// Zapíše obsah souboru z úložiště do streamu.
		/// </summary>
		protected override void PerformReadToStream(string fileName, Stream stream)
		{
			using (Stream fileStream = PerformRead(fileName))
			{
				fileStream.CopyTo(stream);
			}
		}

		/// <summary>
		/// Zapíše obsah souboru z úložiště do streamu.
		/// </summary>
		protected override async Task PerformReadToStreamAsync(string fileName, Stream stream)
		{
			using (Stream fileStream = PerformRead(fileName))
			{
				await fileStream.CopyToAsync(stream);
			}
		}

		/// <summary>
		/// Uloží stream do úložiště.
		/// </summary>
		protected override void PerformSave(string fileName, Stream fileContent, string contentType)
		{
			PerformSave_EnsureDirectory(fileName);

			using (FileStream fileStream = File.Create(GetFullPath(fileName)))
			{
				fileContent.CopyTo(fileStream);
			}
		}

		/// <summary>
		/// Uloží stream do úložiště.
		/// </summary>
		protected override async Task PerformSaveAsync(string fileName, Stream fileContent, string contentType)
		{		
			PerformSave_EnsureDirectory(fileName);

			using (FileStream fileStream = File.Create(GetFullPath(fileName)))
			{
				await fileContent.CopyToAsync(fileStream);
			}
		}

		/// <summary>
		/// Zajistí vytvoření cílové složky, pokud je název souboru včetně složky.
		/// </summary>
		private void PerformSave_EnsureDirectory(string fileName)
		{
			var directory = Path.GetDirectoryName(fileName);
			if (!String.IsNullOrWhiteSpace(directory))
			{
				Directory.CreateDirectory(GetFullPath(directory));
			}
		}

		/// <summary>
		/// Smaže soubor v úložišti.
		/// </summary>
		public override void Delete(string fileName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName));
			System.IO.File.Delete(GetFullPath(fileName));
		}

		/// <summary>
		/// Smaže soubor v úložišti.
		/// Nemá asynchronní implementaci, spouští synchronní Exists.
		/// </summary>
		public override Task DeleteAsync(string fileName)
		{
			Delete(fileName);
			return Task.CompletedTask;
		}

		/// <summary>
		/// Vylistuje seznam souborů v úložišti. ContentType položek je vždy null.
		/// </summary>
		public override IEnumerable<FileInfo> EnumerateFiles(string searchPattern = null)
		{
			if (!String.IsNullOrWhiteSpace(searchPattern))
			{
				// zamen azure blobova '/' za '\\', ktere lze pouzit v souborovem systemu
				searchPattern = searchPattern.Replace("/", "\\");
			}

			// ziskej prefix, uvodni cast cesty, ve kterem nejsou pouzite znaky '*' a '?'
			string prefix = EnumerableFilesGetPrefix(searchPattern);

			// nacti soubory z oblasti dane storagePath a prefixem
			IEnumerable<System.IO.FileInfo> filesEnumerable = 
				new System.IO.DirectoryInfo(Path.Combine(storagePath, prefix ?? String.Empty))
				.EnumerateFiles("*", SearchOption.AllDirectories);

			if (searchPattern != null)
			{
				// vyfiltruj validni soubory podle souboroveho wildcards
				filesEnumerable = filesEnumerable.Where(item => RegexPatterns.IsFileWildcardMatch(item.FullName.Substring(storagePath.Length + 1), searchPattern));
			}

			return filesEnumerable.Select(fileInfo => new FileInfo
			{
				// Zamen souborova '\\' za azure blobova '/'. Toto je dohoda, ze interface IFileStorageService.EnumerateFiles vraci vzdy cestu s '/' a ne s '\\'.
				// Odmaz storage path - misto, kde jsou soubory ulozeny fyzicky na disku. To je soucasti storagePath z konstruktoru.
				Name = fileInfo.FullName.Substring(storagePath.Length + 1).Replace("\\", "/"),
				LastModifiedUtc = fileInfo.LastWriteTimeUtc,
				Size = fileInfo.Length,
				ContentType = null
			});
		}

		/// <summary>
		/// Vylistuje seznam souborů v úložišti.
		/// ContentType položek je vždy null.
		/// Nemá asynchronní implementaci, spouští synchronní EnumerateFiles.
		/// </summary>
		public override Task<IEnumerable<FileInfo>> EnumerateFilesAsync(string pattern = null)
		{
			return Task.FromResult(EnumerateFiles(pattern));
		}

		/// <summary>
		/// Vrátí čas poslední modifikace souboru v UTC timezone.
		/// </summary>
		public override DateTime? GetLastModifiedTimeUtc(string fileName)
		{
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName));
			return File.GetLastWriteTimeUtc(GetFullPath(fileName));
		}

		/// <summary>
		/// Vrátí čas poslední modifikace souboru v UTC timezone.
		/// Nemá asynchronní implementaci, spouští synchronní GetLastModifiedTimeUtc.
		/// </summary>
		public override Task<DateTime?> GetLastModifiedTimeUtcAsync(string fileName)
		{
			return Task.FromResult(GetLastModifiedTimeUtc(fileName));
		}

		internal string GetFullPath(String fileNamePath)
		{
			String fileNameFullPath = Path.Combine(storagePath, fileNamePath);

			DirectoryInfo storagePathDirectoryInfo = new System.IO.DirectoryInfo(storagePath);
			DirectoryInfo fileNameDirectoryInfo = (new System.IO.FileInfo(fileNameFullPath)).Directory;

			if (!IsPathInsideFolder(fileNameDirectoryInfo, storagePathDirectoryInfo))
			{
				throw new InvalidOperationException("Cesta k soubor vede mimo složku úložiště.");
			}

			return fileNameFullPath;
		}

		private bool IsPathInsideFolder(DirectoryInfo filePath, DirectoryInfo storageDirectory)
		{
			if (filePath == null)
			{
				return false;
			}

			if (String.Equals(filePath.FullName.TrimEnd('\\'), storageDirectory.FullName.TrimEnd('\\'), StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}

			return IsPathInsideFolder(filePath.Parent, storageDirectory);
		}
	}
}
