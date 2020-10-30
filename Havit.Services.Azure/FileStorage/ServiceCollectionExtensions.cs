using Azure.Core;
using Havit.Services.FileStorage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services.Azure.FileStorage
{
	/// <summary>
	/// Extension metody k IServiceCollection.
	/// </summary>
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// Zaregistruje úložiště souborů ve Azure Blob Storage.
		/// </summary>
		public static void AddAzureBlobStorageService<TFileStorageContext>(this IServiceCollection services, string blobStorageConnectionString, string containerName)
			where TFileStorageContext : FileStorageContext
		{
			var options = new AzureBlobStorageServiceOptions<TFileStorageContext>
			{
				BlobStorageConnectionString = blobStorageConnectionString,
				ContainerName = containerName,
			};
		
			AddAzureBlobStorageService(services, options);
		}

		/// <summary>
		/// Zaregistruje úložiště souborů ve Azure Blob Storage.
		/// Určeno pro použití s managed identity.
		/// </summary>
		public static void AddAzureBlobStorageService<TFileStorageContext>(this IServiceCollection services, string blobStorageAccountName, string containerName, TokenCredential tokenCredential)
			where TFileStorageContext : FileStorageContext
		{
			var options = new AzureBlobStorageServiceOptions<TFileStorageContext>
			{
				BlobStorageAccountName = blobStorageAccountName,
				ContainerName = containerName,
				TokenCredential = tokenCredential
			};

			AddAzureBlobStorageService(services, options);
		}

		/// <summary>
		/// Zaregistruje úložiště souborů v Azure Blob Storage.
		/// </summary>
		public static void AddAzureBlobStorageService<TFileStorageContext>(this IServiceCollection services, AzureBlobStorageServiceOptions<TFileStorageContext> options)
			where TFileStorageContext : FileStorageContext
		{
			services.AddSingleton<IFileStorageService<TFileStorageContext>, AzureBlobStorageService<TFileStorageContext>>();
			services.AddSingleton<AzureBlobStorageServiceOptions<TFileStorageContext>>(options);
		}

		/// <summary>
		/// Zaregistruje úložiště souborů ve Azure Blob Storage.
		/// </summary>
		public static void AddAzureFileStorageService<TFileStorageContext>(this IServiceCollection services, string fileStorageConnectionString, string fileShareName, string rootDirectoryName = null)
			where TFileStorageContext : FileStorageContext
		{
			var options = new AzureFileStorageServiceOptions<TFileStorageContext>
			{
				FileStorageConnectionString = fileStorageConnectionString,
				FileShareName = fileShareName,
				RootDirectoryName = rootDirectoryName
			};

			AddAzureFileStorageService(services, options);
		}

		/// <summary>
		/// Zaregistruje úložiště souborů v Azure File Storage.
		/// </summary>
		public static void AddAzureFileStorageService<TFileStorageContext>(this IServiceCollection services, AzureFileStorageServiceOptions<TFileStorageContext> options)
			where TFileStorageContext : FileStorageContext
		{
			services.AddSingleton<IFileStorageService<TFileStorageContext>, AzureFileStorageService<TFileStorageContext>>();
			services.AddSingleton<AzureFileStorageServiceOptions<TFileStorageContext>>(options);
		}

	}
}
