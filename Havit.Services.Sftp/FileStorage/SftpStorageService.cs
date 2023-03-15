using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Havit.Linq;
using Havit.Services.FileStorage;
using Havit.Text.RegularExpressions;
using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;

namespace Havit.Services.Sftp.FileStorage
{
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
			GetConnectedSftpClient().Delete(SubstituteFileName(fileName));
		}

		/// <inheritdoc />
		public override Task DeleteAsync(string fileName, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			this.Delete(fileName); // No async support
			return Task.CompletedTask;
		}

		/// <inheritdoc />
		public override IEnumerable<FileInfo> EnumerateFiles(string searchPattern = null)
		{
			// zamen souborova '\\' za '/', ktere lze pouzit v Azure blobu
			searchPattern = searchPattern?.Replace("\\", "/");

			// ziskej prefix, uvodni cast cesty, ve kterem nejsou pouzite znaky '*' a '?'
			string prefix = EnumerableFilesGetPrefix(searchPattern);

			foreach (FileInfo fileInfo in EnumerateFiles_ListFilesInHierarchyInternal("", prefix))
			{
				if (EnumerateFiles_FilterFileInfo(fileInfo, searchPattern))
				{
					yield return fileInfo;
				}
			}
		}

		private IEnumerable<FileInfo> EnumerateFiles_ListFilesInHierarchyInternal(string directoryPrefix, string searchPrefix)
		{
			// speed up
			if (!String.IsNullOrEmpty(searchPrefix) && !(directoryPrefix.StartsWith(searchPrefix) || searchPrefix.StartsWith(directoryPrefix)))
			{
				yield break;
			}

			IEnumerable<SftpFile> sftpFiles;

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
					yield return new FileInfo
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

		//private string EnumerateFiles_GetFilename(string fullname)
		//{
		//	return fullname.StartsWith(sftpClient.WorkingDirectory) // ie. "/primary"
		//		? fullname.Substring(sftpClient.WorkingDirectory.Length + 1) // vč. lomítka ZA /primary
		//		: fullname;
		//}

		private bool EnumerateFiles_FilterFileInfo(FileInfo fileInfo, string searchPattern)
		{
			if ((searchPattern != null) && !RegexPatterns.IsFileWildcardMatch(fileInfo.Name, searchPattern))
			{
				return false;
			}

			return true;
		}


		/// <inheritdoc />
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
		public override async IAsyncEnumerable<Services.FileStorage.FileInfo> EnumerateFilesAsync(string pattern = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
		{
			cancellationToken.ThrowIfCancellationRequested();

			foreach (var item in EnumerateFiles(pattern)) // No async support
			{
				cancellationToken.ThrowIfCancellationRequested();
				yield return item;
			}
		}

		/// <inheritdoc />
		public override bool Exists(string fileName)
		{
			return GetConnectedSftpClient().Exists(SubstituteFileName(fileName));
		}

		/// <inheritdoc />
		public override Task<bool> ExistsAsync(string fileName, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return Task.FromResult(this.Exists(fileName)); // No async support
		}

		/// <inheritdoc />
		public override DateTime? GetLastModifiedTimeUtc(string fileName)
		{
			return GetConnectedSftpClient().GetLastWriteTimeUtc(SubstituteFileName(fileName));
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
			return GetConnectedSftpClient().OpenRead(SubstituteFileName(fileName));
		}

		/// <inheritdoc />
		protected override Task<System.IO.Stream> PerformOpenReadAsync(string fileName, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return Task.FromResult(this.PerformOpenRead(fileName)); // No async support
		}

		/// <inheritdoc />
		protected override void PerformReadToStream(string fileName, System.IO.Stream stream)
		{
			GetConnectedSftpClient().DownloadFile(SubstituteFileName(fileName), stream);
		}

		/// <inheritdoc />
		protected override Task PerformReadToStreamAsync(string fileName, System.IO.Stream stream, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			this.PerformReadToStream(fileName, stream); // No async support
			return Task.CompletedTask;
		}

		/// <inheritdoc />
		protected override void PerformSave(string fileName, System.IO.Stream fileContent, string contentType)
		{
			string substitutedFilename = SubstituteFileName(fileName);

			PerformSave_EnsureFolderFor(substitutedFilename);
			GetConnectedSftpClient().UploadFile(fileContent, substitutedFilename, true);
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

		/// <inheritdoc />
		protected override Task PerformSaveAsync(string fileName, System.IO.Stream fileContent, string contentType, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			this.PerformSave(fileName, fileContent, contentType); // No async support
			return Task.CompletedTask;
		}

		/// <inheritdoc />
		protected override System.IO.Stream PerformOpenWrite(string fileName, string contentType)
		{
			string substitutedFilename = SubstituteFileName(fileName);

			PerformSave_EnsureFolderFor(substitutedFilename);

			var sftpClient = GetConnectedSftpClient();
			if (sftpClient.Exists(substitutedFilename))
			{
				sftpClient.Delete(substitutedFilename);
			}

			return sftpClient.OpenWrite(substitutedFilename);
		}

		/// <inheritdoc />
		protected override Task<System.IO.Stream> PerformOpenWriteAsync(string fileName, string contentType, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return Task.FromResult(PerformOpenWrite(fileName, contentType)); // no async support
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
				PerformCopyInternalOnThis(sourceFileName, targetFileName); // No async support
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

			// upload from temp file
			string substitutedTartetFileName = SubstituteFileName(targetFileName);
			PerformSave_EnsureFolderFor(substitutedTartetFileName);
			using (var tempStream = System.IO.File.OpenRead(tempFilename))
			{
				GetConnectedSftpClient().UploadFile(tempStream, substitutedTartetFileName);
			}

			// clear temp file
			System.IO.File.Delete(tempFilename);
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

			GetConnectedSftpClient().RenameFile(SubstituteFileName(sourceFileName), substitutedTargetFileName);
		}

		/// <inheritdoc />
		protected override Task PerformMoveAsync(string sourceFileName, string targetFileName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			PerformMove(sourceFileName, targetFileName);
			return Task.CompletedTask;
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
	}
}
