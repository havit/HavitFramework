using System.Reflection;
using Havit.Services.FileStorage;

namespace Havit.Services.FileStorage
{
	/// <summary>
	/// Parametry pro <see cref="EmbeddedResourceStorageService{TFileStorageContext}"/>.
	/// </summary>
	public class EmbeddedResourceStorageOptions<TFileStorageContext>
		where TFileStorageContext : FileStorageContext
	{
		/// <summary>
		/// Assembly, ve které jsou embedded resources hledány.
		/// </summary>
		public Assembly ResourceAssembly { get; set; }

		/// <summary>
		/// Namespaces, ve kterém jsou soubory hledány.
		/// </summary>
		public string RootNamespace { get; set; }
	}
}
