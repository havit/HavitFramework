namespace Havit.Services.FileStorage;

/// <summary>
/// Úložiště souborů pro práci s embedded resources. Podporuje pouze čtení z embedded resources a ověření existence embedded resource.
/// </summary>
public class EmbeddedResourceStorageService<TFileStorageContext> : EmbeddedResourceStorageService, IFileStorageService<TFileStorageContext>
	where TFileStorageContext : FileStorageContext
{
	/// <summary>
	/// Konstruktor.
	/// </summary>
	public EmbeddedResourceStorageService(EmbeddedResourceStorageOptions<TFileStorageContext> options) : base(options.ResourceAssembly, options.RootNamespace)
	{
		// NOOP
	}
}
