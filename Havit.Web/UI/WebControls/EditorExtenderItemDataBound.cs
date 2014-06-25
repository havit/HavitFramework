using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Argumenty události nabindování editoru datovým objektem.
	/// </summary>
	public class EditorExtenderItemDataBoundEventArgs : EventArgs
	{
		#region SavedObject
		/// <summary>
		/// Bidnovaný datový objekt.
		/// </summary>
		public object DataItem { get; set; }
		#endregion

		#region Constructor
		/// <summary>
		/// Konstruktor.
		/// </summary>
		public EditorExtenderItemDataBoundEventArgs(object dataItem)
		{
			this.DataItem = dataItem;
		}
		#endregion
	}

	/// <summary>
	/// Událost nabindování editoru datovým objektem.
	/// </summary>
	public delegate void EditorExtenderItemDataBoundEventHandler(object sender, EditorExtenderItemDataBoundEventArgs args);

}
