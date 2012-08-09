using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections;
using System.Globalization;
using Havit.Collections;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// GridView implementující hightlighting, sorting a vıchozí obsluhu událostí editace, stránkování, ...
	/// </summary>
	/// <remarks>
	/// Funkènost <b>Sorting</b> ukládá nastavení øazení dle uivatele a pøípadnì zajišuje automatické øazení pomocí GenericPropertyCompareru.<br/>
	/// Funkènost <b>Inserting</b> umoòuje pouití Insert-øádku pro pøidávání novıch poloek.<br/>
	/// </remarks>
	public class GridViewExt : HighlightingGridView, ICommandFieldStyle
	{
		#region GetInsertRowDataItem (delegate)
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
		/// Spolu s AllowInserting je potøeba nastavit delegáta <see cref="GetInsertRowDataItemDelegate" />
		/// pro získávání vıchozích dat pro novou poloku. Dále lze nastavit pozici pomocí <see cref="InsertRowPosition"/>.
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

		#region AutoSort
		/// <summary>
		/// Nastavuje, zda má pøi databindingu dojít k seøazení poloek podle nastavení.
		/// </summary>
		public bool AutoSort
		{
			get { return (bool)(ViewState["AutoSort"] ?? true); }
			set { ViewState["AutoSort"] = value; }
		}
		#endregion

		#region DefaultSortExpression
		/// <summary>
		/// Vıchozí øazení, které se pouije, pokud je povoleno automatické øazení nastavením vlastnosti AutoSort 
		/// a uivatel dosu ádné øazení nezvolil.
		/// </summary>
		public string DefaultSortExpression
		{
			get { return (string)(ViewState["DefaultSortExpression"] ?? String.Empty); }
			set { ViewState["DefaultSortExpression"] = value; }
		}
		#endregion

		#region SortExpressions
		/// <summary>
		/// Zajišuje práci se seznamem poloek, podle kterıch se øadí.
		/// </summary>
		public SortExpressions SortExpressions
		{
			get
			{
				return _sortExpressions;
			}
		}
		private SortExpressions _sortExpressions = new SortExpressions();
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

		#region CommandFieldStyle
		/// <summary>
		/// Skinovatelné vlastnosti, které se mají pøedat CommandFieldu.
		/// </summary>
		[Category("Styles")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual CommandFieldStyle CommandFieldStyle
		{
			get
			{
				if (this._commandFieldStyle == null)
				{
					this._commandFieldStyle = new CommandFieldStyle();
					if (base.IsTrackingViewState)
					{
						((IStateManager)this._commandFieldStyle).TrackViewState();
					}
				}
				return this._commandFieldStyle;
			}
		}
		private CommandFieldStyle _commandFieldStyle;
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
				base.RequiresDataBinding = value;
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

		#region Events - RowInserting, RowInserted, RowCustomizingCommandButton
		/// <summary>
		/// Událost, která se volá pøi vloení nového øádku (kliknutí na tlaèítko Insert).
		/// </summary>
		/// <remarks>
		/// Obsluha události má vyzvednout data a zaloit novı záznam.
		/// </remarks>
		[Category("Action")]
		public event GridViewInsertEventHandler RowInserting
		{
			add { base.Events.AddHandler(eventItemInserting, value); }
			remove { base.Events.RemoveHandler(eventItemInserting, value); }
		}

		/// <summary>
		/// Událost, která se volá po vloení nového øádku (po události RowInserting).
		/// </summary>
		[Category("Action")]
		public event GridViewInsertedEventHandler RowInserted
		{
			add { base.Events.AddHandler(eventItemInserted, value); }
			remove { base.Events.RemoveHandler(eventItemInserted, value); }
		}

		/// <summary>
		/// Událost, která se volá pøi customizaci command-buttonu øádku (implementováno v <see cref="GridViewCommandField"/>).
		/// </summary>
		[Category("Action")]
		public event GridViewRowCustomizingCommandButtonEventHandler RowCustomizingCommandButton
		{
			add
			{
				base.Events.AddHandler(eventRowCustomizingCommandButton, value);
			}
			remove
			{
				base.Events.RemoveHandler(eventRowCustomizingCommandButton, value);
			}
		}

		#region AllPagesShowing
		/// <summary>
		/// Událost, která se volá pøi obsluze kliknutí na tlaèítko "All Pages" (tlaèítko, vypínající stránkování). Dává monost zrušit akci.
		/// </summary>
		[Category("Action")]
		public event CancelEventHandler AllPagesShowing
		{
			add { base.Events.AddHandler(eventAllPagesShowing, value); }
			remove { base.Events.RemoveHandler(eventAllPagesShowing, value); }
		} 
		#endregion

		#region AllPagesShown
		/// <summary>
		/// Událost oznamující obslouení kliknutí na tlaèítko "All Pages" (tlaèítko, vypínající stránkování).
		/// </summary>
		[Category("Action")]
		public event EventHandler AllPagesShown
		{
			add { base.Events.AddHandler(eventAllPagesShown, value); }
			remove { base.Events.RemoveHandler(eventAllPagesShown, value); }
		} 
		#endregion

		private static readonly object eventItemInserting = new object();
		private static readonly object eventItemInserted = new object();
		private static readonly object eventRowCustomizingCommandButton = new object();
		private static readonly object eventAllPagesShowing = new object();
		private static readonly object eventAllPagesShown = new object();
		#endregion

		#region InsertIndex
		/// <summary>
		/// Index øádku (RowIndex) vkládaného prvku. Pokud nejsme v insert módu, je -1.
		/// </summary>
		public int InsertIndex
		{
			get
			{
				return insertRowIndex;
			}
		}
		#endregion

		#region insertRowIndex (private)
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

			GridViewRow row = GetRow(control);			
			return DataKeys[row.RowIndex];
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

		#region GetRow
		/// <summary>
		/// Najde GridViewRow obsahující danı control.
		/// Pokud není Control v GridView, vyvolá vıjimku.
		/// </summary>
		/// <param name="control">Control, na základì nìho se hledá GridViewRow.</param>
		/// <returns>Nalezenı GridViewRow.</returns>
		public GridViewRow GetRow(Control control)
		{
			if ((control == null) || (control.Parent == null))
			{
				throw new ArgumentException("Nepodaøilo dohledat pøíslušnı GridViewRow.", "control");
			}

			if ((control is GridViewRow) && (control.Parent.Parent == this))
			{
				return (GridViewRow)control;
			}

			return GetRow(control.NamingContainer);
		}
		#endregion

		#region FindColumn - Hledání sloupcù
		/// <summary>
		/// Vyhledá sloupec (field) podle id. Vyhledává jen sloupce implementující rozhraní IEnterpriseField.
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

		#region LoadViewState, SaveViewState - Naèítání a ukládání ViewState (sorting, CommandFieldStyle)
		/// <summary>
		/// Zajistí uloení ViewState. Je pøidáno uloení property Sorting.
		/// </summary>
		protected override object SaveViewState()
		{
			Triplet viewStateData = new Triplet();
			viewStateData.First = base.SaveViewState();
			viewStateData.Second = _sortExpressions;
			viewStateData.Third = (this._commandFieldStyle != null) ? ((IStateManager)this._commandFieldStyle).SaveViewState() : null;
			return viewStateData;
		}

		#region TrackViewState
		/// <summary>
		/// Spouští sledování ViewState.
		/// </summary>
		protected override void TrackViewState()
		{
			base.TrackViewState();
			((IStateManager)this.CommandFieldStyle).TrackViewState();
		}
		#endregion


		/// <summary>
		/// Zajistí naètení ViewState. Je pøidáno naètení property Sorting.
		/// </summary>
		protected override void LoadViewState(object savedState)
		{
			Triplet viewStateData = savedState as Triplet;
			Debug.Assert(viewStateData != null);

			base.LoadViewState(viewStateData.First);
			if (viewStateData.Second != null)
			{
				_sortExpressions = (SortExpressions)viewStateData.Second;
			}
			if (viewStateData.Third != null)
			{
				((IStateManager)this.CommandFieldStyle).LoadViewState(viewStateData.Third);
			}
		}
		#endregion

		#region PerformDataBinding (override - Insert)
		/// <summary>
		/// Zajišuje data-binding dat na GridView.
		/// </summary>
		/// <remarks>
		/// Pro reim Sorting zajišuje pøi AutoSort sesortìní.
		/// Pro reim Inserting zajišuje správu novıch øádek, popø. vkládání fake-øádek pøi stránkování.
		/// </remarks>
		/// <param name="data">data</param>
		protected override void PerformDataBinding(IEnumerable data)
		{
			// SORTING
			IEnumerable sortedData = data;
			if ((sortedData != null) && AutoSort)
			{
				if ((SortExpressions.SortItems.Count == 0) && !String.IsNullOrEmpty(DefaultSortExpression))
				{
					// sorting je nutné zmìnit na základì DefaultSortExpression,
					// kdybychom jej nezmìnili, tak první kliknutí shodné s DefaultSortExpression nic neudìlá
					_sortExpressions.AddSortExpression(DefaultSortExpression);
				}

				sortedData = SortHelper.PropertySort(sortedData, SortExpressions.SortItems);
			}

			// INSERTING
			ArrayList insertingData = null;
			if (sortedData != null)
			{
				insertingData = new ArrayList();
				foreach (object item in sortedData)
				{
					insertingData.Add(item);
				}
				insertRowIndex = -1;
				if (AllowInserting)
				{
					if (GetInsertRowDataItem == null)
					{
						throw new InvalidOperationException("Pøi AllowInserting musíte nastavit GetInsertRowData.");
					}

					object insertRowDataItem = GetInsertRowDataItem();
					if (AllowPaging)
					{
						int itemsCount = (insertingData.Count + this.PageSize) - 1;
						if (itemsCount < 0)
						{
							itemsCount = 1;
						}
						int pageCount = itemsCount / this.PageSize;

						// pokud by InsertingRow mìlo zbıt samotné na poslední stránce, pak snííme poèet stránek (mùe se stát po smazání poslední poloky)
						if ((this.PageIndex > 0) && ((this.PageSize - 1) * (this.PageIndex)) == insertingData.Count)
						{
							this.PageIndex--;
						}

						for (int i = 0; i < this.PageIndex; i++)
						{
							insertingData.Insert(0, insertRowDataItem);
						}
						for (int i = this.PageIndex + 1; i < pageCount; i++)
						{
							insertingData.Add(insertRowDataItem);
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
									this.InsertRowDataSourceIndex = Math.Min((((this.PageIndex + 1) * this.PageSize) - 1), insertingData.Count);
								}
								else
								{
									this.InsertRowDataSourceIndex = insertingData.Count;
								}
								break;
						}
						insertingData.Insert(InsertRowDataSourceIndex, insertRowDataItem);
					}
				}
			}

			base.PerformDataBinding(insertingData);

			if (insertingData != null)
			{
				RequiresDataBinding = false;
			}
		}
		#endregion

		#region CreateRow (override - Insert, øešení THEAD, apod.)
		/// <summary>
		/// CreateRow.
		/// </summary>
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
			if ((AllowInserting) && (insertRowIndex < 0) && (AllowPaging) && (rowIndex == (this.PageSize - 1)))
			{
				row.Visible = false;
			}

			switch (row.RowType)
			{
				case DataControlRowType.Header: row.TableSection = TableRowSection.TableHeader; break;				
			}

			return row;
		}
		#endregion

		#region OnRowCommand (override - Insert)
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
			GridViewInsertEventHandler h = (GridViewInsertEventHandler)base.Events[eventItemInserting];
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
			GridViewInsertedEventHandler h = (GridViewInsertedEventHandler)base.Events[eventItemInserted];
			if (h != null)
			{
				h(this, e);
			}
		}
		#endregion

		#region	OnSorting
		/// <summary>
		/// Pøi poadavku na øazení si zapamatujeme, jak chtìl uivatel øadit a nastavíme RequiresDataBinding na true.
		/// </summary>
		/// <param name="e">argumenty události</param>
		protected override void OnSorting(GridViewSortEventArgs e)
		{
			_sortExpressions.AddSortExpression(e.SortExpression);
			base.RequiresDataBinding = true;
		}
		#endregion

		#region OnSorted
		/// <summary>
		/// Po setøídìní podle sloupce zajistí u vícestránkovıch gridù návrat na první stránku
		/// </summary>
		/// <param name="e">argumenty události</param>
		protected override void OnSorted(EventArgs e)
		{
			base.OnSorted(e);
			PageIndex = 0;
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

		#region OnRowCustomizingCommandButton
		/// <summary>
		/// Spouští událost <see cref="RowCustomizingCommandButton"/>.
		/// </summary>
		/// <param name="e">argumenty události</param>
		protected internal virtual void OnRowCustomizingCommandButton(GridViewRowCustomizingCommandButtonEventArgs e)
		{
			GridViewRowCustomizingCommandButtonEventHandler h = (GridViewRowCustomizingCommandButtonEventHandler)base.Events[eventRowCustomizingCommandButton];
			if (h != null)
			{
				h(this, e);
			}
		}
		#endregion

		#region PagerSettingsShowAllPagesButton
		/// <summary>
		/// Povoluje/zakazuje zobrazení tlaèítka pro vypnutí stránkování.
		/// </summary>
		public bool PagerSettingsShowAllPagesButton
		{
			get
			{
				return (bool)(ViewState["PagerSettingsShowAllPagesButton"] ?? false);
			}
			set
			{
				ViewState["PagerSettingsShowAllPagesButton"] = value;
			}
		} 
		#endregion

		#region PagerSettingsAllPagesButtonText
		/// <summary>
		/// Text tlaèítka pro vypnutí stránkování. Pokud je nastaveno PagerSettingsAllPagesButtonImageUrl, má toto pøednost a tlaèítko bude obrázkové.
		/// </summary>
		public string PagerSettingsAllPagesButtonText
		{
			get
			{
				return (string)ViewState["PagerSettingsAllPagesButtonText"] ?? "*";
			}
			set
			{
				ViewState["PagerSettingsAllPagesButtonText"] = value;
			}
		} 
		#endregion

		#region PagerSettingsAllPagesButtonImageUrl
		/// <summary>
		/// ImageUrl tlaèítka pro vypnutí stránkování. Má pøednost pøed PagerSettingsAllPagesButtonText.
		/// </summary>
		public string PagerSettingsAllPagesButtonImageUrl
		{
			get
			{
				return (string)ViewState["PagerSettingsAllPagesButtonImageUrl"];
			}
			set
			{
				ViewState["PagerSettingsAllPagesButtonImageUrl"] = value;
			}
		} 
		#endregion

		#region InitializePager
		protected override void InitializePager(GridViewRow row, int columnSpan, PagedDataSource pagedDataSource)
		{
			base.InitializePager(row, columnSpan, pagedDataSource);
			// pokud je pouita vıchozí šablona a je povoleno tlaèítko pro vypnutí stránkování, pøidáme jej
			if ((this.PagerTemplate == null) && (this.PagerSettingsShowAllPagesButton))
			{
				// najdeme øádek tabulky, do které budeme pøidávat "All Pages Button".
				TableRow row2 = null;
				if ((row.Controls.Count == 1) && (row.Controls[0].Controls.Count == 1) && (row.Controls[0].Controls[0].Controls.Count == 1)) ;
				{
					row2 = row.Controls[0].Controls[0].Controls[0] as TableRow;
				}

				if (row2 != null)
				{

					Control allPagesControl;
					if (!String.IsNullOrEmpty(this.PagerSettingsAllPagesButtonImageUrl))
					{
						ImageButton allPagesImageButton = new ImageButton();
						allPagesImageButton.ID = "AllPagesImageButton";
						allPagesImageButton.ImageUrl = this.PagerSettingsAllPagesButtonImageUrl;
						allPagesImageButton.Click += new ImageClickEventHandler(AllPagesImageButton_Click);
						allPagesControl = allPagesImageButton;
					}
					else
					{
						LinkButton allPagesLinkButton = new LinkButton();
						allPagesLinkButton.ID = "AllPagesLinkButton";
						allPagesLinkButton.Text = this.PagerSettingsAllPagesButtonText;
						allPagesLinkButton.Click += new EventHandler(AllPagesLinkButton_Click);
						allPagesControl = allPagesLinkButton;
					}

					TableCell cell = new TableCell();
					row2.Cells.Add(cell);
					cell.Controls.Add(allPagesControl);
				}

			}
		} 
		#endregion

		#region AllPagesImageButton_Click, AllPagesLinkButton_Click, HandleAllPagesClicked
		private void AllPagesImageButton_Click(object sender, ImageClickEventArgs e)
		{
			HandleAllPagesClicked();
		}

		private void AllPagesLinkButton_Click(object sender, EventArgs e)
		{
			HandleAllPagesClicked();
		}

		private void HandleAllPagesClicked()
		{
			CancelEventArgs cancelEventArgs = new CancelEventArgs();
			OnAllPagesShowing(cancelEventArgs);
			if (!cancelEventArgs.Cancel)
			{
				this.AllowPaging = false;
				SetRequiresDatabinding();
				OnAllPagesShown(EventArgs.Empty);
			}
		}
		
		#endregion

		#region OnAllPagesShowing, OnAllPagesShown
		/// <summary>
		/// Obsluha událost pøi obsluze kliknutí na tlaèítko "All Pages" (tlaèítko, vypínající stránkování). Dává monost zrušit akci.
		/// </summary>
		protected virtual void OnAllPagesShowing(CancelEventArgs cancelEventArgs)
		{
			CancelEventHandler h = (CancelEventHandler)base.Events[eventAllPagesShowing];
			if (h != null)
			{
				h(this, cancelEventArgs);
			}
		}

		/// <summary>
		/// Obsluha události oznamující obslouení kliknutí na tlaèítko "All Pages" (tlaèítko, vypínající stránkování).
		/// </summary>
		protected virtual void OnAllPagesShown(EventArgs eventArgs)
		{
			EventHandler h = (EventHandler)base.Events[eventAllPagesShown];
			if (h != null)
			{
				h(this, eventArgs);
			}
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
	/// Reprezentuje metodu, která obsluhuje událost <see cref="GridViewExt.RowCustomizingCommandButton"/>.
	/// </summary>
	/// <param name="sender">odesílatel události (GridView)</param>
	/// <param name="e">argumenty události</param>
	public delegate void GridViewRowCustomizingCommandButtonEventHandler(object sender, GridViewRowCustomizingCommandButtonEventArgs e);


	/// <summary>
	/// Delegát k metodì pro získávání data-item pro novı Insert øádek GridView.
	/// </summary>
	/// <returns></returns>
	public delegate object GetInsertRowDataItemDelegate();
}
