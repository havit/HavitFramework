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
		/// Nelze kombinovat s UseFullyQualifiedPathNames.
		/// </summary>
		public string StoragePath { get; set; }

		/// <summary>
		/// Cesta k "rootu" použitého úložiště nebude použita a veškerá volání služby použijí plně kvalifikovaný název souboru.
		/// Nelze kombinovat s StoragePath.
		/// </summary>
		public bool UseFullyQualifiedPathNames { get; set; }

		/// <summary>
		/// Parametry šifrování.
		/// </summary>
		public EncryptionOptions EncryptionOptions { get; set; }
	}
}
