using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services.FileStorage
{
	/// <summary>
	/// IFileStorageService s file systémem pro datové úložiště.
	/// </summary>
	public class FileSystemStorageService : IFileStorageService
	{
		#region Private fields
		private readonly string storagePath;
		#endregion

		#region Constructor
		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="storagePath">Cesta k "rootu" použitého úložiště ve file systému.</param>
		public FileSystemStorageService(string storagePath)
		{
			this.storagePath = storagePath;
		}
		#endregion

		#region Exists
		/// <summary>
		/// Vrátí true, pokud uložený soubor v úložišti existuje. Jinak false.
		/// </summary>
		public bool Exists(string fileName)
		{
			return File.Exists(System.IO.Path.Combine(storagePath, fileName));
		}
		#endregion

		#region Read
		/// <summary>
		/// Vrátí stream s obsahem soubor z úložiště.
		/// </summary>
		public Stream Read(string fileName)
		{
			return File.OpenRead(System.IO.Path.Combine(storagePath, fileName));
		}
		#endregion

		#region ReadToStream
		/// <summary>
		/// Zapíše obsah souboru z úložiště do streamu.
		/// </summary>
		public void ReadToStream(string fileName, Stream stream)
		{
			using (Stream fileStream = File.OpenRead(System.IO.Path.Combine(storagePath, fileName)))
			{
				fileStream.CopyTo(stream);
			}
		}
		#endregion

		#region Save
		/// <summary>
		/// Uloží stream do úložiště.
		/// </summary>
		public void Save(string fileName, Stream fileContent, string contentType)
		{
			using (FileStream fileStream = File.OpenWrite(System.IO.Path.Combine(storagePath, fileName)))
			{
				fileContent.CopyTo(fileStream);
			}
		}
		#endregion

		#region Delete
		/// <summary>
		/// Smaže soubor v úložišti.
		/// </summary>
		public void Delete(string fileName)
		{
			System.IO.File.Delete(System.IO.Path.Combine(storagePath, fileName));
		}
		#endregion

		#region GetLastModifiedTimeUtc
		/// <summary>
		/// Vrátí čas poslední modifikace souboru v UTC timezone
		/// </summary>
		public DateTime? GetLastModifiedTimeUtc(string fileName)
		{
			return File.GetLastWriteTimeUtc(Path.Combine(storagePath, fileName));
		}
		#endregion

	}
}
