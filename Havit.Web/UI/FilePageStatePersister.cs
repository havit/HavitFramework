using System;
using System.Collections.Generic;
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
	public partial class FilePageStatePersister : PageStatePersister
	{
		#region Private fields
		private Page _page;
		private IFileNamingStrategy _fileNamingStrategy;
		private ILogService _logService;
		#endregion

		#region Constructors
		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="page">Stránka, jejíž viewstate se ukládá či načítá.</param>
		/// <param name="fileNamingStrategy">Strategie pro pojmenování souborů</param>
		public FilePageStatePersister(Page page, IFileNamingStrategy fileNamingStrategy) : base(page)
		{
			_page = page;
			_fileNamingStrategy = fileNamingStrategy;
			_logService = new FilePageStatePersisterLogService();
		}

		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="page">Stránka, jejíž viewstate se ukládá či načítá.</param>
		/// <param name="root">Použije strategii PerUserNamingStrategy s  daným rootem.</param>
		public FilePageStatePersister(Page page, string root)
			: this(page, new PerUserNamingStrategy(root))
		{
		}
		#endregion

		#region Save
		/// <summary>
		/// Uložení viewstate/controlstate.
		/// </summary>
		public override void Save()
		{
			string storageSymbol = _fileNamingStrategy.GetStorageSymbol(); // získáme symbol, ten si dále zapamatujeme "do stránky"
			string storageFilename = _fileNamingStrategy.GetFilename(storageSymbol); // ze symbolu získáme celou cestu

			System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(storageFilename)); // založíme složku, pokud ještě neexistuje (jinak File.CreateText padá)

			using (System.IO.FileStream fileStream = System.IO.File.Open(storageFilename, System.IO.FileMode.Create))
			{
				LosFormatter formatter = new LosFormatter();
				formatter.Serialize(fileStream, new Pair(this.ViewState, this.ControlState));
				_logService.Log(String.Format("{0}\tSaved", storageFilename));
			}

			// do hidden fieldu si uložíme symbol
			// můžeme ukládat i jinak, ale hidden field pak obsahuje string s koncem řádku! (blbost .NET Frameworku)
			HiddenFieldPageStatePersister hiddenFieldPageStatePersister = new HiddenFieldPageStatePersister(_page);
			hiddenFieldPageStatePersister.ControlState = storageSymbol;
			hiddenFieldPageStatePersister.Save();
		}
		#endregion

		#region Load
		/// <summary>
		/// Načtení viewstate/controlstate.
		/// </summary>
		public override void Load()
		{
			// načteme si symbol z hidden fieldu
			HiddenFieldPageStatePersister hiddenFieldPageStatePersister = new HiddenFieldPageStatePersister(_page);
			hiddenFieldPageStatePersister.Load();
			string storageSymbol = (string) hiddenFieldPageStatePersister.ControlState;
			string storageFilename = _fileNamingStrategy.GetFilename(storageSymbol); // ze symbolu získáme celou cestu

			try
			{
				using (System.IO.FileStream fileStream = System.IO.File.Open(storageFilename, System.IO.FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete))
				{
					LosFormatter formatter = new LosFormatter();
					Pair pair = (Pair)formatter.Deserialize(fileStream);

					ViewState = pair.First;
					ControlState = pair.Second;
				}
				_logService.Log(String.Format("{0}\tLoaded", storageFilename));
			}
			catch (Exception e)
			{
				_logService.Log(String.Format("{0}\tLoad failed", storageFilename));
				throw new ViewStateLoadFailedException("Nepodařilo se načíst viewstate.", e);
			}
		}
		#endregion

		#region IFileNamingStrategy (interface)
		/// <summary>
		/// Strategie pro pojmenování souborů.
		/// </summary>
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
		#endregion

		#region ILogService (interface)
		/// <summary>
		/// Služba pro logování operací file page persisteru.
		/// </summary>
		internal interface ILogService
		{
			/// <summary>
			/// Zapíše zprávu do logu.
			/// </summary>
			void Log(string message);
		}
		#endregion
	}
}
