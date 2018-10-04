using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Web.UI.WebControls.ControlsValues
{
	/// <summary>
	/// Delegát události předávající parametry typu ControlValueEventArgs.
	/// Používá se pro událost oznamující nastavení hodnoty do controlu.
	/// </summary>
	public delegate void ControlValueSetEventHandler(object sender, ControlValueEventArgs e);
}
