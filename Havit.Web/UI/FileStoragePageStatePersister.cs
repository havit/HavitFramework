﻿using Havit.Services.FileStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.Web.UI
{
	/// <summary>
	/// Persister pro uložení viewstate/controlstate do abstraktního úložiště.
	/// </summary>
	public class FileStoragePageStatePersister : PageStatePersister
	{
		private readonly Page page;
		private readonly IFileStorageService fileStorageService;
		private readonly FilePageStatePersister.IFileNamingStrategy fileNamingStrategy;
		private readonly FilePageStatePersisterLogService logService;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="page">Stránka, jejíž viewstate se ukládá či načítá.</param>
		/// <param name="fileStorageService">Služba pro prácu s úložištěm, do nějž se ukládá (a z nějž se načítá) viewstate.</param>
		/// <param name="fileNamingStrategy">Strategie pro pojmenování souborů.</param>
		public FileStoragePageStatePersister(Page page, IFileStorageService fileStorageService, FilePageStatePersister.IFileNamingStrategy fileNamingStrategy) : base(page)
		{
			this.page = page;
			this.fileStorageService = fileStorageService;
			this.fileNamingStrategy = fileNamingStrategy;
			this.logService = new FilePageStatePersisterLogService();
		}

		/// <summary>
		/// Uložení viewstate/controlstate.
		/// </summary>
		public override void Save()
		{
			string storageSymbol = fileNamingStrategy.GetStorageSymbol(); // získáme symbol, ten si dále zapamatujeme "do stránky"
			string storageFilename = fileNamingStrategy.GetFilename(storageSymbol); // ze symbolu získáme celou cestu

			using (var memoryStream = new MemoryStream(4096))
			{
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(memoryStream, new Pair(this.ViewState, this.ControlState));
				memoryStream.Seek(0, SeekOrigin.Begin);

				try
				{
					fileStorageService.Save(storageFilename, memoryStream, "application/octet-stream");
					logService.Log(String.Format("{0}\tSaved", storageFilename));
				}
				catch
				{
					logService.Log(String.Format("{0}\tSave failed", storageFilename), System.Diagnostics.TraceEventType.Error);
					throw;
				}
			}

			// do hidden fieldu si uložíme symbol
			// můžeme ukládat i jinak, ale hidden field pak obsahuje string s koncem řádku! (blbost .NET Frameworku)
			HiddenFieldPageStatePersister hiddenFieldPageStatePersister = new HiddenFieldPageStatePersister(page);
			hiddenFieldPageStatePersister.ControlState = storageSymbol;
			hiddenFieldPageStatePersister.Save();
		}

		/// <summary>
		/// Načtení viewstate/controlstate.
		/// </summary>
		public override void Load()
		{
			// načteme si symbol z hidden fieldu
			HiddenFieldPageStatePersister hiddenFieldPageStatePersister = new HiddenFieldPageStatePersister(page);
			hiddenFieldPageStatePersister.Load();
			string storageSymbol = (string)hiddenFieldPageStatePersister.ControlState;
			string storageFilename = fileNamingStrategy.GetFilename(storageSymbol); // ze symbolu získáme celou cestu

			try
			{
				using (System.IO.Stream stream = fileStorageService.Read(storageFilename))
				{
					Pair pair;
					try
					{
						BinaryFormatter formatter = new BinaryFormatter();
						pair = (Pair)formatter.Deserialize(stream);
					}
					catch (System.Runtime.Serialization.SerializationException) when (stream.CanSeek)
					{
						logService.Log(String.Format("{0}\tDeserialization failed, trying LosFormatter", storageFilename), System.Diagnostics.TraceEventType.Warning);

						// Zpětná kompatibilita - má význam jen pro tu chvíli, než uživatelé přestanou používat viewstaty, které měly vytvořeny před deploymentem.
						// Musíme načíst data znovu, proto seek na začátek, což můžeme jen tehdy, pokud je stream seekovatelný (viz podmínka v catch).
						// Není-li stream seekovatelný, nemáme jak pomoci.
						stream.Seek(0, SeekOrigin.Begin);

						LosFormatter formatter = new LosFormatter();
						pair = (Pair)formatter.Deserialize(stream);
					}

					ViewState = pair.First;
					ControlState = pair.Second;
				}
				logService.Log(String.Format("{0}\tLoaded", storageFilename));
			}
			catch (Exception e)
			{
				logService.Log(String.Format("{0}\tLoad failed", storageFilename), System.Diagnostics.TraceEventType.Error);
				throw new ViewStateLoadFailedException("Nepodařilo se načíst viewstate.", e);
			}
		}
	}
}
