﻿using System.Runtime.CompilerServices;
using Havit.Linq;
using Havit.Diagnostics.Contracts;
using Havit.Text.RegularExpressions;

namespace Havit.Services.FileStorage;

/// <summary>
/// IFileStorageService s file systémem pro datové úložiště.
/// Některé asynchronní metody pod pokličkou nejsou asynchronní, viz dokumentace jednotlivých metod (jejichž název končí Async).
/// </summary>
/// <remarks>
/// Negenerickou třídu držíme pro zpětnou kompatibilitu.
/// </remarks>
public class FileSystemStorageService : FileStorageServiceBase, IFileStorageService
{
	internal string StoragePath { get; init; }
	private bool useFullyQualifiedPathNames;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	/// <param name="storagePath">Cesta k "rootu" použitého úložiště ve file systému.</param>
	public FileSystemStorageService(string storagePath) : this(storagePath, false, null)
	{
		// NOOP
	}

	/// <summary>
	/// Konstruktor.
	/// </summary>
	/// <param name="storagePath">Cesta k "rootu" použitého úložiště ve file systému.</param>
	/// <param name="useFullyQualifiedPathNames">Cestar k "rootu" nebude použita a veškerá volání budou kvalifikována plnou cestou.</param>
	/// <param name="encryptionOptions">Parametry pro šifrování storage. Nepovinné.</param>
	/// <exception cref="ArgumentException">StoragePath is not empty and UseFullPath is true.</exception>
	/// <exception cref="ArgumentException">StoragePath is empty and UseFullPath is false.</exception>
	public FileSystemStorageService(string storagePath, bool useFullyQualifiedPathNames, EncryptionOptions encryptionOptions) : base(encryptionOptions)
	{
		Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(storagePath) ^ useFullyQualifiedPathNames, "Je nutno zadat buď cestu k úložišti anebo zvolit použití plně kvalifikovaných názvu souborů, přičemž nelze obojí současně.");
		this.StoragePath = storagePath?.Replace("%TEMP%", Path.GetTempPath().TrimEnd(Path.DirectorySeparatorChar));
		this.useFullyQualifiedPathNames = useFullyQualifiedPathNames;
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
	public override Task<bool> ExistsAsync(string fileName, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(Exists(fileName));
	}

	/// <summary>
	/// Vrátí stream s obsahem soubor z úložiště.
	/// </summary>
	protected override Stream PerformOpenRead(string fileName)
	{
		return new FileStream(GetFullPath(fileName), FileMode.Open, FileAccess.Read, FileShare.Read, 81920, FileOptions.RandomAccess);
	}

	/// <summary>
	/// Vrátí stream s obsahem soubor z úložiště.
	/// Nemá asynchronní implementaci, spouští synchronní PerformRead.
	/// </summary>
	protected override Task<Stream> PerformOpenReadAsync(string fileName, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(PerformOpenRead(fileName));
	}


	/// <summary>
	/// Zapíše obsah souboru z úložiště do streamu.
	/// </summary>
	protected override void PerformReadToStream(string fileName, Stream stream)
	{
		using (Stream fileStream = PerformOpenRead(fileName))
		{
			fileStream.CopyTo(stream);
		}
	}

	/// <summary>
	/// Zapíše obsah souboru z úložiště do streamu.
	/// </summary>
	protected override async Task PerformReadToStreamAsync(string fileName, Stream stream, CancellationToken cancellationToken = default)
	{
		using (Stream fileStream = PerformOpenRead(fileName))
		{
			await fileStream.CopyToAsync(stream, 81920 /* default */, cancellationToken).ConfigureAwait(false);
		}
	}

	/// <summary>
	/// Uloží stream do úložiště.
	/// </summary>
	protected override void PerformSave(string fileName, Stream fileContent, string contentType)
	{
		EnsureDirectoryFor(fileName);

		using (FileStream fileStream = new FileStream(GetFullPath(fileName), FileMode.Create, FileAccess.Write, FileShare.None, 81920))
		{
			fileContent.CopyTo(fileStream);
		}
	}

