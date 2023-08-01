namespace Havit.Services.FileStorage;

/// <summary>
/// Úložiště souborů.
/// </summary>
/// <remarks>
/// Dědí z IFileStorageService pro možnost předání generického úložiště do obecné metody.
/// </remarks>
public interface IFileStorageService<TFileStorageContext> : IFileStorageService
	where TFileStorageContext : FileStorageContext
{
}
