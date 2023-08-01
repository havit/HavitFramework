using Havit.Services.FileStorage;

namespace Havit.Services.Azure.FileStorage;

/// <summary>
/// Parametry pro AzureBlobStorageService.
/// Generický parametr TFileStorageContext určen pro možnost použití několika různých služeb v IoC containeru.
/// </summary>
public class AzureBlobStorageServiceOptions<TFileStorageServiceContext> : AzureBlobStorageServiceOptions
	where TFileStorageServiceContext : FileStorageContext
{
}
