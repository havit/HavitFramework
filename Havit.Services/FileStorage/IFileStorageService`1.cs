using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

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
