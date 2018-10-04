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
	public interface IFileStorageService
	{
		/// <summary>
		/// Vrátí true, pokud uložený soubor v úložišti existuje. Jinak false.
		/// </summary>
		bool Exists(string fileName);

		/// <summary>
		/// Vrátí stream s obsahem souboru z úložiště.
		/// </summary>
		Stream Read(string fileName);
		
		/// <summary>
		/// Zapíše obsah souboru z úložiště do streamu.
		/// </summary>
		void ReadToStream(string fileName, Stream stream);

		/// <summary>
		/// Uloží stream do úložiště.
		/// </summary>
		void Save(string fileName, Stream fileContent, string contentType);

		/// <summary>
		/// Smaže soubor v úložišti.
		/// </summary>
		void Delete(string fileName);

		/// <summary>
		/// Vylistuje seznam souborů v úložišti.
		/// </summary>
		IEnumerable<FileInfo> EnumerateFiles(string pattern = null);

		/// <summary>
		/// Vrátí čas poslední modifikace souboru v UTC timezone
		/// </summary>
		DateTime? GetLastModifiedTimeUtc(string fileName);
	}
}
