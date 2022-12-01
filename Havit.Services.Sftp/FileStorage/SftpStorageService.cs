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
				sftpClient = new SftpClient(options.ConnectionInfo);
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
			GetConnectedSftpClient().Delete(fileName);
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
			return GetConnectedSftpClient().Exists(fileName);
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
			return GetConnectedSftpClient().GetLastWriteTimeUtc(fileName);
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
		protected override System.IO.Stream PerformRead(string fileName)
		{
			return GetConnectedSftpClient().OpenRead(fileName);
		}

		/// <inheritdoc />
		protected override Task<System.IO.Stream> PerformReadAsync(string fileName, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return Task.FromResult(this.PerformRead(fileName)); // No async support
		}

		/// <inheritdoc />
		protected override void PerformReadToStream(string fileName, System.IO.Stream stream)
		{
			using (System.IO.Stream sftpStream = PerformRead(fileName))
			{
				sftpStream.CopyTo(stream);
			}
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
			PerformSave_EnsureFolderFor(fileName);

			if (Exists(fileName))
			{
				Delete(fileName);
			}

			using (System.IO.Stream sftpStream = GetConnectedSftpClient().OpenWrite(fileName))
			{
				fileContent.CopyTo(sftpStream);
			}
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
		public void Dispose()
		{
			sftpClient?.Dispose();
			sftpClient = null;
		}
	}
}
