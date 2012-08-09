using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Specifikuje formaty data/casu.
	/// </summary>
	public enum DateTimePart
	{
        Date, // datum, predpoklada se ulozeni do DateTime
        Time, // cas, predpoklada se ulozeni do DateTime
        DateTime // datum a cas, predpoklada se ulozeni do DateTime
	}
}
