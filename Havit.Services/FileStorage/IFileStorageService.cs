using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
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
		/// Vrátí true, pokud uložený soubor v úložišti existuje. Jinak false.
		/// </summary>
		Task<bool> ExistsAsync(string fileName, CancellationToken cancellationToken = default);

		/// <summary>
		/// Vrátí stream s obsahem souboru z úložiště.
		/// </summary>
		Stream Read(string fileName);

		/// <summary>
		/// Vrátí stream s obsahem souboru z úložiště.
		/// </summary>
		Task<Stream> ReadAsync(string fileName, CancellationToken cancellationToken = default);

		/// <summary>
		/// Zapíše obsah souboru z úložiště do streamu.
		/// </summary>
		void ReadToStream(string fileName, Stream stream);

		/// <summary>
		/// Zapíše obsah souboru z úložiště do streamu.
		/// </summary>
		Task ReadToStreamAsync(string fileName, Stream stream, CancellationToken cancellationToken = default);

		/// <summary>
		/// Uloží stream do úložiště.
		/// </summary>
		void Save(string fileName, Stream fileContent, string contentType);

		/// <summary>
		/// Uloží stream do úložiště.
		/// </summary>
		Task SaveAsync(string fileName, Stream fileContent, string contentType, CancellationToken cancellationToken = default);

		/// <summary>
		/// Smaže soubor v úložišti.
		/// </summary>
		void Delete(string fileName);

		/// <summary>
		/// Smaže soubor v úložišti.
		/// </summary>
		Task DeleteAsync(string fileName, CancellationToken cancellationToken = default);

		/// <summary>
		/// Vylistuje seznam souborů v úložišti.
		/// </summary>
		IEnumerable<FileInfo> EnumerateFiles(string pattern = null);

		/// <summary>
		/// Vylistuje seznam souborů v úložišti.
		/// </summary>
		IAsyncEnumerable<FileInfo> EnumerateFilesAsync(string pattern = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Vrátí čas poslední modifikace souboru v UTC timezone
		/// </summary>
		DateTime? GetLastModifiedTimeUtc(string fileName);

		/// <summary>
		/// Vrátí čas poslední modifikace souboru v UTC timezone
		/// </summary>
		Task<DateTime?> GetLastModifiedTimeUtcAsync(string fileName, CancellationToken cancellationToken = default);
	}
}
