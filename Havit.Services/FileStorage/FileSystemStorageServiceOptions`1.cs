using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services.FileStorage
{
	/// <summary>
	/// Parametry pro nastavení úložiště.
	/// </summary>
	public class FileSystemStorageServiceOptions<TFileStorageContext> : FileSystemStorageServiceOptions
		where TFileStorageContext : FileStorageContext
	{
	}
}
