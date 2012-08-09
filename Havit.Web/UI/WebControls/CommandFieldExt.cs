using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Rozšíøená verze <see cref="System.Web.UI.WebControls.ButtonField"/> (základní rozšíøení spoleèné pro GridView i jiná použití fieldu).
	/// Pro použití v <see cref="GridViewExt"/> a odvozených (napø. <see cref="Havit.Web.UI.WebControls.EnterpriseGridView"/>)
	/// je doporuèeno použít bohatšího potomka <see cref="GridViewCommandField"/>.
	/// </summary>
	public class CommandFieldExt : CommandField, IIdentifiableField
	{
		#region ID (IIdentifiableField Members)
		/// <summary>
		/// Identifikátor fieldu na který se lze odkazovat pomocí <see cref="GridViewExt.FindColumn(string)"/>.
		/// </summary>
		public string ID
		{
			get
			{
				object tmp = ViewState["ID"];
				if (tmp != null)
				{
					return (string)tmp;
				}
				return String.Empty;
			}
			set
			{
				ViewState["ID"] = value;
			}
		}
		#endregion
	}
}
