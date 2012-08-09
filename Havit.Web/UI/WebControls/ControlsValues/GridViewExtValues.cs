using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Havit.Collections;
using System.Xml.Serialization;
using Havit.Web.UI.WebControls.ControlsValues;

namespace Havit.Goran.WebBase.UI.WebControls.ControlsValues
{	
	/// <summary>
	/// Reprezentuje stav GridViewExt pro uložení persisterem.
	/// </summary>
	public class GridViewExtValue
	{
		#region Constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		public GridViewExtValue()
		{
			AllowPaging = true; // default value (zpětná kompatibilita)
		}
		#endregion

		#region AllowPaging
		/// <summary>
		/// Indikuje povolení stránkovat.
		/// Nutné pro sledování v situaci, kdy uživatel klikne ve stránkování na zobrazení "vše".
		/// </summary>
		public bool AllowPaging
		{
			get;
			set;
		}
		#endregion

		#region PageIndex
		/// <summary>
		/// Index zobrazené stránky dat.
		/// </summary>
		public int PageIndex
		{
			get;
			set;
		}
		#endregion

		#region SortItems
		/// <summary>
		/// Řazení.
		/// </summary>
		public SortItemCollection SortItems
		{
			get;
			set;
		}
		#endregion
		
	}
}
