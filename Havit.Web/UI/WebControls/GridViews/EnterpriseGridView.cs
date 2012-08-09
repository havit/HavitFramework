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
	public class EnterpriseGridView : SortingGridView
	{
		#region Constructor
		/// <summary>
		/// Vytvoøí instanci EnterpriseGridView.
		/// Vypne automatické generování sloupcù pøi databindingu (nastaví AutoGenerateColumns na false).
		/// </summary>
		public EnterpriseGridView()
		{
			AutoGenerateColumns = false;
		}
		#endregion

		#region Properties

		/// <summary>
		/// Nastavuje automatický databind na GridView.		
		/// </summary>
		public bool AutoDataBind
		{
			get
			{
				return (bool)(ViewState["AutoDataBind"] ?? true);
			}
			set
			{
				ViewState["AutoDataBind"] = value;
			}
		}
		
		/// <summary>
		/// Zpøístupòuje pro ètení chránìnou vlastnost RequiresDataBinding.
		/// </summary>
		public new bool RequiresDataBinding
		{
			get
			{
				return base.RequiresDataBinding;
			}
			protected set
			{
				base.RequiresDataBinding = true;
			}
		}
		#endregion

		#region OnInit
		/// <summary>
		/// Inicializuje EnterpriseGridView.
		/// </summary>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			// Pokud dojde k vyvolání události, který nemá obsluhu, je vyvolána výjimka.
			// Protože ale nìkteré záležitosti øešíme sami, nastavíme "prázdné" obsluhy událostí
			// (nasmìrujeme je do èerné díry).			
			this.PageIndexChanging += new GridViewPageEventHandler(EnterpriseGridView_EventBlackHole);
			this.RowCancelingEdit += new GridViewCancelEditEventHandler(EnterpriseGridView_EventBlackHole);
			this.RowDeleting += new GridViewDeleteEventHandler(EnterpriseGridView_EventBlackHole);
			this.RowEditing += new GridViewEditEventHandler(EnterpriseGridView_EventBlackHole);
			this.RowUpdating += new GridViewUpdateEventHandler(EnterpriseGridView_EventBlackHole);			
		}

		private void EnterpriseGridView_EventBlackHole(object sender, EventArgs e)
		{
		}
		#endregion

		#region Hledání klíèe položky
		/// <summary>
		/// Nalezne hodnotu klíèe položky, ve kterém se nachází control.
		/// </summary>
		/// <param name="control">Control. Hledá se øádek, ve kterém se GridViewRow nalézá 
		/// a DataKey øádku.</param>
		/// <returns>Vrací hodnotu klíèe. Pokud není klíè nalezen
		/// (Control není v GridView), vrací null.
		/// </returns>
		public object GetKeyValue(Control control)
		{
			if (control == null || control.Parent == null)
				return null;
			if (control is GridViewRow && control.Parent.Parent == this)
				return DataKeys[((GridViewRow)control).RowIndex].Value;
			return GetKeyValue(control.NamingContainer);
		}

		/// <summary>
		/// Nalezne hodnotu klíèe položky na základì události.
		/// </summary>
		/// <param name="e">Událost, ke které v gridu došlo.</param>
		/// <returns>Vrací hodnotu klíèe daného øádku. Pokud není klíè nalezen vrací null.</returns>
		public object GetKeyValue(GridViewCommandEventArgs e)
		{
			if ((string)e.CommandArgument != String.Empty)
			{
				int index = int.Parse((string)e.CommandArgument);
				return DataKeys[index].Value;
			}

			if (e.CommandSource is Control)
			{
				return GetKeyValue((Control)e.CommandSource);
			}
			
			return null;
		}
		#endregion

		#region Hledání sloupcù
		/// <summary>
		/// Vyhledá sloupec podle id. Vyhledává jen sloupce implementující rozhraní IEnterpriseField.
		/// </summary>
		/// <param name="id">ID, podle kterého se sloupec vyhledává.</param>
		/// <returns>Nalezený sloupec nebo null, pokud není nalezen.</returns>
		public DataControlField FindColumn(string id)
		{
			foreach (DataControlField field in Columns)
				if ((field is IEnterpriseField) && ((IEnterpriseField)field).ID == id)
					return field;
			return null;
		}
		#endregion

		#region Stránkování
		/// <summary>
		/// Pokud není stránkování stornováno, zmìníme stránku na cílovou.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPageIndexChanging(GridViewPageEventArgs e)
		{
			base.OnPageIndexChanging(e);
			if (!e.Cancel)
			{
				PageIndex = e.NewPageIndex;
				RequiresDataBinding = true;
			}
		}

		/// <summary>
		/// Po setøídìní podle sloupce zajistí u vícestránkových gridù návrat na první stránku
		/// </summary>
		/// <param name="e"></param>
		protected override void OnSorted(EventArgs e)
		{
			base.OnSorted(e);
			PageIndex = 0;
		}

		#endregion

		//#region Zrušení DataSourceID (Naše øazení jej nepodporuje.)
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
		//#endregion

		#region OnPreRender
		/// <summary>
		/// Zajistíme DataBinding, pokud mají vlastnosti AutoDataBind a RequiresDataBinding hodnotu true.
		/// </summary>
		protected override void OnPreRender(EventArgs e)
		{
			if (AutoDataBind && RequiresDataBinding)
			{
				DataBind();
			}

			base.OnPreRender(e);
		}
		#endregion

		#region SetRequiresDataBinding
		/// <summary>
		/// Nastaví RequiresDataBinding na true.
		/// </summary>
		public void SetRequiresDatabinding()
		{
			RequiresDataBinding = true;
		}
		#endregion
	}
}
