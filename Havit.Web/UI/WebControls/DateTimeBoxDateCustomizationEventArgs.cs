using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// Argument události 
/// </summary>
public class DateTimeBoxDateCustomizationEventArgs : EventArgs
{
	/// <summary>
	/// Vlastnost reprezentující customizaci dnů zobrazovaných v DateTimeBox-u.
	/// </summary>
	public DateTimeBoxDateCustomization DateCustomization { get; set; }
}
