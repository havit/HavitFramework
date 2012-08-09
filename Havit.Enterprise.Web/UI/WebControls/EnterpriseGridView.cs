using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// EnterprisGridView poskytuje:
	/// - hledání klíèe øádku, ve kterém došlo k události
	/// - hledání sloupce (IEnterpriseField) na základì ID sloupce
	/// - stránkování
	/// - zveøejòuje vlastnost RequiresDataBinding
	/// - automatický databinding pøi prvním naètení stránky nebo nastavení RequiresDataBinding na true (podmínìno vlastností AutoDataBind)
	/// - pøechod na stránku 0 pøi zmìnì øazení
	/// </summary>
	public class EnterpriseGridView : GridViewExt
	{
		#region Constructor
		/// <summary>
		/// Vytvoøí instanci EnterpriseGridView. Nastavuje defaultní DataKeyNames na ID.
		/// </summary>
		public EnterpriseGridView()
		{
			this.DataKeyNames = new string[] { "ID" };
		}
		#endregion

		#region GetRowID - Hledání klíèe položky
		/// <summary>
		/// Nalezne hodnotu ID klíèe položky, ve kterém se nachází control.
		/// </summary>
		/// <param name="control">Control. Hledá se øádek, ve kterém se GridViewRow nalézá a DataKey øádku.</param>
		/// <returns>Vrací hodnotu klíèe.</returns>
		public int GetRowID(Control control)
		{
			return (int)GetRowKey(control).Value;
		}

		/// <summary>
		/// Nalezne hodnotu ID klíèe položky na základì události.
		/// </summary>
		/// <param name="e">Událost, ke které v gridu došlo.</param>
		/// <returns>Vrací hodnotu klíèe daného øádku.</returns>
		public int GetRowID(GridViewCommandEventArgs e)
		{
			return (int)GetRowKey(e).Value;
		}

		/// <summary>
		/// Nalezne hodnotu ID klíèe položky na základì indexu øádku v gridu.
		/// </summary>
		/// <param name="rowIndex">index øádku</param>
		/// <returns>Vrací hodnotu klíèe daného øádku.</returns>
		public int GetRowID(int rowIndex)
		{
			return (int)GetRowKey(rowIndex).Value;
		}
		#endregion

		//region Zrušení DataSourceID (Naše øazení jej nepodporuje.)
		///// <summary>
		///// Zrušíme možnost nastavení DataSourceID. Pøi pokusu nastavit not-null hodnotu
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
