using System.Runtime.CompilerServices;
using Havit.Linq;
using Havit.Services.FileStorage;
using Havit.Text.RegularExpressions;
using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;

namespace Havit.Services.Sftp.FileStorage;

/// <summary>
/// Úložiště souborů jako klient SFTP serveru. Disposable!
/// </summary>
public class SftpStorageService : FileStorageServiceBase, IFileStorageService, IDisposable
{
	/// <summary>
	/// Konstruktor.
	/// </summary>
	public SftpStorageService(SftpStorageServiceOptions options)
	{
		this.options = options;
	}

	private readonly SftpStorageServiceOptions options;
	private SftpClient sftpClient;

	internal ISftpClient GetConnectedSftpClient()
	{
		if (sftpClient == null)
		{
			var connectionInfo = options.ConnectionInfoFunc();
			sftpClient = new SftpClient(connectionInfo);
		}

		if (!sftpClient.IsConnected)
		{
			sftpClient.Connect();
		}

		return sftpClient;
	}

	internal async ValueTask<ISftpClient> GetConnectedSftpClientAsync(CancellationToken cancellationToken)
	{
		if (sftpClient == null)
		{
			var connectionInfo = options.ConnectionInfoFunc();
			sftpClient = new SftpClient(connectionInfo);
		}

		if (!sftpClient.IsConnected)
		{
			await sftpClient.ConnectAsync(cancellationToken).ConfigureAwait(false);
		}

		return sftpClient;
	}

	/// <summary>
	/// Odpojí se od sFTP serveru, pokud 
	/// </summary>
	public void Disconnect()
	{
		if ((sftpClient != null) && sftpClient.IsConnected)
		{
			sftpClient?.Disconnect();
		}
	}

	/// <inheritdoc />
	public override void Delete(string fileName)
	{
		ISftpClient sftpClient = GetConnectedSftpClient();
		sftpClient.Delete(SubstituteFileName(fileName));
	}

	/// <inheritdoc />
	public override async Task DeleteAsync(string fileName, CancellationToken cancellationToken = default)
	{
		ISftpClient sftpClient = await GetConnectedSftpClientAsync(cancellationToken).ConfigureAwait(false);
		await sftpClient.DeleteAsync(SubstituteFileName(fileName), cancellationToken).ConfigureAwait(false);
	}

	/// <inheritdoc />
	public override IEnumerable<Havit.Services.FileStorage.FileInfo> EnumerateFiles(string searchPattern = null)
	{
		// zamen souborova '\\' za '/', ktere lze pouzit v Azure blobu
		searchPattern = searchPattern?.Replace("\\", "/");

		// ziskej prefix, uvodni cast cesty, ve kterem nejsou pouzite znaky '*' a '?'
		string prefix = EnumerableFilesGetPrefix(searchPattern);

		foreach (Havit.Services.FileStorage.FileInfo fileInfo in EnumerateFiles_ListFilesInHierarchyInternal("", prefix))
		{
			if (EnumerateFiles_FilterFileInfo(fileInfo, searchPattern))
			{
				yield return fileInfo;
			}
		}
	}

	/// <inheritdoc />
	public override async IAsyncEnumerable<Services.FileStorage.FileInfo> EnumerateFilesAsync(string searchPattern = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		// zamen souborova '\\' za '/', ktere lze pouzit v Azure blobu
		searchPattern = searchPattern?.Replace("\\", "/");

		// ziskej prefix, uvodni cast cesty, ve kterem nejsou pouzite znaky '*' a '?'
		string prefix = EnumerableFilesGetPrefix(searchPattern);

		await foreach (Havit.Services.FileStorage.FileInfo fileInfo in EnumerateFiles_ListFilesInHierarchyInternalAsync("", prefix, cancellationToken).ConfigureAwait(false))
		{
			if (EnumerateFiles_FilterFileInfo(fileInfo, searchPattern))
			{
				yield return fileInfo;
			}
		}
	}

