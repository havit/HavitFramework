using Havit.Services.Caching;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
		public async IAsyncEnumerable<FileInfo> EnumerateFilesAsync(string pattern = null)
		{
			await foreach (FileInfo fileInfo in EnumerateFilesAsync(pattern))
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
		public async Task<bool> ExistsAsync(string fileName)
		{
			string cacheKey = GetCacheKey(CachedStorageOperation.Exists, fileName);

			if (cacheService.TryGet(cacheKey, out object cacheValue))
			{
				return (bool)cacheValue;
			}

			bool result = await fileStorageService.ExistsAsync(fileName).ConfigureAwait(false);
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
		public async Task<DateTime?> GetLastModifiedTimeUtcAsync(string fileName)
		{
			string cacheKey = GetCacheKey(CachedStorageOperation.GetLastModifiedTimeUtc, fileName);

			if (cacheService.TryGet(cacheKey, out object cacheValue))
			{
				return (DateTime?)cacheValue;
			}

			DateTime? result = await fileStorageService.GetLastModifiedTimeUtcAsync(fileName).ConfigureAwait(false);
			cacheService.Add(cacheKey, result, GetCacheOptions(CachedStorageOperation.GetLastModifiedTimeUtc, fileName));

			return result;
		}

		/// <inheritdoc />
		public Stream Read(string fileName)
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
			return new MemoryStream(bytes);
		}

		/// <inheritdoc />
		public async Task<Stream> ReadAsync(string fileName)
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
						await dataStream.CopyToAsync(ms2).ConfigureAwait(false);
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
			using (MemoryStream dataStream = (MemoryStream)Read(fileName))
			{
				stream.CopyTo(stream);
			}
		}

		/// <inheritdoc />
		public async Task ReadToStreamAsync(string fileName, Stream stream)
		{
			using (MemoryStream dataStream = (MemoryStream)await ReadAsync(fileName).ConfigureAwait(false))
			{
				await stream.CopyToAsync(stream).ConfigureAwait(false);
			}
		}

		/// <inheritdoc />
		public void Save(string fileName, Stream fileContent, string contentType)
		{
			fileStorageService.Save(fileName, fileContent, contentType);
			InvalidateCacheByFileName(fileName);
		}

		/// <inheritdoc />
		public async Task SaveAsync(string fileName, Stream fileContent, string contentType)
		{
			await fileStorageService.SaveAsync(fileName, fileContent, contentType).ConfigureAwait(false);
			InvalidateCacheByFileName(fileName);
		}

		/// <inheritdoc />
		public void Delete(string fileName)
		{
			fileStorageService.Delete(fileName);
			InvalidateCacheByFileName(fileName);
		}

		/// <inheritdoc />
		public async Task DeleteAsync(string fileName)
		{
			await fileStorageService.DeleteAsync(fileName).ConfigureAwait(false);
			InvalidateCacheByFileName(fileName);
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
