using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections;
using System.Globalization;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// GridView implementující hightlighting, sorting a vıchozí obsluhu událostí editace, stránkování, ...
	/// </summary>
	public class GridViewExt : SortingGridView
	{
		#region GetInsertRowDataItem
		/// <summary>
		/// Metoda, která vrací data-item nového Insert øádku. Obvykle pøednastaveno default hodnotami.
		/// </summary>
		public GetInsertRowDataItemDelegate GetInsertRowDataItem
		{
			get
			{
				return _getInsertRowDataItem;
			}
			set
			{
				_getInsertRowDataItem = value;
			}
		}
		private GetInsertRowDataItemDelegate _getInsertRowDataItem;
		#endregion

		#region AllowInserting
		/// <summary>
		/// Indikuje, zda-li je povoleno pøidávání novıch poloek øádkem Insert.
		/// </summary>
		/// <remarks>
		/// Spolu s AllowInserting je potøeba nastavit delegáta <see cref="GetInsertRowDataItem"/>
		/// pro získávání vıchozích dat pro novou poloku. Dále lze nastavit pozici pomocí <see cref="InsertPosition"/>.
		/// </remarks>
		[Browsable(true), DefaultValue("true"), Category("Behavior")]
		public bool AllowInserting
		{
			get
			{
				object o = ViewState["_AllowInserting"];
				return o == null ? false : (bool)o;
			}
			set
			{
				ViewState["_AllowInserting"] = value;
			}
		}
		#endregion

		#region InsertPosition
		/// <summary>
		/// Indikuje, zda-li je povoleno pøidávání novıch poloek.
		/// </summary>
		public GridViewInsertRowPosition InsertRowPosition
		{
			get
			{
				object o = ViewState["_InsertRowPosition"];
				return o == null ? GridViewInsertRowPosition.Bottom : (GridViewInsertRowPosition)o;
			}
			set
			{
				ViewState["_InsertRowPosition"] = value;
			}
		}
		#endregion

		#region InsertRowDataSourceIndex
		/// <summary>
		/// Pozice øádku pro insert.
		/// </summary>
		/// <remarks>
		///  Nutno ukládat do viewstate kvùli zpìtné rekonstrukci øádkù bez data-bindingu. Jinak nechodí správnì eventy.
		/// </remarks>
		protected int InsertRowDataSourceIndex
		{
			get
			{
				object o = ViewState["_InsertRowDataSourceIndex"];
				return o == null ? -1 : (int)o;
			}
			set
			{
				ViewState["_InsertRowDataSourceIndex"] = value;
			}
		}
		#endregion

		#region AutoDataBind
		/// <summary>
		/// Nastavuje automatickı databind na GridView.		
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

		#region Events - RowInserting, RowInserted
		/// <summary>
		/// Událost, která se volá pøi vloení nového øádku (kliknutí na tlaèítko Insert).
		/// </summary>
		/// <remarks>
		/// Obsluha události má vyzvednout data a zaloit novı záznam.
		/// </remarks>
		[Category("Action")]
		public event GridViewInsertEventHandler RowInserting
		{
			add { base.Events.AddHandler(EventItemInserting, value); }
			remove { base.Events.RemoveHandler(EventItemInserting, value); }
		}

		/// <summary>
		/// Událost, která se volá po vloení nového øádku (po události RowInserting).
		/// </summary>
		[Category("Action")]
		public event GridViewInsertedEventHandler RowInserted
		{
			add { base.Events.AddHandler(EventItemInserted, value); }
			remove { base.Events.RemoveHandler(EventItemInserted, value); }
		}

		private static readonly object EventItemInserting = new object();
		private static readonly object EventItemInserted = new object();
		#endregion

		#region _insertRowIndex
		/// <summary>
		/// Skuteènı index InsertRow na stránce.
		/// </summary>
		private int insertRowIndex = -1;
		#endregion

		#region Constructor
		/// <summary>
		/// Vytvoøí instanci GridViewExt. Nastavuje defaultnì AutoGenerateColumns na false.
		/// </summary>
		public GridViewExt()
		{
			AutoGenerateColumns = false;
		}
		#endregion

		#region OnInit (EventBlackHole)
		/// <summary>
		/// Inicializuje EnterpriseGridView.
		/// </summary>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			// Pokud dojde k vyvolání události, kterı nemá obsluhu, je vyvolána vıjimka.
			// Protoe ale nìkteré záleitosti øešíme sami, nastavíme "prázdné" obsluhy událostí
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

		#region GetKeyValue - Hledání klíèe poloky
		/// <summary>
		/// Nalezne hodnotu klíèe poloky, ve kterém se nachází control.
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
		/// Nalezne hodnotu klíèe poloky na základì události.
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

			throw new ArgumentException("Událost neobsahuje data, z kterıch by se dal klíè urèit.");
		}

		/// <summary>
		/// Nalezne hodnotu klíèe poloky na základì indexu øádku v gridu.
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

		#region FindColumn - Hledání sloupcù
		/// <summary>
		/// Vyhledá sloupec podle id. Vyhledává jen sloupce implementující rozhraní IEnterpriseField.
		/// </summary>
		/// <param name="id">ID, podle kterého se sloupec vyhledává.</param>
		/// <returns>Nalezenı sloupec nebo null, pokud není nalezen.</returns>
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

		#region PerformDataBinding (override - Insert)
		/// <summary>
		/// Zajišuje data-binding dat na GridView.
		/// </summary>
		/// <param name="data">data</param>
		protected override void PerformDataBinding(IEnumerable data)
		{
			insertRowIndex = -1;

			// INSERTING
			if (AllowInserting)
			{
				if (GetInsertRowDataItem == null)
				{
					throw new InvalidOperationException("Pøi AllowInserting musíte nastavit GetInsertRowData");
				}
				ArrayList newData = new ArrayList();

				object insertRowDataItem = GetInsertRowDataItem();
				foreach (object item in data)
				{
					newData.Add(item);
				}
				if (AllowPaging)
				{
					int pageCount = (newData.Count + this.PageSize) - 1;
					if (pageCount < 0)
					{
						pageCount = 1;
					}
					pageCount = pageCount / this.PageSize;

					for (int i = 0; i < this.PageIndex; i++)
					{
						newData.Insert(0, insertRowDataItem);
					}
					for (int i = this.PageIndex + 1; i < pageCount; i++)   // pøepoèítat
					{
						newData.Add(insertRowDataItem);
					}
				}
				if (EditIndex < 0)
				{
					switch (InsertRowPosition)
					{
						case GridViewInsertRowPosition.Top:
							this.InsertRowDataSourceIndex = (this.PageSize * this.PageIndex);
							break;
						case GridViewInsertRowPosition.Bottom:
							if (AllowPaging)
							{
								this.InsertRowDataSourceIndex = Math.Min((((this.PageIndex + 1) * this.PageSize) - 1), newData.Count);
							}
							else
							{
								this.InsertRowDataSourceIndex = newData.Count;
							}
							break;
					}
					newData.Insert(InsertRowDataSourceIndex, insertRowDataItem);
				}
				data = newData;
			}
			base.PerformDataBinding(data);
		}
		#endregion

		#region CreateRow (override - Insert)
		protected override GridViewRow CreateRow(int rowIndex, int dataSourceIndex, DataControlRowType rowType, DataControlRowState rowState)
		{
			GridViewRow row = base.CreateRow(rowIndex, dataSourceIndex, rowType, rowState);

			// Øádek s novım objektem pøepínáme do stavu Insert, co zajistí zvolení EditItemTemplate a správné chování CommandFieldu.
			if ((rowType == DataControlRowType.DataRow)
				&& (AllowInserting)
				&& (dataSourceIndex == InsertRowDataSourceIndex))
			{
				insertRowIndex = rowIndex;
				row.RowState = DataControlRowState.Insert;
			}

			// abychom mìli na stránce vdy stejnı poèet øádek, tak u insertingu pøi editaci skrıváme poslední øádek
			if ((AllowInserting) && (insertRowIndex < 0) && (rowIndex == (this.PageSize - 1)))
			{
				row.Visible = false;
			}
			return row;
		}
		#endregion

		#region OnRowCommand (override -Insert)
		/// <summary>
		/// Metoda, která spouští událost RowCommand.
		/// </summary>
		/// <remarks>
		/// Implementace cachytává a obsluhuje pøíkaz Insert.
		/// </remarks>
		/// <param name="e">argumenty události</param>
		protected override void OnRowCommand(GridViewCommandEventArgs e)
		{
			base.OnRowCommand(e);

			bool causesValidation = false;
			string validationGroup = String.Empty;
			if (e != null)
			{
				IButtonControl control = e.CommandSource as IButtonControl;
				if (control != null)
				{
					causesValidation = control.CausesValidation;
					validationGroup = control.ValidationGroup;
				}
			}

			switch (e.CommandName)
			{
				case DataControlCommands.InsertCommandName:
					this.HandleInsert(Convert.ToInt32(e.CommandArgument, CultureInfo.InvariantCulture), causesValidation);
					break;
			}
		}
		#endregion

		#region HandleInsert
		/// <summary>
		/// Metoda, která øídí logiku pøíkazu Insert.
		/// </summary>
		/// <param name="rowIndex">index øádku, kde insert probíhá</param>
		/// <param name="causesValidation">pøíznak, zda-li má probíhat validace</param>
		protected virtual void HandleInsert(int rowIndex, bool causesValidation)
		{
			if ((!causesValidation || (this.Page == null)) || this.Page.IsValid)
			{
				GridViewInsertEventArgs argsInserting = new GridViewInsertEventArgs(rowIndex);
				this.OnRowInserting(argsInserting);
				if (!argsInserting.Cancel)
				{
					GridViewInsertedEventArgs argsInserted = new GridViewInsertedEventArgs();
					this.OnRowInserted(argsInserted);
					if (!argsInserted.KeepInEditMode)
					{
						this.EditIndex = -1;
						this.InsertRowDataSourceIndex = -1;
						base.RequiresDataBinding = true;
					}
				}
			}
		}
		#endregion

		#region OnRowEditing
		/// <summary>
		/// Spouští událost RowEditing.
		/// </summary>
		/// <remarks>Implementace zajišuje nastavení edit-øádku.</remarks>
		/// <param name="e">argumenty události</param>
		protected override void OnRowEditing(GridViewEditEventArgs e)
		{
			base.OnRowEditing(e);

			if (!e.Cancel)
			{
				this.EditIndex = e.NewEditIndex;
				if ((AllowInserting) && (this.InsertRowDataSourceIndex >= 0) && (this.insertRowIndex < e.NewEditIndex))
				{
					this.EditIndex = this.EditIndex - 1;
					SetRequiresDatabinding();
				}
				this.InsertRowDataSourceIndex = -1;
			}
		}
		#endregion

		#region OnRowUpdating
		/// <summary>
		/// Vıchozí chování RowUpdating - pokud není zvoleno e.Cancel, pak vypne editaci øádku.
		/// </summary>
		/// <param name="e">argumenty události</param>
		protected override void OnRowUpdating(GridViewUpdateEventArgs e)
		{
			base.OnRowUpdating(e);

			if (!e.Cancel)
			{
				this.EditIndex = -1;
				this.SetRequiresDatabinding();
			}
		}
		#endregion

		#region OnRowCancelingEdit
		/// <summary>
		/// Vıchozí chování RowUpdating - pokud není zvoleno e.Cancel, pak vypne editaci øádku.
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
		/// Vıchozí chování události OnPageIndexChanging Pokud není stránkování stornováno, zmìníme stránku na cílovou.
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

		#region OnRowDeleting
		/// <summary>
		/// Spouští událost RowDeleting.
		/// </summary>
		/// <remarks>
		/// Implementace vypíná editaci.
		/// </remarks>
		/// <param name="e">argumenty události</param>
		protected override void OnRowDeleting(GridViewDeleteEventArgs e)
		{
			base.OnRowDeleting(e);

			if (!e.Cancel)
			{
				this.EditIndex = -1;
				this.SetRequiresDatabinding();
			}
		}
		#endregion

		#region OnRowInserting, OnRowInserted
		/// <summary>
		/// Spouští událost RowInserting.
		/// </summary>
		/// <param name="e">argumenty události</param>
		protected virtual void OnRowInserting(GridViewInsertEventArgs e)
		{
			GridViewInsertEventHandler h = (GridViewInsertEventHandler)base.Events[EventItemInserting];
			if (h != null)
			{
				h(this, e);
			}
		}

		/// <summary>
		/// Spouští událost RowInserted.
		/// </summary>
		/// <param name="e">argumenty události</param>
		protected virtual void OnRowInserted(GridViewInsertedEventArgs e)
		{
			GridViewInsertedEventHandler h = (GridViewInsertedEventHandler)base.Events[EventItemInserted];
			if (h != null)
			{
				h(this, e);
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

	/// <summary>
	/// Reprezentuje metodu, která obsluhuje událost RowInserting controlu GridViewExt.
	/// </summary>
	/// <param name="sender">odesílatel události (GridView)</param>
	/// <param name="e">argumenty události</param>
	public delegate void GridViewInsertEventHandler(object sender, GridViewInsertEventArgs e);

	/// <summary>
	/// Reprezentuje metodu, která obsluhuje událost RowInserted controlu GridViewExt.
	/// </summary>
	/// <param name="sender">odesílatel události (GridView)</param>
	/// <param name="e">argumenty události</param>
	public delegate void GridViewInsertedEventHandler(object sender, GridViewInsertedEventArgs e);


	/// <summary>
	/// Delegát k metodì pro získávání data-item pro novı Insert øádek GridView.
	/// </summary>
	/// <returns></returns>
	public delegate object GetInsertRowDataItemDelegate();
}
