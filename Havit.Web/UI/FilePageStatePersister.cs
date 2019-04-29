using Havit.Services.FileStorage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.Web.UI
{
	/// <summary>
	/// Persister pro uložení viewstate/controlstate do filesystému.
	/// </summary>
	public partial class FilePageStatePersister : FileStoragePageStatePersister
	{
		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="page">Stránka, jejíž viewstate se ukládá či načítá.</param>
		/// <param name="root">Složka, do které se ukládá viewstate.</param>
		/// <param name="fileNamingStrategy">Strategie pro pojmenování souborů.</param>
		public FilePageStatePersister(Page page, string root, IFileNamingStrategy fileNamingStrategy) : base(page, new FileSystemStorageService(root), fileNamingStrategy)
		{
		}

		/// <summary>
		/// Konstruktor. Použije strategii PerUserNamingStrategy.
		/// </summary>
		/// <param name="page">Stránka, jejíž viewstate se ukládá či načítá.</param>
		/// <param name="root">Složka, do které se ukládá viewstate.</param>
		public FilePageStatePersister(Page page, string root)
			: this(page, root, new PerUserNamingStrategy())
		{
		}

		/// <summary>
		/// Strategie pro pojmenování souborů.
		/// </summary>
		/// <remarks>
		/// Jako vnitřní třída v FilePageStatePersisteru je pouze z důvodu zpětné kompatibility. S předkem FileStoragePageStatePersisterBase, který ji používá, není situace úplně přehledná a pochopitelná.
		/// </remarks>
		public interface IFileNamingStrategy
		{
			/// <summary>
			/// Získá symbol. Tj. určitý klíč, na základě kterého se pak tvoří název souboru, do kterého se ukládá viewstate/controlstate.
			/// </summary>
			string GetStorageSymbol();

			/// <summary>
			/// Ze symbolu určí název souboru (včetně cesty), do kterého se ukládá viewstate/controlstate.
			/// </summary>
			string GetFilename(string storageSymbol);
		}

		/// <summary>
		/// Služba pro logování operací file page persisteru.
		/// </summary>
		/// <remarks>
		/// Jako vnitřní třída v FilePageStatePersisteru je pouze z důvodu zpětné kompatibility. S předkem FileStoragePageStatePersisterBase, který ji používá, není situace úplně přehledná a pochopitelná.
		/// </remarks> 
		internal interface ILogService
		{
			/// <summary>
			/// Zapíše zprávu do logu.
			/// </summary>
			void Log(string message, TraceEventType eventType = TraceEventType.Information);
		}
	}
}
