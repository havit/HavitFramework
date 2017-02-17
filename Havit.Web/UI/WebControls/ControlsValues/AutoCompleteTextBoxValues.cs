using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Web.UI.WebControls.ControlsValues
{
	/// <summary>
	/// Reprezentuje stav AutoCompleteTextBox pro uložení persisterem.
	/// </summary>
	public class AutoCompleteTextBoxValues
	{
		/// <summary>
		/// Identifikátor položky.
		/// </summary>
		public string SelectedValue
		{
			get;
			set;
		}

		/// <summary>
		/// Text pro zobrazení položky.
		/// </summary>
		public string Text
		{
			get;
			set;
		}
	}
}