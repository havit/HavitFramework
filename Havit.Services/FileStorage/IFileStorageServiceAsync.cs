using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services.FileStorage
{
	/// <summary>
	/// Úložiště souborů.
	/// </summary>
	public interface IFileStorageServiceAsync
	{
		/// <summary>
		/// Vrátí true, pokud uložený soubor v úložišti existuje. Jinak false.
		/// </summary>
		Task<bool> ExistsAsync(string fileName);

		/// <summary>
		/// Vrátí stream s obsahem souboru z úložiště.
		/// </summary>
		Task<Stream> ReadAsync(string fileName);
		
		/// <summary>
		/// Zapíše obsah souboru z úložiště do streamu.
		/// </summary>
		Task ReadToStreamAsync(string fileName, Stream stream);

		/// <summary>
		/// Uloží stream do úložiště.
		/// </summary>
		Task SaveAsync(string fileName, Stream fileContent, string contentType);

		/// <summary>
		/// Smaže soubor v úložišti.
		/// </summary>
		Task DeleteAsync(string fileName);

		/// <summary>
		/// Vylistuje seznam souborů v úložišti.
		/// </summary>
		Task<IEnumerable<FileInfo>> EnumerateFilesAsync(string pattern = null);

		/// <summary>
		/// Vrátí čas poslední modifikace souboru v UTC timezone
		/// </summary>
		Task<DateTime?> GetLastModifiedTimeUtcAsync(string fileName);
	}
}
