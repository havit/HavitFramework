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
	public class AzureBlobStorageServiceOptions
	{
		/// <summary>
		/// CacheControl, která je nastavena do (CloudBlobReference.)Properties při save (uploadech) souborů.
		/// </summary>
		public string CacheControl { get; set; }
	}
}
