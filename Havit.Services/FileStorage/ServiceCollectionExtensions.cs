using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services.FileStorage
{
	/// <summary>
	/// Extension metody k IServiceCollection.
	/// </summary>
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// Zaregistruje úložiště souborů ve filesystému.
		/// </summary>
		public static void AddFileSystemStorageService<TFileStorageContext>(this IServiceCollection services, string storagePath)
			where TFileStorageContext : FileStorageContext
		{
			services.AddSingleton<IFileStorageService<TFileStorageContext>, FileSystemStorageService<TFileStorageContext>>();
			services.AddSingleton<FileSystemStorageServiceOptions<TFileStorageContext>>(new FileSystemStorageServiceOptions<TFileStorageContext> { StoragePath = storagePath });
		}

		/// <summary>
		/// Zaregistruje cachující wrapper úložiště souborů.
		/// </summary>
		/// <typeparam name="TFileStorageContext">Typ, pod kterým je k dispozici úložiště pro aplikaci (resp. IFileStorage&lt;TFileStorageContext&gt;).</typeparam>
		/// <typeparam name="TUnderlyingFileStorageContextContext">Typ úložiště, které je obaleno cachováním</typeparam>
		public static void AddFileStorageCachingProxy<TFileStorageContext, TUnderlyingFileStorageContextContext>(this IServiceCollection services)
			where TFileStorageContext : FileStorageContext
			where TUnderlyingFileStorageContextContext : FileStorageContext
		{
			services.AddSingleton<IFileStorageService<TFileStorageContext>, FileStorageServiceCachingProxy<TFileStorageContext, TUnderlyingFileStorageContextContext>>();
		}

		/// <summary>
		/// Zaregistruje file storage wrapper, určeno pro IXyFileStorageService, pokud takový interface chceme v aplikaci.
		/// </summary>
		public static void AddFileStorageWrappingService<TService, TImplementation, TFileStorageContext>(this IServiceCollection services)
			where TService : class, IFileStorageService
			where TImplementation : FileStorageWrappingService<TFileStorageContext>, TService
			where TFileStorageContext : FileStorageContext
		{
			services.AddSingleton<TService, TImplementation>();
		}

		/// <summary>
		/// Zaregistruje úložiště souborů z embdedded resources.
		/// </summary>
		public static void AddEmbeddedResourcesStorageService<TFileStorageContext>(this IServiceCollection services, Assembly resourceAssembly, string rootNamespace)
			where TFileStorageContext : FileStorageContext
		{
			services.AddSingleton<IFileStorageService<TFileStorageContext>, EmbeddedResourceStorageService<TFileStorageContext>>();
			services.AddSingleton<EmbeddedResourceStorageOptions<TFileStorageContext>>(new EmbeddedResourceStorageOptions<TFileStorageContext>
			{
				ResourceAssembly = resourceAssembly,
				RootNamespace = rootNamespace
			});
		}
	}
}