	/// <summary>
	/// Uloží stream do úložiště.
	/// </summary>
	protected override async Task PerformSaveAsync(string fileName, Stream fileContent, string contentType, CancellationToken cancellationToken = default)
	{
		EnsureDirectoryFor(fileName);

		using (FileStream fileStream = new FileStream(GetFullPath(fileName), FileMode.Create, FileAccess.Write, FileShare.None, 81920))
		{
			await fileContent.CopyToAsync(fileStream, 81920 /* default */, cancellationToken).ConfigureAwait(false);
		}
	}

	protected override Stream PerformOpenCreate(string fileName, string contentType)
	{
		return new FileStream(GetFullPath(fileName), FileMode.Create, FileAccess.Write, FileShare.None, 81920);
	}

	protected override Task<Stream> PerformOpenCreateAsync(string fileName, string contentType, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		return Task.FromResult(PerformOpenCreate(fileName, contentType)); // no async version
	}

	/// <summary>
	/// Zajistí vytvoření cílové složky, pokud je název souboru včetně složky.
	/// </summary>
	private void EnsureDirectoryFor(string fileName)
	{
		var directory = Path.GetDirectoryName(fileName);
		if (!String.IsNullOrWhiteSpace(directory))
		{
			Directory.CreateDirectory(GetFullPath(directory));
		}
	}

	/// <inheritdoc />
	protected override void PerformCopy(string sourceFileName, IFileStorageService targetFileStorageService, string targetFileName)
	{
		if ((targetFileStorageService is FileSystemStorageService targetFileSystemStorageService) && !this.SupportsBasicEncryption && !targetFileSystemStorageService.SupportsBasicEncryption)
		{
			targetFileSystemStorageService.EnsureDirectoryFor(targetFileName);
			File.Copy(GetFullPath(sourceFileName), targetFileSystemStorageService.GetFullPath(targetFileName), overwrite: true);
		}
		else
		{
			base.PerformCopy(sourceFileName, targetFileStorageService, targetFileName);
		}
	}

	/// <inheritdoc />
	protected override void PerformMove(string sourceFileName, string targetFileName)
	{
		EnsureDirectoryFor(targetFileName);
		if (File.Exists(GetFullPath(targetFileName)))
		{
			File.Delete(GetFullPath(targetFileName));
		}
		File.Move(GetFullPath(sourceFileName), GetFullPath(targetFileName));
	}

	/// <inheritdoc />
	protected override Task PerformMoveAsync(string sourceFileName, string targetFileName, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		// Předpokládáme, že přejmenovat (přesunout soubor) v rámci jednoho FileSystemStorageService neasynchronně, je efektivnější,
		// než přesun souboru přes streamy.
		PerformMove(sourceFileName, targetFileName);

		return Task.CompletedTask;
	}

	/// <inheritdoc />
	protected override void PerformMove(string sourceFileName, IFileStorageService targetFileStorageService, string targetFileName)
	{
		if ((targetFileStorageService is FileSystemStorageService targetFileSystemStorageService) && !this.SupportsBasicEncryption && !targetFileSystemStorageService.SupportsBasicEncryption)
		{
			targetFileSystemStorageService.EnsureDirectoryFor(targetFileName);
			if (File.Exists(targetFileSystemStorageService.GetFullPath(targetFileName)))
			{
				File.Delete(targetFileSystemStorageService.GetFullPath(targetFileName));
			}
			File.Move(GetFullPath(sourceFileName), targetFileSystemStorageService.GetFullPath(targetFileName));
		}
		else
		{
			base.PerformMove(sourceFileName, targetFileStorageService, targetFileName);
		}
	}

