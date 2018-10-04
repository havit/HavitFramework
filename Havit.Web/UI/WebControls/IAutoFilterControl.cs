using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Interface označuje control, který je možné použít pro automatické filtrování v gridu.
	/// Takový control se musí vyvoláním události (RaiseBubbleEvent) s argumentem AutoFilterControlCreatedEventArgs.Empty registrovat. jak control pro automatický databind.
	/// Změny filtru oznamuje událostí ValueChanged, filtrování dat provádí metodou FilterData.
	/// </summary>
	public interface IAutoFilterControl
	{
		#region ValueChanged
		/// <summary>
		/// Oznamuje změnu filtru.
		/// </summary>
		event EventHandler ValueChanged;
		#endregion

		#region FilterData
		/// <summary>
		/// Filtruje data dle nastavení filtru.
		/// </summary>
		IEnumerable FilterData(IEnumerable data);
		#endregion
	}
}
