using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Rozšířená verze <see cref="System.Web.UI.WebControls.CheckBoxField"/>.
	/// </summary>
	public class CheckBoxFieldExt : CheckBoxField, IIdentifiableField
	{
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
	}
}
