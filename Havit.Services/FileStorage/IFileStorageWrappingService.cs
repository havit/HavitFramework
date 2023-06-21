namespace Havit.Services.FileStorage;

/// <summary>
/// Wrapuje jinou službu file storage.
/// </summary>
public interface IFileStorageWrappingService : IFileStorageService
{
	/// <summary>
	/// Vrátí wrappovanou službu file storage.
	/// </summary>
	public IFileStorageService GetWrappedFileStorageService();
}