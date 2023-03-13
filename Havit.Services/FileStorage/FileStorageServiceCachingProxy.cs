using Havit.Services.Caching;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Havit.Services.FileStorage
{
	/// <summary>
	/// Služba (proxy), která slouží jako cachující proxy IFileStorageService.
	/// Tedy na vstupu (závislost) má IFileStorageService a sama poskytuje IFileStorageService, přičemž implementaci deleguji na svoji závilost a snaží se získané cachovat.
	/// </summary>
	/// <remarks>
	/// Cachovány jsou hodnoty vracené metodami:
	/// * Exists,
	/// * GetLastModifiedTimeUtc,
	/// * Read (a v důsledku též ReadToStream).
	/// 
	/// Cachovány naopak nejsou výsledky vracené metodou
	/// * EnumerateFiles.
	/// 
	/// Klíč do se skládá:
	/// * z názvu typu použité závislosti fileStorageService (v praktickém použití máme, pokud potřebujeme více různých úložišť v aplikaci, více různých tříd (a interfaceů)), pokud se má chovat jinak, lze předefinovat metodu GetCacheKeyPrefix().
	/// * z operace, pro kterou jsou data cachována (Exists / GetLastModifiedTimeUtc / Read)
	/// * a z názvu souboru malými písmeny (nepředpokládá se práce se soubory, jejich název se liší jen "velikostí" písmen - lowercase vs. uppercase.
	/// 
	/// Invalidace
	/// Při uložení nebo smazání souboru jsou hodnoty pro daný soubor invalidovány při uložení souboru a při smazání souboru.
	/// 
	/// CacheOptions
	/// Pro určení CacheOptions (například pro expiraci cache) lze předefinovat metodu GetCacheOptions. Metoda může vracet null (výchozí implementace) a musí pokaždé vrátit novou hodnotu (nelze reusovat jednu instanci).
	/// </remarks>
	public class FileStorageServiceCachingProxy : IFileStorageService
	{
		private readonly IFileStorageService fileStorageService;
		private readonly ICacheService cacheService;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public FileStorageServiceCachingProxy(IFileStorageService fileStorageService, ICacheService cacheService)
		{
			this.fileStorageService = fileStorageService;
			this.cacheService = cacheService;
		}

		/// <inheritdoc />
		public IEnumerable<Havit.Services.FileStorage.FileInfo> EnumerateFiles(string pattern = null)
		{
			return fileStorageService.EnumerateFiles(pattern);
		}

		/// <inheritdoc />
		public async IAsyncEnumerable<FileInfo> EnumerateFilesAsync(string pattern = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			await foreach (FileInfo fileInfo in fileStorageService.EnumerateFilesAsync(pattern, cancellationToken).ConfigureAwait(false))
			{
				yield return fileInfo;
			}
		}

		/// <inheritdoc />
		public bool Exists(string fileName)
		{
			string cacheKey = GetCacheKey(CachedStorageOperation.Exists, fileName);

			if (cacheService.TryGet(cacheKey, out object cacheValue))
			{
				return (bool)cacheValue;
			}

			bool result = fileStorageService.Exists(fileName);
			cacheService.Add(cacheKey, result, GetCacheOptions(CachedStorageOperation.Exists, fileName));

			return result;
		}

		/// <inheritdoc />
		public async Task<bool> ExistsAsync(string fileName, CancellationToken cancellationToken = default)
		{
			string cacheKey = GetCacheKey(CachedStorageOperation.Exists, fileName);

			if (cacheService.TryGet(cacheKey, out object cacheValue))
			{
				return (bool)cacheValue;
			}

			bool result = await fileStorageService.ExistsAsync(fileName, cancellationToken).ConfigureAwait(false);
			cacheService.Add(cacheKey, result, GetCacheOptions(CachedStorageOperation.Exists, fileName));

			return result;
		}

		/// <inheritdoc />
		public DateTime? GetLastModifiedTimeUtc(string fileName)
		{
			string cacheKey = GetCacheKey(CachedStorageOperation.GetLastModifiedTimeUtc, fileName);

			if (cacheService.TryGet(cacheKey, out object cacheValue))
			{
				return (DateTime?)cacheValue;
			}

			DateTime? result = fileStorageService.GetLastModifiedTimeUtc(fileName);
			cacheService.Add(cacheKey, result, GetCacheOptions(CachedStorageOperation.GetLastModifiedTimeUtc, fileName));

			return result;
		}

		/// <inheritdoc />
		public async Task<DateTime?> GetLastModifiedTimeUtcAsync(string fileName, CancellationToken cancellationToken = default)
		{
			string cacheKey = GetCacheKey(CachedStorageOperation.GetLastModifiedTimeUtc, fileName);

			if (cacheService.TryGet(cacheKey, out object cacheValue))
			{
				return (DateTime?)cacheValue;
			}

			DateTime? result = await fileStorageService.GetLastModifiedTimeUtcAsync(fileName, cancellationToken).ConfigureAwait(false);
			cacheService.Add(cacheKey, result, GetCacheOptions(CachedStorageOperation.GetLastModifiedTimeUtc, fileName));

			return result;
		}

		/// <inheritdoc />
		[Obsolete]
		public Stream Read(string fileName)
		{
			return OpenRead(fileName);
		}

		/// <inheritdoc />
		[Obsolete]
		public async Task<Stream> ReadAsync(string fileName, CancellationToken cancellationToken = default)
		{
			return await OpenReadAsync(fileName, cancellationToken).ConfigureAwait(false);
		}

		/// <inheritdoc />
		public Stream OpenRead(string fileName)
		{
			string cacheKey = GetCacheKey(CachedStorageOperation.Read, fileName);

			if (cacheService.TryGet(cacheKey, out object cacheValue))
			{
				return new MemoryStream((byte[])cacheValue);
			}

			byte[] bytes;
			using (Stream dataStream = fileStorageService.Read(fileName))
			{
				if (dataStream is MemoryStream ms1)
				{
					bytes = ms1.ToArray();
				}
				else
				{
					using (MemoryStream ms2 = new MemoryStream())
					{
						dataStream.CopyTo(ms2);
						bytes = ms2.ToArray();
					}
				}
			}
			var cacheOptions = GetCacheOptions(CachedStorageOperation.Read, fileName) ?? new CacheOptions();
			cacheOptions.Size = bytes.Length;

			cacheService.Add(cacheKey, bytes, cacheOptions);
			return new MemoryStream(bytes, false);
		}

		/// <inheritdoc />
		public async Task<Stream> OpenReadAsync(string fileName, CancellationToken cancellationToken = default)
		{
			string cacheKey = GetCacheKey(CachedStorageOperation.Read, fileName);

			if (cacheService.TryGet(cacheKey, out object cacheValue))
			{
				return new MemoryStream((byte[])cacheValue);
			}

			byte[] bytes;
			using (Stream dataStream = fileStorageService.Read(fileName))
			{
				if (dataStream is MemoryStream ms1)
				{
					bytes = ms1.ToArray();
				}
				else
				{
					using (MemoryStream ms2 = new MemoryStream())
					{
						await dataStream.CopyToAsync(ms2, 81920 /* default */, cancellationToken).ConfigureAwait(false);
						bytes = ms2.ToArray();
					}
				}
			}
			var cacheOptions = GetCacheOptions(CachedStorageOperation.Read, fileName) ?? new CacheOptions();
			cacheOptions.Size = bytes.Length;

			cacheService.Add(cacheKey, bytes, cacheOptions);
			return new MemoryStream(bytes);
		}

		/// <inheritdoc />
		public void ReadToStream(string fileName, Stream stream)
		{
			using (MemoryStream dataStream = (MemoryStream)OpenRead(fileName))
			{
				stream.CopyTo(stream);
			}
		}

		/// <inheritdoc />
		public async Task ReadToStreamAsync(string fileName, Stream stream, CancellationToken cancellationToken = default)
		{
			using (MemoryStream dataStream = (MemoryStream)await OpenReadAsync(fileName, cancellationToken).ConfigureAwait(false))
			{
				await stream.CopyToAsync(stream, 81920 /* default */, cancellationToken).ConfigureAwait(false);
			}
		}

		/// <inheritdoc />
		public void Save(string fileName, Stream fileContent, string contentType)
		{
			try
			{
				fileStorageService.Save(fileName, fileContent, contentType);
			}
			finally
			{
				InvalidateCacheByFileName(fileName);
			}
		}

		/// <inheritdoc />
		public async Task SaveAsync(string fileName, Stream fileContent, string contentType, CancellationToken cancellationToken = default)
		{
			try
			{
				await fileStorageService.SaveAsync(fileName, fileContent, contentType, cancellationToken).ConfigureAwait(false);
			}
			finally
			{
				InvalidateCacheByFileName(fileName);
			}
		}

		/// <inheritdoc />
		public Stream OpenWrite(string fileName, string contentType)
		{
			try
			{
				return fileStorageService.OpenWrite(fileName, contentType);
			}
			finally
			{
				// TODO: Invalidovat až po uzavření streamu
				InvalidateCacheByFileName(fileName);
			}
		}

		/// <inheritdoc />
		public async Task<Stream> OpenWriteAsync(string fileName, string contentType, CancellationToken cancellationToken = default)
		{
			try
			{
				return await fileStorageService.OpenWriteAsync(fileName, contentType, cancellationToken).ConfigureAwait(false);
			}
			finally
			{
				// TODO: Invalidovat až po uzavření streamu
				InvalidateCacheByFileName(fileName);
			}
		}

		/// <inheritdoc />
		public void Delete(string fileName)
		{
			try
			{
				fileStorageService.Delete(fileName);
			}
			finally
			{
				InvalidateCacheByFileName(fileName);
			}
		}

		/// <inheritdoc />
		public async Task DeleteAsync(string fileName, CancellationToken cancellationToken = default)
		{
			try
			{
				await fileStorageService.DeleteAsync(fileName, cancellationToken).ConfigureAwait(false);
			}
			finally
			{
				InvalidateCacheByFileName(fileName);
			}
		}

		/// <summary>
		/// Vrací prefix klíče do cache.
		/// </summary>
		protected virtual string GetCacheKeyPrefix()
		{
			return fileStorageService.GetType().FullName;
		}

		/// <summary>
		/// Vrací klíč do cache.
		/// </summary>
		protected string GetCacheKey(CachedStorageOperation operation, string fileName)
		{
			return GetCacheKeyPrefix() + "|" + operation.ToString() + "|" + fileName.ToLower();
		}

		/// <summary>
		/// Vrací cache options.
		/// </summary>
		protected virtual CacheOptions GetCacheOptions(CachedStorageOperation operation, string fileName)
		{
			return null;
		}

		/// <summary>
		/// Invaliduje hodnoty pro daný soubor z cache.
		/// </summary>
		private void InvalidateCacheByFileName(string fileName)
		{
			cacheService.Remove(GetCacheKey(CachedStorageOperation.GetLastModifiedTimeUtc, fileName));
			cacheService.Remove(GetCacheKey(CachedStorageOperation.Exists, fileName));
			cacheService.Remove(GetCacheKey(CachedStorageOperation.Read, fileName));
		}

		/// <inheritdoc />
		public void Copy(string sourceFileName, IFileStorageService targetFileStorageService, string targetFileName)
		{
			try
			{
				fileStorageService.Copy(sourceFileName, targetFileStorageService, targetFileName);
			}
			finally
			{
				InvalidateCacheByFileName(targetFileName);
			}
		}

		/// <inheritdoc />
		public async Task CopyAsync(string sourceFileName, IFileStorageService targetFileStorageService, string targetFileName, CancellationToken cancellationToken = default)
		{
			try
			{
				await fileStorageService.CopyAsync(sourceFileName, targetFileStorageService, targetFileName, cancellationToken).ConfigureAwait(false);
			}
			finally
			{
				InvalidateCacheByFileName(targetFileName);
			}

		}

		/// <inheritdoc />
		public void Move(string sourceFileName, string targetFileName)
		{
			try
			{
				fileStorageService.Move(sourceFileName, targetFileName);
			}
			finally
			{
				InvalidateCacheByFileName(sourceFileName);
				InvalidateCacheByFileName(targetFileName);
			}
		}

		/// <inheritdoc />
		public async Task MoveAsync(string sourceFileName, string targetFileName, CancellationToken cancellationToken = default)
		{
			try
			{
				await fileStorageService.MoveAsync(sourceFileName, targetFileName, cancellationToken).ConfigureAwait(false);
			}
			finally
			{
				InvalidateCacheByFileName(sourceFileName);
				InvalidateCacheByFileName(targetFileName);
			}
		}

		/// <inheritdoc />
		public void Move(string sourceFileName, IFileStorageService targetFileStorageService, string targetFileName)
		{
			try
			{
				fileStorageService.Move(sourceFileName, targetFileStorageService, targetFileName);
			}
			finally
			{
				InvalidateCacheByFileName(sourceFileName);
			}
		}

		/// <inheritdoc />
		public async Task MoveAsync(string sourceFileName, IFileStorageService targetFileStorageService, string targetFileName, CancellationToken cancellationToken = default)
		{
			try
			{
				await fileStorageService.MoveAsync(sourceFileName, targetFileStorageService, targetFileName, cancellationToken).ConfigureAwait(false);
			}
			finally
			{
				InvalidateCacheByFileName(sourceFileName);
			}
		}

		/// <summary>
		/// Seznam operací pro účely sestavení klíče do cache.
		/// </summary>
		protected enum CachedStorageOperation
		{
			/// <summary>
			/// Metoda Exists.
			/// </summary>
			Exists,

			/// <summary>
			/// Metoda Read.
			/// </summary>
			Read,

			/// <summary>
			/// Metoda GetLastModifiedTimeUtc.
			/// </summary>
			GetLastModifiedTimeUtc
		}
	}

}
