using Havit.Services.FileStorage;

namespace Havit.Services.Azure.FileStorage;

/// <summary>
/// Parametry pro AzureFileStorageService.
/// Generický parametr TFileStorageContext určen pro možnost použití několika různých služeb v IoC containeru.
/// </summary>
public class AzureFileStorageServiceOptions<TFileStorageServiceContext> : AzureFileStorageServiceOptions
	where TFileStorageServiceContext : FileStorageContext
{
}