	protected override async Task PerformMoveAsync(string sourceFileName, IFileStorageService targetFileStorageService, string targetFileName, CancellationToken cancellationToken = default)
	{
		if ((targetFileStorageService is FileSystemStorageService targetFileSystemStorageService) && !this.SupportsBasicEncryption && !targetFileSystemStorageService.SupportsBasicEncryption)
		{
			cancellationToken.ThrowIfCancellationRequested();

			targetFileSystemStorageService.EnsureDirectoryFor(targetFileName);
			// Předpokládáme, že přejmenovat (přesunout soubor) v rámci jednoho FileSystemStorageService neasynchronně, je efektivnější,
			// než přesun souboru přes streamy v bázové třídě asynchronně.
			if (File.Exists(targetFileSystemStorageService.GetFullPath(targetFileName)))
			{
				File.Delete(targetFileSystemStorageService.GetFullPath(targetFileName));
			}
			File.Move(GetFullPath(sourceFileName), targetFileSystemStorageService.GetFullPath(targetFileName));
		}
		else
		{
			await base.PerformMoveAsync(sourceFileName, targetFileStorageService, targetFileName, cancellationToken).ConfigureAwait(false);
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
	public override Task DeleteAsync(string fileName, CancellationToken cancellationToken = default)
	{
		Delete(fileName);
		return Task.CompletedTask;
	}

	/// <summary>
	/// Vylistuje seznam souborů v úložišti. ContentType položek je vždy null.
	/// </summary>
	public override IEnumerable<FileInfo> EnumerateFiles(string searchPattern = null)
	{
		#region Local function ExecuteWithDirectoryNotFoundExceptionHandling
		IEnumerable<System.IO.FileInfo> ExecuteWithDirectoryNotFoundExceptionHandling(Func<IEnumerable<System.IO.FileInfo>> func)
		{
			try
			{
				return func();
			}
			catch (DirectoryNotFoundException)
			{
				return Enumerable.Empty<System.IO.FileInfo>();
			}
		}
		#endregion

		// zpětná lomítka potřebujeme v searchpatterns pro RegexPatterns.IsFileWildcardMatch
		searchPattern = searchPattern?.Replace("/", "\\");

		// ziskej prefix, uvodni cast cesty, ve kterem nejsou pouzite znaky '*' a '?'
		// EnumerableFilesGetPrefix předpokládá členění dle běžných lomítek, nikoliv dle zpětných
		string prefix = EnumerableFilesGetPrefix(searchPattern?.Replace("\\", "/"));

		IEnumerable<System.IO.FileInfo> filesEnumerable;
		if (!useFullyQualifiedPathNames)
		{
			// nacti soubory z oblasti dane storagePath a prefixem
			filesEnumerable = ExecuteWithDirectoryNotFoundExceptionHandling(() =>
					new System.IO.DirectoryInfo(Path.Combine(StoragePath, prefix ?? String.Empty))
					.EnumerateFiles("*", SearchOption.AllDirectories)
					.WhereIf(searchPattern != null, item => RegexPatterns.IsFileWildcardMatch(item.FullName.Substring(StoragePath.Length + 1), searchPattern))); // vyfiltruj validni soubory podle souboroveho wildcards
		}
		else
		{
			VerifyPathIsFullyQualified(searchPattern);

			// prefix musí být not null, jinak není cesta validní
			// pokud je použito např. D:\Nu*, vrací prefix jen "D:", chceme však použít "D:\".
			if (prefix.EndsWith(":"))
			{
				prefix += '\\';
			}

			filesEnumerable = ExecuteWithDirectoryNotFoundExceptionHandling(() => new System.IO.DirectoryInfo(prefix)
				.EnumerateFiles("*", SearchOption.AllDirectories)
				.Where(item => RegexPatterns.IsFileWildcardMatch(item.FullName, searchPattern)));
		}

		return filesEnumerable.Select(fileInfo => new FileInfo
		{
			// Zamen souborova '\\' za azure blobova '/'. Toto je dohoda, ze interface IFileStorageService.EnumerateFiles vraci vzdy cestu s '/' a ne s '\\'.
			// Odmaz storage path - misto, kde jsou soubory ulozeny fyzicky na disku. To je soucasti storagePath z konstruktoru.
			Name = (useFullyQualifiedPathNames ? fileInfo.FullName : fileInfo.FullName.Substring(StoragePath.Length + 1)).Replace("\\", "/"),
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
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public override async IAsyncEnumerable<FileInfo> EnumerateFilesAsync(string pattern = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		// no await here
		// použijeme synchronní variantu, asynchronní nemáme
		foreach (FileInfo fileInfo in EnumerateFiles(pattern))
		{
			cancellationToken.ThrowIfCancellationRequested();
			yield return fileInfo;
		}
	}

	/// <summary>
	/// Vrátí čas poslední modifikace souboru v UTC timezone.
	/// </summary>
	public override DateTime? GetLastModifiedTimeUtc(string fileName)
	{
		Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(fileName), nameof(fileName));

		return File.GetLastWriteTimeUtc(GetFullPath(fileName));
	}

	/// <summary>
	/// Vrátí čas poslední modifikace souboru v UTC timezone.
	/// Nemá asynchronní implementaci, spouští synchronní GetLastModifiedTimeUtc.
	/// </summary>
	public override Task<DateTime?> GetLastModifiedTimeUtcAsync(string fileName, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(GetLastModifiedTimeUtc(fileName));
	}

	internal string GetFullPath(String fileNamePath)
	{
		if (useFullyQualifiedPathNames)
		{
			VerifyPathIsFullyQualified(fileNamePath);
			return fileNamePath;
		}
		else
		{
			String fileNameFullPath = Path.Combine(StoragePath, fileNamePath);

			DirectoryInfo storagePathDirectoryInfo = new System.IO.DirectoryInfo(StoragePath);
			DirectoryInfo fileNameDirectoryInfo = (new System.IO.FileInfo(fileNameFullPath)).Directory;

			if (!IsPathInsideFolder(fileNameDirectoryInfo, storagePathDirectoryInfo))
			{
				throw new InvalidOperationException("Cesta k souboru vede mimo složku úložiště.");
			}

			return fileNameFullPath;
		}
	}

	private static void VerifyPathIsFullyQualified(string path)
	{
		if (!IsPathFullyQualified(path))
		{
			throw new InvalidOperationException("Cesta k souboru musí být zadána jako plně kvalifikovaná (vč. disku, od rootu).");
		}
	}

	internal static bool IsPathFullyQualified(string path)
	{
		// Path.IsPathFullyQualified není součástí .NET Frameworku ani .NET Standard 2.0 (je v .NET Standard 2.1)
		// navíc nám moc nepomáhá ani jako inspirace

		return !String.IsNullOrWhiteSpace(path)
		   && (( /* local path */
				(path.Length >= 3)
				&& (((path[0] >= 'A') && (path[0] <= 'Z')) || ((path[0] >= 'a') && (path[0] <= 'z')))
				&& (path[1] == ':')
				&& ((path[2] == '/') || (path[2] == '\\')))
			|| ( /* UNC */
				(path.Length >= 4)
				&& (path[0] == '\\')
				&& (path[1] == '\\')
				&& (((path[2] >= 'A') && (path[2] <= 'Z')) || ((path[2] >= 'a') && (path[2] <= 'z')))
				&& (path.LastIndexOf('\\') > 2)));
	}

	private bool IsPathInsideFolder(DirectoryInfo filePath, DirectoryInfo storageDirectory)
	{
		if (filePath == null)
		{
			return false;
		}

		if (String.Equals(filePath.FullName.TrimEnd(Path.DirectorySeparatorChar), storageDirectory.FullName.TrimEnd(Path.DirectorySeparatorChar), StringComparison.InvariantCultureIgnoreCase))
		{
			return true;
		}

		return IsPathInsideFolder(filePath.Parent, storageDirectory);
	}

	protected override string GetContentType(string fileName)
	{
		return null; // FileSystem nepoužívá content types
	}

	protected override ValueTask<string> GetContentTypeAsync(string fileName, CancellationToken cancellationToken)
	{
		return new ValueTask<string>((string)null); // FileSystem nepoužívá content types
	}
}
