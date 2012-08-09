using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Diagnostics;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// GridView implementující hightlighting, sorting a výchozí obsluhu událostí editace, stránkování, ...
	/// </summary>
	public class GridViewExt : SortingGridView
	{
		#region GridViewExt
		/// <summary>
		/// Vytvoøí instanci GridViewExt. Nastavuje defaultnì AutoGenerateColumns na false.
		/// </summary>
		public GridViewExt()
		{
			AutoGenerateColumns = false;
		}
		#endregion

		#region AutoDataBind
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
		#endregion

		#region RequiresDataBinding (new), SetRequiresDatabinding
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

		/// <summary>
		/// Nastaví RequiresDataBinding na true.
		/// </summary>
		public void SetRequiresDatabinding()
		{
			RequiresDataBinding = true;
		}
		#endregion

		#region OnInit (EventBlackHole)
		/// <summary>
		/// Inicializuje EnterpriseGridView.
		/// </summary>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			// Pokud dojde k vyvolání události, který nemá obsluhu, je vyvolána výjimka.
			// Protože ale nìkteré záležitosti øešíme sami, nastavíme "prázdné" obsluhy událostí
			// (nasmìrujeme je do èerné díry).			
			this.PageIndexChanging += new GridViewPageEventHandler(GridViewExt_EventBlackHole);
			this.RowCancelingEdit += new GridViewCancelEditEventHandler(GridViewExt_EventBlackHole);
			this.RowDeleting += new GridViewDeleteEventHandler(GridViewExt_EventBlackHole);
			this.RowEditing += new GridViewEditEventHandler(GridViewExt_EventBlackHole);
			this.RowUpdating += new GridViewUpdateEventHandler(GridViewExt_EventBlackHole);
		}

		private void GridViewExt_EventBlackHole(object sender, EventArgs e)
		{
			// NOOP
		}
		#endregion

		#region GetKeyValue - Hledání klíèe položky
		/// <summary>
		/// Nalezne hodnotu klíèe položky, ve kterém se nachází control.
		/// </summary>
		/// <param name="control">Control. Hledá se øádek, ve kterém se GridViewRow nalézá a DataKey øádku.</param>
		/// <returns>Vrací hodnotu klíèe.</returns>
		public DataKey GetRowKey(Control control)
		{
			if (DataKeyNames.Length == 0)
			{
				throw new InvalidOperationException("Není nastavena property DataKeyNames, nelze pracovat s klíèi.");
			}
			if ((control == null) || (control.Parent == null))
			{
				throw new ArgumentException("Z controlu se nepodaøilo klíè dohledat.", "control");
			}

			if ((control is GridViewRow) && (control.Parent.Parent == this))
			{
				return DataKeys[((GridViewRow)control).RowIndex];
			}
			return GetRowKey(control.NamingContainer);
		}

		/// <summary>
		/// Nalezne hodnotu klíèe položky na základì události.
		/// </summary>
		/// <param name="e">Událost, ke které v gridu došlo.</param>
		/// <returns>Vrací hodnotu klíèe daného øádku.</returns>
		public DataKey GetRowKey(GridViewCommandEventArgs e)
		{
			if (DataKeyNames.Length == 0)
			{
				throw new InvalidOperationException("Není nastavena property DataKeyNames, nelze pracovat s klíèi.");
			}

			if ((string)e.CommandArgument != String.Empty)
			{
				int rowIndex = int.Parse((string)e.CommandArgument);
				return GetRowKey(rowIndex);
			}

			if (e.CommandSource is Control)
			{
				return GetRowKey((Control)e.CommandSource);
			}

			throw new ArgumentException("Událost neobsahuje data, z kterých by se dal klíè urèit.");
		}

		/// <summary>
		/// Nalezne hodnotu klíèe položky na základì indexu øádku v gridu.
		/// </summary>
		/// <param name="rowIndex">index øádku</param>
		/// <returns>Vrací hodnotu klíèe daného øádku.</returns>
		public DataKey GetRowKey(int rowIndex)
		{
			if (DataKeyNames.Length == 0)
			{
				throw new InvalidOperationException("Není nastavena property DataKeyNames, nelze pracovat s klíèi.");
			}

			return this.DataKeys[rowIndex];
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
			{
				if ((field is IIdentifiableField) && ((IIdentifiableField)field).ID == id)
				{
					return field;
				}
			}
			return null;
		}
		#endregion

		#region OnRowEditing
		/// <summary>
		/// Výchozí chování RowEditing - nastaví editovaný øádek.
		/// </summary>
		/// <param name="e">argumenty události</param>
		protected override void OnRowEditing(GridViewEditEventArgs e)
		{
			this.EditIndex = e.NewEditIndex;
			base.OnRowEditing(e);
		}
		#endregion

		#region OnRowUpdating
		/// <summary>
		/// Výchozí chování RowUpdating - pokud není zvoleno e.Cancel, pak vypne editaci øádku.
		/// </summary>
		/// <param name="e">argumenty události</param>
		protected override void OnRowUpdating(GridViewUpdateEventArgs e)
		{
			base.OnRowUpdating(e);

			if (!e.Cancel)
			{
				this.EditIndex = -1;
			}
		}
		#endregion

		#region OnRowCancelingEdit
		/// <summary>
		/// Výchozí chování RowUpdating - pokud není zvoleno e.Cancel, pak vypne editaci øádku.
		/// </summary>
		/// <param name="e">argumenty události</param>
		protected override void OnRowCancelingEdit(GridViewCancelEditEventArgs e)
		{
			base.OnRowCancelingEdit(e);

			if (!e.Cancel)
			{
				this.EditIndex = -1;
			}
		}
		#endregion

		#region OnPageIndexChanging
		/// <summary>
		/// Výchozí chování události OnPageIndexChanging Pokud není stránkování stornováno, zmìníme stránku na cílovou.
		/// </summary>
		/// <param name="e">argumenty události</param>
		protected override void OnPageIndexChanging(GridViewPageEventArgs e)
		{
			base.OnPageIndexChanging(e);

			if (!e.Cancel)
			{
				PageIndex = e.NewPageIndex;
				RequiresDataBinding = true;
			}
		}
		#endregion

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
	}
}
