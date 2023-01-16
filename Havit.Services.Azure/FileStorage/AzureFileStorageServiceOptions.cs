using Azure.Core;
using Havit.Services.FileStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services.Azure.FileStorage
{
	/// <summary>
	/// Parametry pro AzureBlobStorageService.
	/// </summary>
	public class AzureFileStorageServiceOptions
	{
		/// <summary>
		/// Connection string pro připojení k Azure File Storage.
		/// </summary>
		public string FileStorageConnectionString { get; set; }

		/// <summary>
		/// File Share ve File Storage pro práci se soubory.
		/// </summary>
		public string FileShareName { get; set; }

		/// <summary>
		/// Název složky, která bude rootem pro použití.
		/// </summary>
		public string RootDirectoryName { get; set; }

		/// <summary>
		/// Indikuje, zda má automaticky zakládat FileShare, pokud neexistuje.
		/// </summary>
		public bool AutoCreateFileShare { get; set; } = true;

		/// <summary>
		/// Indikuje, zda má automaticky zakládat neexistující složky použité ve filename při zakládání FileShare nebo při ukládání souboru.
		/// </summary>
		public bool AutoCreateDirectories { get; set; } = true;
	}
}