	private IEnumerable<Havit.Services.FileStorage.FileInfo> EnumerateFiles_ListFilesInHierarchyInternal(string directoryPrefix, string searchPrefix)
	{
		// speed up
		if (!String.IsNullOrEmpty(searchPrefix) && !(directoryPrefix.StartsWith(searchPrefix) || searchPrefix.StartsWith(directoryPrefix)))
		{
			yield break;
		}

		IEnumerable<ISftpFile> sftpFiles;

		try
		{
			sftpFiles = GetConnectedSftpClient().ListDirectory(directoryPrefix);
		}
		catch (SftpPathNotFoundException)
		{
			yield break;
		}

		List<string> subdirectories = new List<string>();

		foreach (SftpFile item in sftpFiles)
		{
			if ((item.Name == ".") || (item.Name == ".."))
			{
				// NOOP
			}
			else if (item.IsRegularFile)
			{
				yield return new Havit.Services.FileStorage.FileInfo
				{
					Name = directoryPrefix + item.Name,
					LastModifiedUtc = item.LastWriteTimeUtc,
					Size = item.Length,
					ContentType = null
				};
			}
			else if (item.IsDirectory)
			{
				subdirectories.Add(item.Name);
			}
			else
			{
				throw new InvalidOperationException($"Unknown SftpFile item ({item.ToString()}).");
			}
		}

		foreach (string subdirectory in subdirectories)
		{
			var subdirectoryItems = EnumerateFiles_ListFilesInHierarchyInternal(directoryPrefix + subdirectory + '/', searchPrefix);

			foreach (var subdirectoryItem in subdirectoryItems)
			{
				yield return subdirectoryItem;
			}
		}
	}

	private async IAsyncEnumerable<Havit.Services.FileStorage.FileInfo> EnumerateFiles_ListFilesInHierarchyInternalAsync(string directoryPrefix, string searchPrefix, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		// speed up
		if (!String.IsNullOrEmpty(searchPrefix) && !(directoryPrefix.StartsWith(searchPrefix) || searchPrefix.StartsWith(directoryPrefix)))
		{
			yield break;
		}

		IAsyncEnumerable<ISftpFile> sftpFiles;

		try
		{
			var sftpClient = await GetConnectedSftpClientAsync(cancellationToken).ConfigureAwait(false);
			sftpFiles = sftpClient.ListDirectoryAsync(directoryPrefix, cancellationToken);
		}
		catch (SftpPathNotFoundException)
		{
			yield break;
		}

		List<string> subdirectories = new List<string>();

		await foreach (SftpFile item in sftpFiles.ConfigureAwait(false))
		{
			if ((item.Name == ".") || (item.Name == ".."))
			{
				// NOOP
			}
			else if (item.IsRegularFile)
			{
				yield return new Havit.Services.FileStorage.FileInfo
				{
					Name = directoryPrefix + item.Name,
					LastModifiedUtc = item.LastWriteTimeUtc,
					Size = item.Length,
					ContentType = null
				};
			}
			else if (item.IsDirectory)
			{
				subdirectories.Add(item.Name);
			}
			else
			{
				throw new InvalidOperationException($"Unknown SftpFile item ({item.ToString()}).");
			}
		}

		foreach (string subdirectory in subdirectories)
		{
			var subdirectoryItems = EnumerateFiles_ListFilesInHierarchyInternalAsync(directoryPrefix + subdirectory + '/', searchPrefix, cancellationToken);

			await foreach (var subdirectoryItem in subdirectoryItems.ConfigureAwait(false))
			{
				yield return subdirectoryItem;
			}
		}
	}

	private bool EnumerateFiles_FilterFileInfo(Havit.Services.FileStorage.FileInfo fileInfo, string searchPattern)
	{
		if ((searchPattern != null) && !RegexPatterns.IsFileWildcardMatch(fileInfo.Name, searchPattern))
		{
			return false;
		}

		return true;
	}

	/// <inheritdoc />
	public override bool Exists(string fileName)
	{
		var sftpClient = GetConnectedSftpClient();
		return sftpClient.Exists(SubstituteFileName(fileName));
	}

	/// <inheritdoc />
	public override async Task<bool> ExistsAsync(string fileName, CancellationToken cancellationToken = default)
	{
		var sftpClient = await GetConnectedSftpClientAsync(cancellationToken).ConfigureAwait(false);
		return await sftpClient.ExistsAsync(SubstituteFileName(fileName), cancellationToken).ConfigureAwait(false);
	}

