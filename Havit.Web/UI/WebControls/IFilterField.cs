using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Označuje field, který lze použít pro filtrování.
	/// </summary>
	public interface IFilterField 
	{
		/// <summary>
		/// Styl pro buňku filtru.
		/// </summary>
		TableItemStyle FilterStyleInternal { get; }

		/// <summary>
		/// Inicializuje buňku filtru.
		/// </summary>
		void InitializeFilterCell(DataControlFieldCell cell);
	}
}
