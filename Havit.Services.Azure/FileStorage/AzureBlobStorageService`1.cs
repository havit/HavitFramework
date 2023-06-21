using Havit.Services.FileStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services.Azure.FileStorage;

/// <summary>
/// Úložiště souborů jako Azure Blob Storage.
/// Podporuje client-side šifrování (z předka FileStorageServiceBase).
/// Generický parametr TFileStorageContext určen pro možnost použití několika různých služeb v IoC containeru.
/// </summary>
public class AzureBlobStorageService<TFileStorageContext> : AzureBlobStorageService, IFileStorageService<TFileStorageContext>
	where TFileStorageContext : FileStorageContext
{
	/// <summary>
	/// Konstruktor.
	/// </summary>
	public AzureBlobStorageService(AzureBlobStorageServiceOptions<TFileStorageContext> options) : base(options)
	{
		// NOOP
	}
}
