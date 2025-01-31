﻿using System.Collections;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// Interface označuje control, který je možné použít pro automatické filtrování v gridu.
/// Takový control se musí vyvoláním události (RaiseBubbleEvent) s argumentem AutoFilterControlCreatedEventArgs.Empty registrovat. jak control pro automatický databind.
/// Změny filtru oznamuje událostí ValueChanged, filtrování dat provádí metodou FilterData.
/// </summary>
public interface IAutoFilterControl
{
	/// <summary>
	/// Oznamuje změnu filtru.
	/// </summary>
	event EventHandler ValueChanged;

	/// <summary>
	/// Filtruje data dle nastavení filtru.
	/// </summary>
	IEnumerable FilterData(IEnumerable data);
}
