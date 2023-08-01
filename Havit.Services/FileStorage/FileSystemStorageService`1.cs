namespace Havit.Services.FileStorage;

/// <summary>
/// IFileStorageService a IFileStorageServiceAsync s file systémem pro datové úložiště.
/// Některé asynchronní metody pod pokličkou nejsou asynchronní, viz dokumentace jednotlivých metod (jejichž název končí Async).
/// Generický parametr TFileStorageContext určen pro možnost použití několika různých služeb v IoC containeru.
/// </summary>
public class FileSystemStorageService<TFileStorageContext> : FileSystemStorageService, IFileStorageService<TFileStorageContext>
	where TFileStorageContext : FileStorageContext
{
	/// <summary>
	/// Konstruktor.
	/// </summary>
	public FileSystemStorageService(FileSystemStorageServiceOptions<TFileStorageContext> options) : base(options.StoragePath, options.UseFullyQualifiedPathNames, options.EncryptionOptions)
	{
		// NOOP
	}
}
