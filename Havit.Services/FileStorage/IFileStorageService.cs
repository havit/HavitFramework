using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services.FileStorage
{
	/// <summary>
	/// Úložiště souborů.
	/// </summary>
	public interface IFileStorageService
	{
		#region ExistsFile
		/// <summary>
		/// Vrátí true, pokud uložený soubor v úložišti existuje. Jinak false.
		/// </summary>
		bool Exists(string fileName);
		#endregion

		#region Read
		/// <summary>
		/// Vrátí stream s obsahem souboru z úložiště.
		/// </summary>
		Stream Read(string fileName);
		#endregion
		
		#region ReadToStream
		/// <summary>
		/// Zapíše obsah souboru z úložiště do streamu.
		/// </summary>
		void ReadToStream(string fileName, Stream stream);
		#endregion

		#region Save
		/// <summary>
		/// Uloží stream do úložiště.
		/// </summary>
		void Save(string fileName, Stream fileContent, string contentType);
		#endregion

		#region Delete
		/// <summary>
		/// Smaže soubor v úložišti.
		/// </summary>
		void Delete(string fileName);
		#endregion

		#region GetLastModifiedTimeUtc
		/// <summary>
		/// Vrátí čas poslední modifikace souboru v UTC timezone
		/// </summary>
		DateTime? GetLastModifiedTimeUtc(string fileName);
		#endregion
	}
}
