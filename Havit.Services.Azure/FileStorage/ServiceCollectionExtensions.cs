using Azure.Core;
using Havit.Diagnostics.Contracts;
using Havit.Services.FileStorage;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Services.Azure.FileStorage;

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
		Contract.Requires(blobStorageConnectionString.Contains(';'), $"{nameof(blobStorageConnectionString)} must contain a semicolon (;).");

		var options = new AzureBlobStorageServiceOptions<TFileStorageContext>
		{
			BlobStorage = blobStorageConnectionString,
			ContainerName = containerName,
		};

		AddAzureBlobStorageService(services, options);
	}

	/// <summary>
	/// Zaregistruje úložiště souborů ve Azure Blob Storage.
	/// Určeno pro použití s managed identity.
	/// </summary>
	/// <param name="services">Services where to register.</param>
	/// <param name="blobStorage">Blob storage connection string (contains ;) or blob storage account name (does not contain ;).</param>
	/// <param name="containerName">Container name.</param>
	/// <param name="tokenCredential">Token credential. Used only when blobStorage contains storage account name (not connection string).</param>
	public static void AddAzureBlobStorageService<TFileStorageContext>(this IServiceCollection services, string blobStorage, string containerName, TokenCredential tokenCredential)
		where TFileStorageContext : FileStorageContext
	{
		var options = new AzureBlobStorageServiceOptions<TFileStorageContext>
		{
			BlobStorage = blobStorage,
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
