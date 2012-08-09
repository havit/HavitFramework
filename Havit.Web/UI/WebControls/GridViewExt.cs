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
	/// GridView implementující hightlighting, sorting a výchozí obsluhu událostí editace, stránkování, ...
	/// </summary>
	/// <remarks>
	/// Funkčnost <b>Sorting</b> ukládá nastavení řazení dle uživatele a případně zajišťuje automatické řazení pomocí GenericPropertyCompareru.<br/>
	/// Funkčnost <b>Inserting</b> umožňuje použití Insert-řádku pro přidávání nových položek.<br/>
	/// </remarks>
	public class GridViewExt : HighlightingGridView, ICommandFieldStyle
	{
		#region GetInsertRowDataItem (delegate)
		/// <summary>
		/// Metoda, která vrací data-item nového Insert řádku. Obvykle přednastaveno default hodnotami.
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
		/// Indikuje, zda-li je povoleno přidávání nových položek řádkem Insert.
		/// </summary>
		/// <remarks>
		/// Spolu s AllowInserting je potřeba nastavit delegáta <see cref="GetInsertRowDataItemDelegate" />
		/// pro získávání výchozích dat pro novou položku. Dále lze nastavit pozici pomocí <see cref="InsertRowPosition"/>.
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
		/// Indikuje, zda-li je povoleno přidávání nových položek.
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
		/// Pozice řádku pro insert.
		/// </summary>
		/// <remarks>
		///  Nutno ukládat do viewstate kvůli zpětné rekonstrukci řádků bez data-bindingu. Jinak nechodí správně eventy.
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
		/// Nastavuje, zda má při databindingu dojít k seřazení položek podle nastavení.
		/// </summary>
		public bool AutoSort
		{
			get { return (bool)(ViewState["AutoSort"] ?? true); }
			set { ViewState["AutoSort"] = value; }
		}
		#endregion

		#region DefaultSortExpression
		/// <summary>
		/// Výchozí řazení, které se použije, pokud je povoleno automatické řazení nastavením vlastnosti AutoSort 
		/// a uživatel dosuž žádné řazení nezvolil.
		/// </summary>
		public string DefaultSortExpression
		{
			get { return (string)(ViewState["DefaultSortExpression"] ?? String.Empty); }
			set { ViewState["DefaultSortExpression"] = value; }
		}
		#endregion

		#region SortExpressions
		/// <summary>
		/// Zajišťuje práci se seznamem položek, podle kterých se řadí.
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

		#region CommandFieldStyle
		/// <summary>
		/// Skinovatelné vlastnosti, které se mají předat CommandFieldu.
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
		/// Zpřístupňuje pro čtení chráněnou vlastnost RequiresDataBinding.
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
		/// Událost, která se volá při vložení nového řádku (kliknutí na tlačítko Insert).
		/// </summary>
		/// <remarks>
		/// Obsluha události má vyzvednout data a založit nový záznam.
		/// </remarks>
		[Category("Action")]
		public event GridViewInsertEventHandler RowInserting
		{
			add { base.Events.AddHandler(eventItemInserting, value); }
			remove { base.Events.RemoveHandler(eventItemInserting, value); }
		}

		/// <summary>
		/// Událost, která se volá po vložení nového řádku (po události RowInserting).
		/// </summary>
		[Category("Action")]
		public event GridViewInsertedEventHandler RowInserted
		{
			add { base.Events.AddHandler(eventItemInserted, value); }
			remove { base.Events.RemoveHandler(eventItemInserted, value); }
		}

		/// <summary>
		/// Událost, která se volá při customizaci command-buttonu řádku (implementováno v <see cref="GridViewCommandField"/>).
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
		/// Událost, která se volá při obsluze kliknutí na tlačítko "All Pages" (tlačítko, vypínající stránkování). Dává možnost zrušit akci.
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
		/// Událost oznamující obsloužení kliknutí na tlačítko "All Pages" (tlačítko, vypínající stránkování).
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
		/// Index řádku (RowIndex) vkládaného prvku. Pokud nejsme v insert módu, je -1.
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
		/// Skutečný index InsertRow na stránce.
		/// </summary>
		private int insertRowIndex = -1;
		#endregion

		#region Constructor
		/// <summary>
		/// Vytvoří instanci GridViewExt. Nastavuje defaultně AutoGenerateColumns na false.
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

			// Pokud dojde k vyvolání události, který nemá obsluhu, je vyvolána výjimka.
			// Protože ale některé záležitosti řešíme sami, nastavíme "prázdné" obsluhy událostí
			// (nasměrujeme je do černé díry).			
			this.PageIndexChanging += new GridViewPageEventHandler(GridViewExt_EventBlackHole);
			this.RowCancelingEdit += new GridViewCancelEditEventHandler(GridViewExt_EventBlackHole);
			this.RowDeleting += new GridViewDeleteEventHandler(GridViewExt_EventBlackHole);
			this.RowEditing += new GridViewEditEventHandler(GridViewExt_EventBlackHole);
			this.RowUpdating += new GridViewUpdateEventHandler(GridViewExt_EventBlackHole);
			this.Sorting += new GridViewSortEventHandler(GridViewExt_EventBlackHole);
		}

		private void GridViewExt_EventBlackHole(object sender, EventArgs e)
		{
			// NOOP
		}
		#endregion

		#region GetKeyValue - Hledání klíče položky
		/// <summary>
		/// Nalezne hodnotu klíče položky, ve kterém se nachází control.
		/// </summary>
		/// <param name="control">Control. Hledá se řádek, ve kterém se GridViewRow nalézá a DataKey řádku.</param>
		/// <returns>Vrací hodnotu klíče.</returns>
		public DataKey GetRowKey(Control control)
		{
			if (DataKeyNames.Length == 0)
			{
				throw new InvalidOperationException("Není nastavena property DataKeyNames, nelze pracovat s klíči.");
			}

			GridViewRow row = GetRow(control);			
			return DataKeys[row.RowIndex];
		}

		/// <summary>
		/// Nalezne hodnotu klíče položky na základě události.
		/// </summary>
		/// <param name="e">Událost, ke které v gridu došlo.</param>
		/// <returns>Vrací hodnotu klíče daného řádku.</returns>
		public DataKey GetRowKey(GridViewCommandEventArgs e)
		{
			if (DataKeyNames.Length == 0)
			{
				throw new InvalidOperationException("Není nastavena property DataKeyNames, nelze pracovat s klíči.");
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

			throw new ArgumentException("Událost neobsahuje data, z kterých by se dal klíč určit.");
		}

		/// <summary>
		/// Nalezne hodnotu klíče položky na základě indexu řádku v gridu.
		/// </summary>
		/// <param name="rowIndex">index řádku</param>
		/// <returns>Vrací hodnotu klíče daného řádku.</returns>
		public DataKey GetRowKey(int rowIndex)
		{
			if (DataKeyNames.Length == 0)
			{
				throw new InvalidOperationException("Není nastavena property DataKeyNames, nelze pracovat s klíči.");
			}

			return this.DataKeys[rowIndex];
		}
		#endregion

		#region GetRow
		/// <summary>
		/// Najde GridViewRow obsahující daný control.
		/// Pokud není Control v GridView, vyvolá výjimku.
		/// </summary>
		/// <param name="control">Control, na základě něhož se hledá GridViewRow.</param>
		/// <returns>Nalezený GridViewRow.</returns>
		public GridViewRow GetRow(Control control)
		{
			if ((control == null) || (control.Parent == null))
			{
				throw new ArgumentException("Nepodařilo dohledat příslušný GridViewRow.", "control");
			}

			if ((control is GridViewRow) && (control.Parent.Parent == this))
			{
				return (GridViewRow)control;
			}

			return GetRow(control.NamingContainer);
		}
		#endregion

		#region FindColumn - Hledání sloupců
		/// <summary>
		/// Vyhledá sloupec (field) podle id. Vyhledává jen sloupce implementující rozhraní IEnterpriseField.
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

		#region LoadViewState, SaveViewState - Načítání a ukládání ViewState (sorting, CommandFieldStyle)
		/// <summary>
		/// Zajistí uložení ViewState. Je přidáno uložení property Sorting.
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
		/// Zajistí načtení ViewState. Je přidáno načtení property Sorting.
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
		/// Zajišťuje data-binding dat na GridView.
		/// </summary>
		/// <remarks>
		/// Pro režim Sorting zajišťuje při AutoSort sesortění.
		/// Pro režim Inserting zajišťuje správu nových řádek, popř. vkládání fake-řádek při stránkování.
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
					// sorting je nutné změnit na základě DefaultSortExpression,
					// kdybychom jej nezměnili, tak první kliknutí shodné s DefaultSortExpression nic neudělá
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
						throw new InvalidOperationException("Při AllowInserting musíte nastavit GetInsertRowData.");
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

						// pokud by InsertingRow mělo zbýt samotné na poslední stránce, pak snížíme počet stránek (může se stát po smazání poslední položky)
						if ((this.PageIndex > 0) && ((this.PageSize - 1) * (this.PageIndex)) == insertingData.Count)
						{
							this.PageIndex--;
						}

						// pokud někdo binduje nová data a zůstala mu nastavená stránka někam mimo rozsah (zapomněl přestránkovat)
						// tak tomu trochu pomůžeme a tento zjevný problém vyřešíme už tady přestránkováním na 1. stránku
						// problém se projevoval třeba tak, že když jsem byl na 5. stránce a dal hledání (nová menší data)
						if (pageCount < (this.PageIndex + 1))
						{
							this.PageIndex = 0;
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

			string originalEmptyDataText = this.EmptyDataText;
			EmptyDataText = HttpUtilityExt.GetResourceString(EmptyDataText);
			base.PerformDataBinding(insertingData);
			this.EmptyDataText = originalEmptyDataText;

			if (insertingData != null)
			{
				RequiresDataBinding = false;
			}

			if (data != null)
			{
				if (HeaderRow != null)
				{
					HeaderRow.TableSection = TableRowSection.TableHeader;
				}

				if (FooterRow != null)
				{
					FooterRow.TableSection = TableRowSection.TableFooter;
				}

				if (TopPagerRow != null)
				{
					TopPagerRow.TableSection = TableRowSection.TableHeader;
				}

				if (BottomPagerRow != null)
				{
					BottomPagerRow.TableSection = TableRowSection.TableFooter;
				}
			}

		}
		#endregion

		#region CreateRow (override - Insert, řešení THEAD, apod.)
		/// <summary>
		/// CreateRow.
		/// </summary>
		protected override GridViewRow CreateRow(int rowIndex, int dataSourceIndex, DataControlRowType rowType, DataControlRowState rowState)
		{
			GridViewRow row = base.CreateRow(rowIndex, dataSourceIndex, rowType, rowState);

			// Řádek s novým objektem přepínáme do stavu Insert, což zajistí zvolení EditItemTemplate a správné chování CommandFieldu.
			if ((rowType == DataControlRowType.DataRow)
				&& (AllowInserting)
				&& (dataSourceIndex == InsertRowDataSourceIndex))
			{
				insertRowIndex = rowIndex;
				row.RowState = DataControlRowState.Insert;
			}

			// abychom měli na stránce vždy stejný počet řádek, tak u insertingu při editaci skrýváme poslední řádek
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
		/// Implementace cachytává a obsluhuje příkaz Insert.
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
		/// Metoda, která řídí logiku příkazu Insert.
		/// </summary>
		/// <param name="rowIndex">index řádku, kde insert probíhá</param>
		/// <param name="causesValidation">příznak, zda-li má probíhat validace</param>
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
		/// <remarks>Implementace zajišťuje nastavení edit-řádku.</remarks>
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
		/// Výchozí chování RowUpdating - pokud není zvoleno e.Cancel, pak vypne editaci řádku.
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
		/// Výchozí chování RowUpdating - pokud není zvoleno e.Cancel, pak vypne editaci řádku.
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
		/// Výchozí chování události OnPageIndexChanging Pokud není stránkování stornováno, změníme stránku na cílovou.
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
		/// Při požadavku na řazení si zapamatujeme, jak chtěl uživatel řadit a nastavíme RequiresDataBinding na true.
		/// </summary>
		/// <param name="e">argumenty události</param>
		protected override void OnSorting(GridViewSortEventArgs e)
		{
			base.OnSorting(e);

			if (!e.Cancel)
			{
				_sortExpressions.AddSortExpression(e.SortExpression);
				base.RequiresDataBinding = true;
			}
		}
		#endregion

		#region OnSorted
		/// <summary>
		/// Po setřídění podle sloupce zajistí u vícestránkových gridů návrat na první stránku
		/// </summary>
		/// <param name="e">argumenty události</param>
		protected override void OnSorted(EventArgs e)
		{
			base.OnSorted(e);
			PageIndex = 0;
			this.EditIndex = -1;
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
		/// Povoluje/zakazuje zobrazení tlačítka pro vypnutí stránkování.
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
		/// Text tlačítka pro vypnutí stránkování. Pokud je nastaveno PagerSettingsAllPagesButtonImageUrl, má toto přednost a tlačítko bude obrázkové.
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
		/// ImageUrl tlačítka pro vypnutí stránkování. Má přednost před PagerSettingsAllPagesButtonText.
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
			// pokud je použita výchozí šablona a je povoleno tlačítko pro vypnutí stránkování, přidáme jej
			if ((this.PagerTemplate == null) && (this.PagerSettingsShowAllPagesButton))
			{
				// najdeme řádek tabulky, do které budeme přidávat "All Pages Button".
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
						allPagesImageButton.CausesValidation = false;
						allPagesControl = allPagesImageButton;
					}
					else
					{
						LinkButton allPagesLinkButton = new LinkButton();
						allPagesLinkButton.ID = "AllPagesLinkButton";
						allPagesLinkButton.Text = this.PagerSettingsAllPagesButtonText;
						allPagesLinkButton.Click += new EventHandler(AllPagesLinkButton_Click);
						allPagesLinkButton.CausesValidation = false;
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
		/// Obsluha událost při obsluze kliknutí na tlačítko "All Pages" (tlačítko, vypínající stránkování). Dává možnost zrušit akci.
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
		/// Obsluha události oznamující obsloužení kliknutí na tlačítko "All Pages" (tlačítko, vypínající stránkování).
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
	/// Delegát k metodě pro získávání data-item pro nový Insert řádek GridView.
	/// </summary>
	/// <returns></returns>
	public delegate object GetInsertRowDataItemDelegate();
}
