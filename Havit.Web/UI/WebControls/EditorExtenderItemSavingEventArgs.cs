using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Argumenty události ukládání datového objektu.
	/// </summary>
	public class EditorExtenderItemSavingEventArgs : CancelEventArgs
	{
		/// <summary>
		/// Ukládaný datový objekt.
		/// </summary>
		public object EditedObject { get; set; }

		/// <summary>
		/// Skutečně uložený objekt. Slouží pro vyjímečné případy, kdy je obsluhou události nakonec uložen jiný objekt než SavedObject (typicky při zakládání nového objektu). Účelem obsluhy události je nastavit tuto vlastnost na skutečně editovaný objekt, pokud je jiný, než SavedObject.
		/// </summary>
		public object SavedObject { get; set; }

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public EditorExtenderItemSavingEventArgs()
		{
		}

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public EditorExtenderItemSavingEventArgs(object editedObject)
		{
			this.EditedObject = editedObject;
		}
	}

	/// <summary>
	/// Událost ukládání datového objektu.
	/// </summary>
	public delegate void EditorExtenderItemSavingEventHandler(object sender, EditorExtenderItemSavingEventArgs args);
}
