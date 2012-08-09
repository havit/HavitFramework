using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Collections;
using Havit.Collections;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// SortingGridView zajišuje øazení poloek.
	/// Ukládá nastavení øazení dle uivatele a pøípadnì zajišuje automatické øazení pomocí GenericPropertyCompareru.
	/// </summary>
	public abstract class SortingGridView: HighlightingGridView
	{
		#region PerformDataBinding
		/// <summary>
		/// Provede databinding dat.
		/// Pokud data nejsou null a AutoSort je true, automaticky seøadí data pomocí GenericPropertyCompareru.
		/// Pokud uivatel dosud nenastavil ádné øazení, pouije se øazení dle DefaultSortExpression.
		/// </summary>
		/// <param name="data"></param>
		protected override void PerformDataBinding(System.Collections.IEnumerable data)
		{
		}
		#endregion
	}
}
