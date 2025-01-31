﻿using System.Web.UI;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// Reprezentuje label, který se renderuje jako HTML tag H3.
/// </summary>
public class H3 : System.Web.UI.WebControls.Label
{
	/// <summary>
	/// Vrací HtmlTextWriterTag.H3 zajišťující správné renderování.
	/// </summary>
	protected override HtmlTextWriterTag TagKey
	{
		get
		{
			return HtmlTextWriterTag.H3;
		}
	}
}
