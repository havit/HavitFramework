using Havit.Services.Caching;

namespace Havit.Services.FileStorage;

public class FileStorageServiceCachingProxy<TFileStorageContext, TUnderlyingFileStorageContext> : FileStorageServiceCachingProxy, IFileStorageService<TFileStorageContext>
	where TFileStorageContext : FileStorageContext
	where TUnderlyingFileStorageContext : FileStorageContext
{
	/// <summary>
	/// Konstruktor.
	/// </summary>
	public FileStorageServiceCachingProxy(IFileStorageService<TUnderlyingFileStorageContext> fileStorageService, ICacheService cacheService) : base(fileStorageService, cacheService)
	{
		// NOOP
	}
}
