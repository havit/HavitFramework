using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services.FileStorage
{
	/// <summary>
	/// Parametry pro nastavení úložiště.
	/// Generický parametr TFileStorageContext určen pro možnost použití několika různých konfigurací v IoC containeru.
	/// </summary>
	public class FileSystemStorageServiceOptions<TFileStorageContext>
		where TFileStorageContext : FileStorageContext
	{
		/// <summary>
		/// Cesta k "rootu" použitého úložiště ve file systému.
		/// </summary>
		public string StoragePath { get; set; }

		/// <summary>
		/// Parametry šifrování.
		/// </summary>
		public EncryptionOptions EncryptionOptions { get; set; }
	}
}