	/// <inheritdoc />
	public override DateTime? GetLastModifiedTimeUtc(string fileName)
	{
		try
		{
			var sftpClient = GetConnectedSftpClient();
			return sftpClient.GetLastWriteTimeUtc(SubstituteFileName(fileName));
		}
		catch (SftpPathNotFoundException sftpPathNotFoundException)
		{
			throw CreateFileNotFoundException(fileName, sftpPathNotFoundException);
		}
	}

	/// <inheritdoc />
	public override Task<DateTime?> GetLastModifiedTimeUtcAsync(string fileName, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		return Task.FromResult(this.GetLastModifiedTimeUtc(fileName)); // No async support
	}

	/// <inheritdoc />
	protected override string GetContentType(string fileName)
	{
		return null; // sFTP nepoužívá content types
	}

	/// <inheritdoc />
	protected override ValueTask<string> GetContentTypeAsync(string fileName, CancellationToken cancellationToken)
	{
		return new ValueTask<string>((string)null); // sFTP nepoužívá content types
	}

	/// <inheritdoc />
	protected override System.IO.Stream PerformOpenRead(string fileName)
	{
		try
		{
			var sftpClient = GetConnectedSftpClient();
			return sftpClient.OpenRead(SubstituteFileName(fileName));
		}
		catch (SftpPathNotFoundException sftpPathNotFoundException)
		{
			throw CreateFileNotFoundException(fileName, sftpPathNotFoundException);
		}
	}

	/// <inheritdoc />
	protected override async Task<System.IO.Stream> PerformOpenReadAsync(string fileName, CancellationToken cancellationToken = default)
	{
		try
		{
			var sftpClient = await GetConnectedSftpClientAsync(cancellationToken).ConfigureAwait(false);
			return await sftpClient.OpenAsync(SubstituteFileName(fileName), FileMode.Open, FileAccess.Read, cancellationToken).ConfigureAwait(false);
		}
		catch (SftpPathNotFoundException sftpPathNotFoundException)
		{
			throw CreateFileNotFoundException(fileName, sftpPathNotFoundException);
		}
	}

	/// <inheritdoc />
	protected override void PerformReadToStream(string fileName, System.IO.Stream stream)
	{
		try
		{
			var sftpClient = GetConnectedSftpClient();
			sftpClient.DownloadFile(SubstituteFileName(fileName), stream);
		}
		catch (SftpPathNotFoundException sftpPathNotFoundException)
		{
			throw CreateFileNotFoundException(fileName, sftpPathNotFoundException);
		}
	}

	/// <inheritdoc />
	protected override async Task PerformReadToStreamAsync(string fileName, System.IO.Stream stream, CancellationToken cancellationToken = default)
	{
		try
		{
			var sftpClient = await GetConnectedSftpClientAsync(cancellationToken).ConfigureAwait(false);
			await sftpClient.DownloadFileAsync(SubstituteFileName(fileName), stream, cancellationToken).ConfigureAwait(false);
		}
		catch (SftpPathNotFoundException sftpPathNotFoundException)
		{
			throw CreateFileNotFoundException(fileName, sftpPathNotFoundException);
		}
	}

	/// <inheritdoc />
	protected override void PerformSave(string fileName, System.IO.Stream fileContent, string contentType)
	{
		string substitutedFilename = SubstituteFileName(fileName);

		PerformSave_EnsureFolderFor(substitutedFilename);

		var sftpClient = GetConnectedSftpClient();
		sftpClient.UploadFile(fileContent, substitutedFilename, true);
	}

	/// <inheritdoc />
	protected override async Task PerformSaveAsync(string fileName, System.IO.Stream fileContent, string contentType, CancellationToken cancellationToken = default)
	{
		string substitutedFilename = SubstituteFileName(fileName);

		await PerformSave_EnsureFolderForAsync(substitutedFilename, cancellationToken).ConfigureAwait(false);

		var sftpClient = await GetConnectedSftpClientAsync(cancellationToken).ConfigureAwait(false);
		await sftpClient.UploadFileAsync(fileContent, substitutedFilename, cancellationToken).ConfigureAwait(false); // TODO: Nemá přetíženou verzi s overwrite?
	}

