using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services.FileStorage
{
	public static class ServiceCollectionExtensions
	{
		public static void AddFileSystemStorageService<TFileStorageContext>(this IServiceCollection services, string storagePath)
			where TFileStorageContext : FileStorageContext
		{
			services.AddSingleton<IFileStorageService<TFileStorageContext>, FileSystemStorageService<TFileStorageContext>>();
			services.AddSingleton<FileSystemStorageServiceOptions<TFileStorageContext>>(new FileSystemStorageServiceOptions<TFileStorageContext> { StoragePath = storagePath });
		}

		public static void AddFileStorageCachingProxy<TFileStorageContext, TUnderlyingFileStorageContextContext>(this IServiceCollection services)
			where TFileStorageContext : FileStorageContext
			where TUnderlyingFileStorageContextContext : FileStorageContext
		{
			services.AddSingleton<IFileStorageService<TFileStorageContext>, FileStorageServiceCachingProxy<TFileStorageContext, TUnderlyingFileStorageContextContext>>();
		}
	}
}
