using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// GridViewInsertedEventArgs.
	/// </summary>
	public class GridViewInsertedEventArgs : EventArgs
	{
		#region KeepInEditMode
		/// <summary>
		/// Indikuje, zdali má GridView zůstat po zpracování událost v režimu editace nového řádku.
		/// </summary>
		public bool KeepInEditMode
		{
			get
			{
				return this._keepInEditMode;
			}
			set
			{
				this._keepInEditMode = value;
			}
		}
		private bool _keepInEditMode = false;
		#endregion
	}
}