	private void PerformSave_EnsureFolderFor(string fileName)
	{
		string[] segments = fileName.Split('/').SkipLast(1).ToArray();  // a/b/c/d.txt -> a, b, c (d.txt odstraněno pomocí SkipLast)
		if (segments.Length > 0) // máme složky?
		{
			var client = GetConnectedSftpClient();
			for (int i = 0; i < segments.Length; i++)
			{
				string folderToCheck = String.Join("/", segments.Take(i + 1)); // postupně a, a/b, a/b/c
				if (!client.Exists(folderToCheck))
				{
					client.CreateDirectory(folderToCheck);
				}
			}
		}
	}

	private async Task PerformSave_EnsureFolderForAsync(string fileName, CancellationToken cancellationToken)
	{
		string[] segments = fileName.Split('/').SkipLast(1).ToArray();  // a/b/c/d.txt -> a, b, c (d.txt odstraněno pomocí SkipLast)
		if (segments.Length > 0) // máme složky?
		{
			var sftpClient = await GetConnectedSftpClientAsync(cancellationToken).ConfigureAwait(false);
			for (int i = 0; i < segments.Length; i++)
			{
				string folderToCheck = String.Join("/", segments.Take(i + 1)); // postupně a, a/b, a/b/c
				if (!await sftpClient.ExistsAsync(folderToCheck, cancellationToken).ConfigureAwait(false))
				{
					await sftpClient.CreateDirectoryAsync(folderToCheck, cancellationToken).ConfigureAwait(false);
				}
			}
		}
	}

	/// <inheritdoc />
	protected override System.IO.Stream PerformOpenCreate(string fileName, string contentType)
	{
		string substitutedFilename = SubstituteFileName(fileName);

		PerformSave_EnsureFolderFor(substitutedFilename);

		// ISftpClient má k dispozici metodu Create, která dle dokumentace dělá přesně to, co potřebujeme.
		// Nicméně při použití (minimálně vůči SFTP serveru nad Blob Storage, dostáváme chybu Renci.SshNet.Common.SshException: FeatureNotSupported: This feature is not supported.)
		// Ponecháme tedy méně hezké, zato funčkní řešení s Exists+Delete+OpenWrite.
		ISftpClient sftpClient = GetConnectedSftpClient();
		if (sftpClient.Exists(substitutedFilename))
		{
			sftpClient.Delete(substitutedFilename);
		}

		return sftpClient.OpenWrite(substitutedFilename);
	}

	/// <inheritdoc />
	protected override async Task<System.IO.Stream> PerformOpenCreateAsync(string fileName, string contentType, CancellationToken cancellationToken = default)
	{
		string substitutedFilename = SubstituteFileName(fileName);

		await PerformSave_EnsureFolderForAsync(substitutedFilename, cancellationToken).ConfigureAwait(false);

		// ISftpClient má k dispozici metodu Create, která dle dokumentace dělá přesně to, co potřebujeme.
		// Nicméně při použití (minimálně vůči SFTP serveru nad Blob Storage, dostáváme chybu Renci.SshNet.Common.SshException: FeatureNotSupported: This feature is not supported.)
		// Ponecháme tedy méně hezké, zato funčkní řešení s Exists+Delete+OpenWrite.
		ISftpClient sftpClient = await GetConnectedSftpClientAsync(cancellationToken).ConfigureAwait(false);
		if (await sftpClient.ExistsAsync(substitutedFilename, cancellationToken).ConfigureAwait(false))
		{
			await sftpClient.DeleteAsync(substitutedFilename, cancellationToken).ConfigureAwait(false);
		}

		return await sftpClient.OpenAsync(substitutedFilename, FileMode.Create, FileAccess.Write, cancellationToken).ConfigureAwait(false);
	}

	/// <inheritdoc />
	protected override void PerformCopy(string sourceFileName, IFileStorageService targetFileStorageService, string targetFileName)
	{
		if (this == targetFileStorageService)
		{
			PerformCopyInternalOnThis(sourceFileName, targetFileName);
		}
		else
		{
			base.PerformCopy(sourceFileName, targetFileStorageService, targetFileName);
		}
	}

	/// <inheritdoc />
	protected override async Task PerformCopyAsync(string sourceFileName, IFileStorageService targetFileStorageService, string targetFileName, CancellationToken cancellationToken)
	{
		if (this == targetFileStorageService)
		{
			await PerformCopyInternalOnThisAsync(sourceFileName, targetFileName, cancellationToken).ConfigureAwait(false);
		}
		else
		{
			await base.PerformCopyAsync(sourceFileName, targetFileStorageService, targetFileName, cancellationToken).ConfigureAwait(false);
		}
	}

