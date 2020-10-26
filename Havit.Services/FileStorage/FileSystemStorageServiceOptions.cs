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
	public class FileSystemStorageServiceOptions
	{
		/// <summary>
		/// Cesta k "rootu" použitého úložiště ve file systému.
		/// </summary>
		public string StoragePath { get; set; }		
	}
}
