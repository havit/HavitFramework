using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Sloupec, který se umí identifikovat (má ID).
	/// </summary>
	public interface IIdentifiableField
	{
		/// <summary>
		/// ID sloupce. Umožní GridView hledat sloupec podle ID.
		/// </summary>
		string ID { get; }
	}
}
