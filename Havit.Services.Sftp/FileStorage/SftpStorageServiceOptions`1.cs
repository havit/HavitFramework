using Havit.Services.FileStorage;

namespace Havit.Services.Sftp.FileStorage
{
	/// <summary>
	/// Parametry pro SftpStorageService.
	/// Generický parametr TFileStorageContext určen pro možnost použití několika různých služeb v IoC containeru.
	/// </summary>
	public class SftpStorageServiceOptions<TFileStorageServiceContext> : SftpStorageServiceOptions
		where TFileStorageServiceContext : FileStorageContext
	{
	}
}
