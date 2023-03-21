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
		public async Task CopyAsync(string sourceFileName, IFileStorageService targetFileStorageService, string targetFileName, CancellationToken cancellationToken = default)
		{
			await fileStorageService.CopyAsync(sourceFileName, targetFileStorageService, targetFileName, cancellationToken).ConfigureAwait(false);
		}

		/// <inheritdoc />
		public void Delete(string fileName)
		{
			fileStorageService.Delete(fileName);
		}

		/// <inheritdoc />
		public async Task DeleteAsync(string fileName, CancellationToken cancellationToken = default)
		{
			await fileStorageService.DeleteAsync(fileName, cancellationToken).ConfigureAwait(false);
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
		public async Task<bool> ExistsAsync(string fileName, CancellationToken cancellationToken = default)
		{
			return await fileStorageService.ExistsAsync(fileName, cancellationToken).ConfigureAwait(false);
		}

		/// <inheritdoc />
		public DateTime? GetLastModifiedTimeUtc(string fileName)
		{
			return fileStorageService.GetLastModifiedTimeUtc(fileName);
		}

		/// <inheritdoc />
		public async Task<DateTime?> GetLastModifiedTimeUtcAsync(string fileName, CancellationToken cancellationToken = default)
		{
			return await fileStorageService.GetLastModifiedTimeUtcAsync(fileName, cancellationToken).ConfigureAwait(false);
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
		public async Task MoveAsync(string sourceFileName, IFileStorageService targetFileStorageService, string targetFileName, CancellationToken cancellationToken = default)
		{
			await fileStorageService.MoveAsync(sourceFileName, targetFileStorageService, targetFileName, cancellationToken).ConfigureAwait(false);
		}

		/// <inheritdoc />
		[Obsolete]
		public Stream Read(string fileName)
		{
			return fileStorageService.Read(fileName);
		}

		/// <inheritdoc />
		[Obsolete]
		public async Task<Stream> ReadAsync(string fileName, CancellationToken cancellationToken = default)
		{
			return await fileStorageService.ReadAsync(fileName, cancellationToken).ConfigureAwait(false);
		}

		public Stream OpenRead(string fileName)
		{
			return fileStorageService.OpenRead(fileName);
		}

		public async Task<Stream> OpenReadAsync(string fileName, CancellationToken cancellationToken = default)
		{
			return await fileStorageService.OpenReadAsync(fileName, cancellationToken).ConfigureAwait(false);
		}

		/// <inheritdoc />
		public void ReadToStream(string fileName, Stream stream)
		{
			fileStorageService.ReadToStream(fileName, stream);
		}

		/// <inheritdoc />
		public async Task ReadToStreamAsync(string fileName, Stream stream, CancellationToken cancellationToken = default)
		{
			await fileStorageService.ReadToStreamAsync(fileName, stream, cancellationToken).ConfigureAwait(false);
		}

		/// <inheritdoc />
		public void Save(string fileName, Stream fileContent, string contentType)
		{
			fileStorageService.Save(fileName, fileContent, contentType);
		}

		/// <inheritdoc />
		public async Task SaveAsync(string fileName, Stream fileContent, string contentType, CancellationToken cancellationToken = default)
		{
			await fileStorageService.SaveAsync(fileName, fileContent, contentType, cancellationToken).ConfigureAwait(false);
		}

		public Stream OpenCreate(string fileName, string contentType)
		{
			return fileStorageService.OpenCreate(fileName, contentType);
		}

		public async Task<Stream> OpenCreateAsync(string fileName, string contentType, CancellationToken cancellationToken = default)
		{
			return await fileStorageService.OpenCreateAsync(fileName, contentType, cancellationToken).ConfigureAwait(false);
		}
	}
}
