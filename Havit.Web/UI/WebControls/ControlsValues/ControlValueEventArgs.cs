using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace Havit.Web.UI.WebControls.ControlsValues
{
	/// <summary>
	/// Parametry událostí předávající Control a hodnotu
	/// </summary>
	public class ControlValueEventArgs : EventArgs
	{
		/// <summary>
		/// Control, ke kterému se událost vztahuje.
		/// </summary>
		public Control Control
		{
			get;
			private set;
		}

		/// <summary>
		/// Hodnota, která se k události vztahuje.
		/// </summary>
		public object Value
		{
			get;
			private set;
		}

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public ControlValueEventArgs(Control control, object value)
		{
			this.Control = control;
			this.Value = value;
		}
	}
}