	private void PerformCopyInternalOnThis(string sourceFileName, string targetFileName)
	{
		// Implementace je ochranou před:
		// The requested operation cannot be performed because there is a file transfer in progress.

		// download to temp tile
		string tempFilename = System.IO.Path.GetTempFileName();
		using (var tempStream = System.IO.File.OpenWrite(tempFilename))
		{
			GetConnectedSftpClient().DownloadFile(SubstituteFileName(sourceFileName), tempStream);
		}

		try
		{
			// upload from temp file
			string substitutedTartetFileName = SubstituteFileName(targetFileName);
			PerformSave_EnsureFolderFor(substitutedTartetFileName);
			using (var tempStream = System.IO.File.OpenRead(tempFilename))
			{
				GetConnectedSftpClient().UploadFile(tempStream, substitutedTartetFileName);
			}
		}
		finally
		{
			// clear temp file
			System.IO.File.Delete(tempFilename);
		}
	}

	private async Task PerformCopyInternalOnThisAsync(string sourceFileName, string targetFileName, CancellationToken cancellationToken)
	{
		// Implementace je ochranou před:
		// The requested operation cannot be performed because there is a file transfer in progress.

		// download to temp tile
		string tempFilename = System.IO.Path.GetTempFileName();
		using (var tempStream = System.IO.File.OpenWrite(tempFilename))
		{
			var sftpClient = await GetConnectedSftpClientAsync(cancellationToken).ConfigureAwait(false);
			await sftpClient.DownloadFileAsync(SubstituteFileName(sourceFileName), tempStream, cancellationToken).ConfigureAwait(false);
		}

		try
		{
			// upload from temp file
			string substitutedTartetFileName = SubstituteFileName(targetFileName);
			await PerformSave_EnsureFolderForAsync(substitutedTartetFileName, cancellationToken).ConfigureAwait(false);
			using (var tempStream = System.IO.File.OpenRead(tempFilename))
			{
				await sftpClient.UploadFileAsync(tempStream, substitutedTartetFileName, cancellationToken).ConfigureAwait(false);
			}
		}
		finally
		{
			// clear temp file
			System.IO.File.Delete(tempFilename);
		}
	}

	/// <inheritdoc />
	protected override void PerformMove(string sourceFileName, string targetFileName)
	{
		string substitutedTargetFileName = SubstituteFileName(targetFileName);
		PerformSave_EnsureFolderFor(substitutedTargetFileName);

		var sftpClient = GetConnectedSftpClient();

		if (sftpClient.Exists(substitutedTargetFileName))
		{
			sftpClient.Delete(substitutedTargetFileName);
		}

		sftpClient.RenameFile(SubstituteFileName(sourceFileName), substitutedTargetFileName);
	}

	/// <inheritdoc />
	protected override async Task PerformMoveAsync(string sourceFileName, string targetFileName, CancellationToken cancellationToken)
	{
		string substitutedTargetFileName = SubstituteFileName(targetFileName);
		await PerformSave_EnsureFolderForAsync(substitutedTargetFileName, cancellationToken).ConfigureAwait(false);

		var sftpClient = await GetConnectedSftpClientAsync(cancellationToken).ConfigureAwait(false);

		if (await sftpClient.ExistsAsync(substitutedTargetFileName, cancellationToken).ConfigureAwait(false))
		{
			await sftpClient.DeleteAsync(substitutedTargetFileName, cancellationToken).ConfigureAwait(false);
		}

		await sftpClient.RenameFileAsync(SubstituteFileName(sourceFileName), substitutedTargetFileName, cancellationToken).ConfigureAwait(false);
	}

	/// <inheritdoc />
	public void Dispose()
	{
		sftpClient?.Dispose();
		sftpClient = null;
	}

	private string SubstituteFileName(string sourceFileName)
	{
		return sourceFileName.Replace(@"\", "/");
	}

	private static Exception CreateFileNotFoundException(string fileName, Exception exception)
	{
		throw new FileNotFoundException($"Could not find file '{fileName}' on SFTP server.", fileName, exception);
	}
}
