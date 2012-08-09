using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// EnterprisGridView poskytuje:
	/// - hledání klíče řádku, ve kterém došlo k události
	/// - hledání sloupce (IEnterpriseField) na základě ID sloupce
	/// - stránkování
	/// - zveřejňuje vlastnost RequiresDataBinding
	/// - automatický databinding při prvním načtení stránky nebo nastavení RequiresDataBinding na true (podmíněno vlastností AutoDataBind)
	/// - přechod na stránku 0 při změně řazení
	/// </summary>
	public class EnterpriseGridView : GridViewExt
	{
		#region Constructor
		/// <summary>
		/// Vytvoří instanci EnterpriseGridView. Nastavuje defaultní DataKeyNames na ID.
		/// </summary>
		public EnterpriseGridView()
		{
			this.DataKeyNames = new string[] { "ID" };
		}
		#endregion

		#region GetRowID - Hledání klíče položky
		/// <summary>
		/// Nalezne hodnotu ID klíče položky, ve kterém se nachází control.
		/// </summary>
		/// <param name="control">Control. Hledá se řádek, ve kterém se GridViewRow nalézá a DataKey řádku.</param>
		/// <returns>Vrací hodnotu klíče.</returns>
		public int GetRowID(Control control)
		{
			return (int)GetRowKey(control).Value;
		}

		/// <summary>
		/// Nalezne hodnotu ID klíče položky na základě události.
		/// </summary>
		/// <param name="e">Událost, ke které v gridu došlo.</param>
		/// <returns>Vrací hodnotu klíče daného řádku.</returns>
		public int GetRowID(GridViewCommandEventArgs e)
		{
			return (int)GetRowKey(e).Value;
		}

		/// <summary>
		/// Nalezne hodnotu ID klíče položky na základě indexu řádku v gridu.
		/// </summary>
		/// <param name="rowIndex">index řádku</param>
		/// <returns>Vrací hodnotu klíče daného řádku.</returns>
		public int GetRowID(int rowIndex)
		{
			return (int)GetRowKey(rowIndex).Value;
		}
		#endregion

		//region Zrušení DataSourceID (Naše řazení jej nepodporuje.)
		///// <summary>
		///// Zrušíme možnost nastavení DataSourceID. Při pokusu nastavit not-null hodnotu
		///// dojde k vyvolání výjimky.
		///// </summary>
		//public override string DataSourceID
		//{
		//    get
		//    {
		//        return base.DataSourceID;
		//    }
		//    set
		//    {
		//        if (value != null)
		//            throw new ArgumentException("DataSourceID not supported.");
		//        base.DataSourceID = value;
		//    }
		//}
		//endregion
	}
}
