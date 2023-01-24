using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Havit.Services.FileStorage
{
	/// <summary>
	/// Třída jen wrapuje jinou službu file storage.
	/// Cílem této třídy je poskytnout pohodlný způsob implementace (např.) IApplicationFileStorage do aplikací.
	/// </summary>
	public abstract class FileStorageWrappingService<TUnderlyingFileStorageContext> : IFileStorageService<TUnderlyingFileStorageContext>, IFileStorageWrappingService
		where TUnderlyingFileStorageContext : FileStorageContext
	{
		private readonly IFileStorageService<TUnderlyingFileStorageContext> fileStorageService;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		protected FileStorageWrappingService(IFileStorageService<TUnderlyingFileStorageContext> fileStorageService)
		{
			this.fileStorageService = fileStorageService;
		}

		/// <inheritdoc />
		public void Copy(string sourceFileName, IFileStorageService targetFileStorageService, string targetFileName)
		{
			fileStorageService.Copy(sourceFileName, targetFileStorageService, targetFileName);
		}

		/// <inheritdoc />
		public Task CopyAsync(string sourceFileName, IFileStorageService targetFileStorageService, string targetFileName, CancellationToken cancellationToken = default)
		{
			return fileStorageService.CopyAsync(sourceFileName, targetFileStorageService, targetFileName, cancellationToken);
		}

		/// <inheritdoc />
		public void Delete(string fileName)
		{
			fileStorageService.Delete(fileName);
		}

		/// <inheritdoc />
		public Task DeleteAsync(string fileName, CancellationToken cancellationToken = default)
		{
			return fileStorageService.DeleteAsync(fileName, cancellationToken);
		}

		/// <inheritdoc />
		public IEnumerable<FileInfo> EnumerateFiles(string pattern = null)
		{
			return fileStorageService.EnumerateFiles(pattern);
		}

		/// <inheritdoc />
		public IAsyncEnumerable<FileInfo> EnumerateFilesAsync(string pattern = null, CancellationToken cancellationToken = default)
		{
			return fileStorageService.EnumerateFilesAsync(pattern, cancellationToken);
		}

		/// <inheritdoc />
		public bool Exists(string fileName)
		{
			return fileStorageService.Exists(fileName);
		}

		/// <inheritdoc />
		public Task<bool> ExistsAsync(string fileName, CancellationToken cancellationToken = default)
		{
			return fileStorageService.ExistsAsync(fileName, cancellationToken);
		}

		/// <inheritdoc />
		public DateTime? GetLastModifiedTimeUtc(string fileName)
		{
			return fileStorageService.GetLastModifiedTimeUtc(fileName);
		}

		/// <inheritdoc />
		public Task<DateTime?> GetLastModifiedTimeUtcAsync(string fileName, CancellationToken cancellationToken = default)
		{
			return fileStorageService.GetLastModifiedTimeUtcAsync(fileName, cancellationToken);
		}

		/// <inheritdoc />
		public IFileStorageService GetWrappedFileStorageService()
		{
			return fileStorageService;
		}

		public void Move(string sourceFileName, string targetFileName)
		{
			fileStorageService.Move(sourceFileName, targetFileName);
		}

		public async Task MoveAsync(string sourceFileName, string targetFileName, CancellationToken cancellationToken = default)
		{
			await fileStorageService.MoveAsync(sourceFileName, targetFileName, cancellationToken).ConfigureAwait(false);
		}

		/// <inheritdoc />
		public void Move(string sourceFileName, IFileStorageService targetFileStorageService, string targetFileName)
		{
			fileStorageService.Move(sourceFileName, targetFileStorageService, targetFileName);
		}

		/// <inheritdoc />
		public Task MoveAsync(string sourceFileName, IFileStorageService targetFileStorageService, string targetFileName, CancellationToken cancellationToken = default)
		{
			return fileStorageService.MoveAsync(sourceFileName, targetFileStorageService, targetFileName, cancellationToken);
		}

		/// <inheritdoc />
		public Stream Read(string fileName)
		{
			return fileStorageService.Read(fileName);
		}

		/// <inheritdoc />
		public Task<Stream> ReadAsync(string fileName, CancellationToken cancellationToken = default)
		{
			return fileStorageService.ReadAsync(fileName, cancellationToken);
		}

		/// <inheritdoc />
		public void ReadToStream(string fileName, Stream stream)
		{
			fileStorageService.ReadToStream(fileName, stream);
		}

		/// <inheritdoc />
		public Task ReadToStreamAsync(string fileName, Stream stream, CancellationToken cancellationToken = default)
		{
			return fileStorageService.ReadToStreamAsync(fileName, stream, cancellationToken);
		}

		/// <inheritdoc />
		public void Save(string fileName, Stream fileContent, string contentType)
		{
			fileStorageService.Save(fileName, fileContent, contentType);
		}

		/// <inheritdoc />
		public Task SaveAsync(string fileName, Stream fileContent, string contentType, CancellationToken cancellationToken = default)
		{
			return fileStorageService.SaveAsync(fileName, fileContent, contentType, cancellationToken);
		}
	}
}
