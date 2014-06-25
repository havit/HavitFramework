using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Argumenty události uložení datového objektu.
	/// </summary>
	public class EditorExtenderItemSavedEventArgs : CancelEventArgs
	{
		/// <summary>
		/// Editovaný datový objekt. Slouží k použití v obsluze události ukládání objektu.
		/// </summary>
		public object SavedObject { get; set; }

		#region Constructors
		/// <summary>
		/// Konstruktor.
		/// </summary>
		public EditorExtenderItemSavedEventArgs()
		{
			
		}

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public EditorExtenderItemSavedEventArgs(object savedObject)
		{
			this.SavedObject = savedObject;
		}
		#endregion
	}

	/// <summary>
	/// Událost uložení datového objektu.
	/// </summary>
	public delegate void EditorExtenderItemSavedEventHandler(object sender, EditorExtenderItemSavedEventArgs args);

}
