using System;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Třída implementující special date zobrazovaný v DateTimeBox-u.
	/// </summary>
	public class SpecialDate
	{
		#region Datum
		/// <summary>
		/// Datum.
		/// </summary>
		public DateTime Datum { get; private set; }
		#endregion

		#region Disabled
		/// <summary>
		/// Příznak zakázání výběru data.
		/// </summary>		
		public bool Disabled { get; private set; }
		#endregion

		#region CssClass
		/// <summary>
		/// CSS class.
		/// </summary>
		public string CssClass { get; private set; }
		#endregion

		#region SpecialDate
		/// <summary>
		/// SpecialDate (ctor)
		/// </summary>		
		public SpecialDate(DateTime datum, bool disabled, string cssClass)
		{
			Datum = datum;
			Disabled = disabled;
			CssClass = cssClass;
		}
		#endregion
	}
}