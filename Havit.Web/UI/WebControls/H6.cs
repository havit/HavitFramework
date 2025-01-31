﻿using System.Web.UI;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// Reprezentuje label, který se renderuje jako HTML tag H6.
/// </summary>
public class H6 : System.Web.UI.WebControls.Label
{
	/// <summary>
	/// Vrací HtmlTextWriterTag.H6 zajišťující správné renderování.
	/// </summary>
	protected override HtmlTextWriterTag TagKey
	{
		get
		{
			return HtmlTextWriterTag.H6;
		}
	}
}
