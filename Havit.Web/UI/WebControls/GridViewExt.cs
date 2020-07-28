using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using Havit.Collections;
using Havit.Diagnostics.Contracts;
using Havit.Web.UI.ClientScripts;
using Havit.Web.UI.WebControls.ControlsValues;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// GridView implementující hightlighting, sorting a výchozí obsluhu událostí editace, stránkování, filtrování...
	/// </summary>
	/// <remarks>
	/// Funkčnost <b>Sorting</b> ukládá nastavení řazení dle uživatele a případně zajišťuje automatické řazení pomocí GenericPropertyCompareru.<br/>
	/// Funkčnost <b>Inserting</b> umožňuje použití Insert-řádku pro přidávání nových položek.<br/>
	/// </remarks>
	public class GridViewExt : HighlightingGridView, ICommandFieldStyle, IEditorExtensible
	{
		/// <summary>
		/// Slouží k evidenci controlů automatických filtrů.
		/// </summary>
		private List<IAutoFilterControl> autoFilterControls;

		/// <summary>
		/// Předává CommandFields z PerformDataBinding do PrepareControlHierarchy, které nemají být vidět.
		/// </summary>
		private List<CommandField> commandFieldsToHide;

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

		/// <summary>
		/// Indikuje, zdali je povoleno přidávání nových položek řádkem Insert.
		/// </summary>
		/// <remarks>
		/// Spolu s AllowInserting je potřeba nastavit delegáta <see cref="GetInsertRowDataItemDelegate" />
		/// pro získávání výchozích dat pro novou položku. Dále lze nastavit pozici pomocí <see cref="InsertRowPosition"/>.
		/// </remarks>
		[Browsable(true)]
		[DefaultValue("false")]
		[Category("Behavior")]
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

		/// <summary>
		/// Vrací true, pokud je použito vkládání nového záznamu inline editací.
		/// To není povoleno, pokud je použit externí editor.
		/// </summary>
		protected virtual bool IsInsertingByInsertRow
		{
			get
			{
				return AllowInserting && (EditorExtender == null);
			}
		}

		/// <summary>
		/// Indikuje, zdali je povoleno přidávání nových položek.
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

		/// <summary>
		/// Nastavuje, zda má při databindingu dojít k seřazení položek podle nastavení.
		/// </summary>
		public bool AutoSort
		{
			get { return (bool)(ViewState["AutoSort"] ?? true); }
			set { ViewState["AutoSort"] = value; }
		}

		/// <summary>
		/// Výchozí řazení, které se použije, pokud je povoleno automatické řazení nastavením vlastnosti AutoSort 
		/// a uživatel dosuž žádné řazení nezvolil.
		/// </summary>
		public string DefaultSortExpression
		{
			get { return (string)(ViewState["DefaultSortExpression"] ?? String.Empty); }
			set { ViewState["DefaultSortExpression"] = value; }
		}

		/// <summary>
		/// Zajišťuje práci se seznamem položek, podle kterých se řadí.
		/// </summary>
		public SortExpressions SortExpressions
		{
			get
			{
				if (_sortExpressions == null)
				{
					_sortExpressions = new SortExpressions();
					if (!String.IsNullOrEmpty(DefaultSortExpression))
					{
						_sortExpressions.AddSortExpression(DefaultSortExpression);
					}
				}
				return _sortExpressions;
			}

		}
		private SortExpressions _sortExpressions;

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
		/// Index řádku (RowIndex) vkládaného prvku. Pokud nejsme v insert módu, je -1.
		/// </summary>
		public int InsertIndex
		{
			get
			{
				return insertRowIndex;
			}
		}
		private int insertRowIndex = -1;

		/// <summary>
		/// Externí editor připojený k GridView.
		/// Pro připojení slouží metoda RegisterExtender.
		/// </summary>
		public IEditorExtender EditorExtender { get; private set; }

		/// <summary>
		/// Index řádku editovaného externím editorem.
		/// Pro insert nabývá hodnoty -1.
		/// Pokud není režim editace externím editorem, vyhazuje výjimku.
		/// </summary>
		public int EditorExtenderEditIndex
		{
			get
			{
				int? editorExtenderEditIndex = EditorExtenderEditIndexInternal;
				if (editorExtenderEditIndex == null)
				{
					throw new InvalidOperationException("EditorExtenderEditIndex not set.");
				}
				return editorExtenderEditIndex.Value;
			}
			set
			{
				EditorExtenderEditIndexInternal = value;
			}

		}

		/// <summary>
		/// Index řádku editovaného externím editorem.
		/// Pro insert nabývá hodnoty -1.
		/// Pokud není režim editace externím editorem, vyhazuje výjimku.
		/// </summary>
		protected int? EditorExtenderEditIndexInternal
		{
			get
			{
				return (int?)ViewState["EditorExtenderEditIndex"];				
			}
			set
			{
				if (IsTrackingViewState)
				{
					if (EditorExtenderEditIndexInternal != value)
					{
						_editorExtenderEditIndexInternalChanged = true;
					}
				}
				ViewState["EditorExtenderEditIndex"] = value;
			}
		}
		private bool _editorExtenderEditIndexInternalChanged = false;

		/// <summary>
		/// Vrací režim editace záznamu v externím editoru.
		/// </summary>
		public EditorExtenderMode EditorExtenderMode
		{
			get
			{
				return (this.EditorExtenderEditIndex == -1) ? WebControls.EditorExtenderMode.Insert : WebControls.EditorExtenderMode.Edit;
			}
		}

		/// <summary>
		/// Css třída pro aktuálně editovaný záznam v externím editoru.
		/// </summary>
		public string EditorExtenderEditCssClass
		{
			get
			{
				return (string)(ViewState["EditorExtenderEditCssClass"] ?? String.Empty);
			}
			set
			{
				ViewState["EditorExtenderEditCssClass"] = value;
			}
		}

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
					if (IsTrackingViewState)
					{
						((IStateManager)this._commandFieldStyle).TrackViewState();
					}
				}
				return this._commandFieldStyle;
			}
		}
		private CommandFieldStyle _commandFieldStyle;

		/// <summary>
		/// Pager render mode.
		/// V případě, že je použita (nastavena) vlastnost PagerTemplate, hodnota se ignoruje a použije se standardní renderování.
		/// </summary>
		public PagerRenderMode PagerRenderMode
		{
			get
			{
				return (PagerRenderMode)(ViewState["PagerRenderMode"] ?? PagerRenderMode.Standard);
			}
			set
			{
				ViewState["PagerRenderMode"] = value;
			}
		}

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

		/// <summary>
		/// Událost, která se volá při obsluze kliknutí na tlačítko "All Pages" (tlačítko, vypínající stránkování). Dává možnost zrušit akci.
		/// </summary>
		[Category("Action")]
		public event CancelEventHandler AllPagesShowing
		{
			add { Events.AddHandler(eventAllPagesShowing, value); }
			remove { Events.RemoveHandler(eventAllPagesShowing, value); }
		}

		/// <summary>
		/// Indikuje, zda je zobrazen řádek filtru.
		/// </summary>
		public bool ShowFilter
		{
			get
			{
				return (bool)(ViewState["ShowFilter"] ?? false);
			}
			set
			{
				ViewState["ShowFilter"] = value;
			}
		}

		/// <summary>
		/// Filtrovací řádek.
		/// </summary>
		public GridViewRow FilterRow
		{
			get
			{
				return _filterRow;
			}
		}
		private GridViewRow _filterRow;

		/// <summary>
		/// Styl řádku filtru.
		/// </summary>
		[Category("Styles")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual TableItemStyle FilterStyle
		{
			get
			{
				if (this._filterStyle == null)
				{
					this._filterStyle = new TableItemStyle();
					if (IsTrackingViewState)
					{
						((IStateManager)this._filterStyle).TrackViewState();
					}
				}
				return this._filterStyle;
			}
		}
		private TableItemStyle _filterStyle;

		/// <summary>
		/// Gets or sets a value that indicates whether the heading of a column in the System.Web.UI.WebControls.GridView control is visible when the column has no data.
		/// </summary>
		public override bool ShowHeaderWhenEmpty
		{
			get
			{
				// Pro účely filtrování potřebujeme zajistit, aby se tabulka vyrendrovala i když neobsahuje po filtrování žádná data. No nejlépe zajistíme tím, že tato vlastnost vrátí true.
				return overrideShowHeaderWhenEmptyToTrue || base.ShowHeaderWhenEmpty;
			}
			set
			{
				base.ShowHeaderWhenEmpty = value;
			}
		}
		private bool overrideShowHeaderWhenEmptyToTrue = false;

		/// <summary>
		/// Pomocná vlastnost pro perzistenci stavu bootstrap pageru
		/// </summary>
		private int FirstDisplayedPageIndex
		{
			get
			{
				return (int)(this.ViewState["FirstDisplayedPageIndexExt"] ?? -1);
			}
			set
			{
				this.ViewState["FirstDisplayedPageIndexExt"] = value;
			}
		}

		/// <summary>
		/// Pokud je nastaveno, generuje se okolo tabulky gridu další DIV, který má nastavenu tuto css třídu.
		/// By default je již tabulka jedním DIVem obalena, toto přidává druhý DIV.
		/// </summary>
		public string ContainerCssClass
		{
			get
			{
				return (string)(ViewState["ContainerCssClass"] ?? String.Empty);
			}
			set
			{
				ViewState["ContainerCssClass"] = value;
			}
		}

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
		/// Událost, která se volá při vložení nového řádku (kliknutí na tlačítko Insert).
		/// </summary>
		/// <remarks>
		/// Obsluha události má vyzvednout data a založit nový záznam.
		/// </remarks>
		[Category("Action")]
		public event GridViewInsertEventHandler RowInserting
		{
			add { Events.AddHandler(eventItemInserting, value); }
			remove { Events.RemoveHandler(eventItemInserting, value); }
		}

		/// <summary>
		/// Událost, která se volá po vložení nového řádku (po události RowInserting).
		/// </summary>
		[Category("Action")]
		public event GridViewInsertedEventHandler RowInserted
		{
			add { Events.AddHandler(eventItemInserted, value); }
			remove { Events.RemoveHandler(eventItemInserted, value); }
		}

		/// <summary>
		/// Událost, která se volá při začátku události pro založení nového záznamu.
		/// </summary>
		/// <remarks>
		/// Obsluha události má zajistit zobrazení UI pro vložení nového záznamu.
		/// Vzhledek k jazykové nemožnosti použití konvence Editing/Edited, Inserting/Inserted pro "New" je zde dvojice NewProcessing/NewProcessed.
		/// </remarks>
		[Category("Action")]
		public event GridViewNewProcessingEventHandler NewProcessing
		{
			add { Events.AddHandler(eventNewProcessing, value); }
			remove { Events.RemoveHandler(eventNewProcessing, value); }
		}

		/// <summary>
		/// Událost, která se volá po události pro založení nového záznamu (po události NewProcessing).
		/// </summary>
		/// <remarks>
		/// Vzhledek k jazykové nemožnosti použití konvence Editing/Edited, Inserting/Inserted pro "New" je zde dvojice NewProcessing/NewProcessed.
		/// </remarks>
		[Category("Action")]
		public event GridViewNewProcessingEventHandler NewProcessed
		{
			add { Events.AddHandler(eventNewProcessed, value); }
			remove { Events.RemoveHandler(eventNewProcessed, value); }
		}

		/// <summary>
		/// Událost, která se volá při customizaci command-buttonu řádku (implementováno v <see cref="GridViewCommandField"/>).
		/// </summary>
		[Category("Action")]
		public event GridViewRowCustomizingCommandButtonEventHandler RowCustomizingCommandButton
		{
			add
			{
				Events.AddHandler(eventRowCustomizingCommandButton, value);
			}
			remove
			{
				Events.RemoveHandler(eventRowCustomizingCommandButton, value);
			}
		}

		/// <summary>
		/// Událost je vyvolána při založení filtrovacího řádku, před vložením do stromu controlů.
		/// </summary>
		public event GridViewRowEventHandler FilterRowCreated
		{
			add
			{
				Events.AddHandler(eventFilterRowCreated, value);
			}
			remove
			{
				Events.RemoveHandler(eventFilterRowCreated, value);
			}
		}

		/// <summary>
		/// Událost je vložena po provedení DataBindu na filtrovacím řádku.
		/// </summary>
		public event GridViewRowEventHandler FilterRowDataBound
		{
			add
			{
				Events.AddHandler(eventFilterRowDataBound, value);
			}
			remove
			{
				Events.RemoveHandler(eventFilterRowDataBound, value);
			}
		}

		/// <summary>
		/// Událost je zaválána v případě zapnutého filtrování dat a dává k dispozici další bod pro možnost omezení množiny zobrazených dat. V případě změny zobrazených dat se očekává změna vlastnosti Data argumentů události.
		/// </summary>
		public event GridViewDataFilteringEventHandler GridViewDataFiltering
		{
			add
			{
				Events.AddHandler(eventGridViewDataFiltering, value);
			}
			remove
			{
				Events.RemoveHandler(eventGridViewDataFiltering, value);
			}
		}

		/// <summary>
		/// Událost oznamující obsloužení kliknutí na tlačítko "All Pages" (tlačítko, vypínající stránkování).
		/// </summary>
		[Category("Action")]
		public event EventHandler AllPagesShown
		{
			add { Events.AddHandler(eventAllPagesShown, value); }
			remove { Events.RemoveHandler(eventAllPagesShown, value); }
		}

		private static readonly object eventItemInserting = new object();
		private static readonly object eventItemInserted = new object();
		private static readonly object eventNewProcessing = new object();
		private static readonly object eventNewProcessed = new object();
		private static readonly object eventRowCustomizingCommandButton = new object();
		private static readonly object eventAllPagesShowing = new object();
		private static readonly object eventAllPagesShown = new object();
		private static readonly object eventFilterRowCreated = new object();
		private static readonly object eventFilterRowDataBound = new object();
		private static readonly object eventGridViewDataFiltering = new object();

		/// <summary>
		/// Indikuje, zda je povoleno rozkliknutí celého řádku.
		/// Výchozí hodnota je false.
		/// 
		/// Ve výchozím stavu není rozkliknutelný řádek, který je editován či vkládán a pokud RowClickCommandName je "Edit". Brání se tak možnosti znovu kliknout na editaci aktuálně editovaného řádku, což by molho vést je ztrátě již vepsaných údajů při lehkém ukliknutí.
		/// Ve výchozím stavu dále není rozkliknutelný řádek, který je vybraný a pokud RowClickCommandName je "Select". Brání se tak možnosti znovu kliknout na výběr již vybraného řádku.
		/// Výchozí chování je možné změnit nastavením hodnoty Enabled na true v obsluze události OnRowCustomizingCommandButton pro daný RowClickCommandName.
		/// </summary>
		/// <remarks>
		/// Možnost rozkliknutí řádku se řídí pravidly:
		/// <ol>
		/// <li>AllowRowClick musí být true.</li>
		/// <li>Grid musí být povolen.</li>
		/// <li>Obsluha události OnRowCustomizingCommandButton nesmí vrátit ve vlastnostech Enabled ani Visible hodnotu false.</li>
		/// </ol>
		/// </remarks>
		public bool AllowRowClick
		{
			get
			{
				return (bool)(ViewState["AllowRowClick"] ?? false);
			}
			set
			{
				ViewState["AllowRowClick"] = value;
			}
		}

		/// <summary>
		/// CommandName, který se provede při rozkliknutí řádku.
		/// Výchozí hodnota je Edit.
		/// </summary>
		public string RowClickCommandName
		{
			get
			{
				return (string)(ViewState["RowClickCommandName"] ?? CommandNames.Edit);
			}
			set
			{
				ViewState["RowClickCommandName"] = value;
			}
		}

		/// <summary>
		/// CssClass pro řádky, na kterých je povoleno rozkliknutí celého řádku (pomocí AllowRowClick).
		/// Výchozí hodnotou je prázdný text.
		/// </summary>
		public string RowClickCssClass
		{
			get
			{
				return (string)(ViewState["RowClickCssClass"] ?? String.Empty);
			}
			set
			{
				ViewState["RowClickCssClass"] = value;
			}
		}

		/// <summary>
		/// Child Table.
		/// </summary>
		protected Table ChildTable { get; set; }

		/// <summary>
		/// Vytvoří instanci GridViewExt. Nastavuje defaultně AutoGenerateColumns na false.
		/// </summary>
		public GridViewExt()
		{
			AutoGenerateColumns = false;
		}

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
			this.Page.PreRenderComplete += new EventHandler(Page_PreRenderComplete);
		}
		private void GridViewExt_EventBlackHole(object sender, EventArgs e)
		{
			// NOOP
		}

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

		/// <summary>
		/// Zajistí uložení ViewState. Je přidáno uložení property Sorting.
		/// </summary>
		protected override object SaveViewState()
		{
			return new object[]
			{
				base.SaveViewState(),
				_sortExpressions,
				(this._commandFieldStyle != null) ? ((IStateManager)this._commandFieldStyle).SaveViewState() : null,
				(this._filterStyle != null) ? ((IStateManager)this._filterStyle).SaveViewState() : null
			};
		}

		/// <summary>
		/// Spouští sledování ViewState.
		/// </summary>
		protected override void TrackViewState()
		{
			base.TrackViewState();
			((IStateManager)this.CommandFieldStyle).TrackViewState();
			((IStateManager)this.FilterStyle).TrackViewState();
		}

		/// <summary>
		/// Zajistí načtení ViewState. Je přidáno načtení property Sorting.
		/// </summary>
		protected override void LoadViewState(object savedState)
		{
			object[] viewStateData = savedState as object[];
			Debug.Assert(viewStateData != null);

			base.LoadViewState(viewStateData[0]);
			_sortExpressions = (SortExpressions)viewStateData[1];
			if (viewStateData[2] != null)
			{
				((IStateManager)this.CommandFieldStyle).LoadViewState(viewStateData[2]);
			}
			if (viewStateData[3] != null)
			{
				((IStateManager)this.FilterStyle).LoadViewState(viewStateData[3]);
			}
		}

		/// <summary>
		/// Creates the control hierarchy used to render the <see cref="T:System.Web.UI.WebControls.GridView"/> control using the specified data source.
		/// </summary>
		/// <returns>
		/// The number of rows created.
		/// </returns>
		/// <param name="dataSource">An <see cref="T:System.Collections.IEnumerable"/> that contains the data source for the <see cref="T:System.Web.UI.WebControls.GridView"/> control. </param><param name="dataBinding">true to indicate that the child controls are bound to data; otherwise, false. </param><exception cref="T:System.Web.HttpException"><paramref name="dataSource"/> returns a null <see cref="T:System.Web.UI.DataSourceView"/>.-or-<paramref name="dataSource"/> does not implement the <see cref="T:System.Collections.ICollection"/> interface and cannot return a <see cref="P:System.Web.UI.DataSourceSelectArguments.TotalRowCount"/>. -or-<see cref="P:System.Web.UI.WebControls.GridView.AllowPaging"/> is true and <paramref name="dataSource"/> does not implement the <see cref="T:System.Collections.ICollection"/> interface and cannot perform data source paging.-or-<paramref name="dataSource"/> does not implement the <see cref="T:System.Collections.ICollection"/> interface and <paramref name="dataBinding"/> is set to false.</exception>
		protected override int CreateChildControls(IEnumerable dataSource, bool dataBinding)
		{
			autoFilterControls = new List<IAutoFilterControl>();
			// pokud zobrazujeme filtr, pak chceme, aby se zobrazil grid i bez dat
			// stejně tak potřebujeme grid, pokud vkládáme nový záznam pomocí editor extenderu
			overrideShowHeaderWhenEmptyToTrue = ShowFilter || (AllowInserting && (EditorExtender != null)); 

			string originalEmptyDataText = this.EmptyDataText;
			EmptyDataText = HttpUtilityExt.GetResourceString(EmptyDataText);
			int result = base.CreateChildControls(dataSource, dataBinding);
			EmptyDataText = originalEmptyDataText;

			overrideShowHeaderWhenEmptyToTrue = false;
			
			// zajistíme přidání filtrovacího řádku
			if (ShowFilter)	
			{				
				this.CreateFilterChildControls(dataBinding);
			}
			
			return result;
		}

		/// <summary>
		/// Vytvoří ChildTable.
		/// </summary>
		protected override Table CreateChildTable()
		{
			ChildTable = base.CreateChildTable();
			return ChildTable;
		}

		/// <summary>
		/// Přidá filtrovací řádek do gridu.
		/// </summary>
		private void CreateFilterChildControls(bool dataBinding)
		{			
			if (this._fields != null)
			{
				Table table = ChildTable;

				// V prvním databindu se provádí fake call PerformDataBinding (viz volání base v metode PerformDataBinding).
				// V tom důsledku je metoda CreateChildControls/CreateFilterChildControls volána dvakrát.
				// Abychom omezili dvojí tvorbě filtru řádku a hlavně dvojím událostem FilterRowCreater a FilterRowDataBound, zapamatujeme si hodnotu z prvního (fake) volání metody
				// a v dalším (pravém) volání tuto hodnotu použijeme.

				if ((this._filterRowCreatedInPerformDataBindingFakeCall != null) && (!_performDataBindingDataInFakeCall))
				{
					_filterRow = this._filterRowCreatedInPerformDataBindingFakeCall;
					table.Rows.AddAt((this.HeaderRow != null) ? table.Rows.GetRowIndex(this.HeaderRow) + 1 : 0, _filterRow);
					return;
				}
				
				_filterRow = new GridViewRow(-1, -1, DataControlRowType.Header, DataControlRowState.Normal);
				_filterRow.ID = "FilterRow";
				
				GridViewRowEventArgs e = new GridViewRowEventArgs(_filterRow);

				this.InitializeFilterRow(_filterRow, this._fields);

				//_filterRow.MergeStyle(FilterStyle);

				OnFilterRowCreated(e);

				table.Rows.AddAt((this.HeaderRow != null) ? table.Rows.GetRowIndex(this.HeaderRow) + 1 : 0, _filterRow);
				
				if (dataBinding)
				{
					_filterRow.DataItem = this._performDataBindingData;
					_filterRow.DataBind();
					if (_previousFilterRowData != null)
					{
						ControlsValuesPersister.ApplyValues(_previousFilterRowData, _filterRow);
					}
					OnFilterRowDataBound(e);
				}

				this._filterRowCreatedInPerformDataBindingFakeCall = _performDataBindingDataInFakeCall ? _filterRow : null;
			}
			else
			{
				_filterRow = null;
			}
		}
		private GridViewRow _filterRowCreatedInPerformDataBindingFakeCall;

		/// <summary>
		/// Rozšiřuje RaisePostBackEvent o obsluhu událostí RowClick.
		/// </summary>
		protected override void RaisePostBackEvent(string eventArgument)
		{
			base.RaisePostBackEvent(eventArgument);

			if ((eventArgument != null) && (eventArgument.StartsWith("RowClick$")))
			{
				int rowIndex = int.Parse(eventArgument.Substring(9 /* "RowClick$".IndexOf('$') + 1 */));
				HandleRowClick(rowIndex);
			}
		}

		/// <summary>
		/// Inicializuje řádek gridu danými fieldy. Není použito pro filtrovací řádek, ten řeší samostatná metoda InitializeFilterRow
		/// (tato metoda používá row.RowType a row.RowState, pro filtr nemáme hodnoty enumu, které by byly použitelné).
		/// </summary>
		protected override void InitializeRow(GridViewRow row, DataControlField[] fields)
		{
			// Pro potřeby filtrovacího řádku si zde pouze zapamatováváme, s jakými fieldy je metoda volána (neřešíme, že je volána opakovaně - je vždy volána se stejnými parametry).
			base.InitializeRow(row, fields);
			_fields = fields;
		}
		private DataControlField[] _fields;

		/// <summary>
		/// Inicializuje filtrovací řádek gridu.
		/// </summary>
		protected virtual void InitializeFilterRow(GridViewRow row, DataControlField[] fields)
		{
			for (int i = 0; i < fields.Length; i++)
			{
				DataControlFieldCell cell = new DataControlFieldCell(fields[i]);

				IFilterField filterField = fields[i] as IFilterField;
				if (filterField != null)
				{
					filterField.InitializeFilterCell(cell);
				}
				row.Cells.Add(cell);
			}
		}

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
			this._performDataBindingData = data; // uchováváme pro databind filtru

			// FILTERING
			IEnumerable filteredData = data;

			// Pokud již máme filter row, vytáhneme si jeho hodnoty.
			// Musíme zde, před voláním CreateChildControls, protože CreateChildControls vyhodí řádek ze stromu controlů a všem controlům v řádku se kompletně změní IDčka.
			// Kdybychom vytáhli hodnoty až v CreateFilterChildControls, tak je vytáhneme s jinými IDčka a do nového řádku se nám je nepodaří dostat, protože nedojde ke spárování přes IDčka.
			_previousFilterRowData = (_filterRow != null) ? ControlsValuesPersister.RetrieveValues(_filterRow) : null;

			// Pokud používáme filtr, tak chceme provést filtrování dat a to včetně výchozích hodnot.
			// Jenže filter row (i s výchozími daty) je vytvořen až po nabindování dat v CreateChildControls.
			// Pokud chceme výchozí hodnoty získat i pro první DataBind, použijeme obezličku takovou,
			// že si vynutíme vytvoření filtru tak, že gridview nanečisto nabindujeme prázdnou kolekcí (vytvoří se header, footer a filterRow)
			if ((filteredData != null) && ShowFilter && (_filterRow == null))
			{
				_performDataBindingDataInFakeCall = true;
				//base.PerformDataBinding(new object[] { });
				_performDataBindingDataInFakeCall = false;
			}

			if ((filteredData != null) && ShowFilter && (_filterRow != null))
			{
				filteredData = this.PerformAutoFiltering(filteredData);
				GridViewDataFilteringEventArgs e = new GridViewDataFilteringEventArgs(filteredData, _filterRow);
				this.OnGridViewDataFiltering(e);
				filteredData = e.Data;
			}

			// SORTING
			IEnumerable sortedData = filteredData;
			if ((sortedData != null) && AutoSort)
			{
				//if ((SortExpressions.SortItems.Count == 0) && !String.IsNullOrEmpty(DefaultSortExpression))
				//{
				//	// sorting je nutné změnit na základě DefaultSortExpression,
				//	// kdybychom jej nezměnili, tak první kliknutí shodné s DefaultSortExpression nic neudělá
				//	_sortExpressions.AddSortExpression(DefaultSortExpression);
				//}

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
				if (IsInsertingByInsertRow)
				{
					if (GetInsertRowDataItem == null)
					{
						throw new InvalidOperationException("Při AllowInserting musíte nastavit GetInsertRowDataItem.");
					}

					object insertRowDataItem = GetInsertRowDataItem();
					if (AllowPaging)
					{
						int pageCount = (insertingData.Count - 1) / (this.PageSize - 1) + 1; // funguje i pro insertingData.Count == 0

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
							default:
								break;
						}
						insertingData.Insert(InsertRowDataSourceIndex, insertRowDataItem);
					}
				}
			}

			commandFieldsToHide = new List<CommandField>();
			foreach (var column in Columns)
			{
				if (column is GridViewCommandField)
				{
					GridViewCommandField gridViewCommandField = (GridViewCommandField)column;
					gridViewCommandField.RowClickEnabledInGridView = this.IsRowClickEnabled();
					gridViewCommandField.RowClickCommandNameInGridView = RowClickCommandName;

					if ((EditorExtender != null) && (gridViewCommandField.ShowNewButtonForInsertByEditorExtender))
					{
						gridViewCommandField.ShowNewButton = gridViewCommandField.ShowInsertButton && AllowInserting;
					}
					// Chceme schovat obsah sloupce, ovšem jen pro tentokrát,
					// tj. chceme schovat, ale mít ho při příštím requestu nebo databindu zobrazený.
					// To bychom rádi udělali nastavením sloupce visible na false a po databindu zpět.
					// Jenže to nejde, protože viditelnost buněk gridu se nastavuje až v Renderu, konkrétně v PrepareControlHieararch a to tak, 
					// že se nastavuje viditelnost buněk gridu (buňky tedy existují i pro schované sloupce).
					if (((insertingData == null) || (insertingData.Count == 0)) && !Enabled)
					{
						// (alternativně místo předávání v kolekcí můžeme celou logiku přenést do metody PrepareControlHierarchy)
						commandFieldsToHide.Add(gridViewCommandField);
					}
				}
			}

			base.PerformDataBinding(insertingData);

			if (insertingData != null)
			{
				RequiresDataBinding = false;
				_currentlyRequiresDataBinding = false;
			}

			if (data != null)
			{
				if (HeaderRow != null)
				{
					HeaderRow.TableSection = TableRowSection.TableHeader;
				}

				if (FilterRow != null)
				{
					FilterRow.TableSection = TableRowSection.TableHeader;
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
		private IEnumerable _performDataBindingData;
		private bool _performDataBindingDataInFakeCall;
		private ControlsValuesHolder _previousFilterRowData;

		/// <summary>
		/// CreateRow.
		/// </summary>
		protected override GridViewRow CreateRow(int rowIndex, int dataSourceIndex, DataControlRowType rowType, DataControlRowState rowState)
		{
			GridViewRow row = base.CreateRow(rowIndex, dataSourceIndex, rowType, rowState);

			// Řádek s novým objektem přepínáme do stavu Insert, což zajistí zvolení EditItemTemplate a správné chování CommandFieldu.
			if ((rowType == DataControlRowType.DataRow)
				&& (IsInsertingByInsertRow)
				&& (dataSourceIndex == InsertRowDataSourceIndex))
			{
				insertRowIndex = rowIndex;
				row.RowState = DataControlRowState.Insert;
			}

			// abychom měli na stránce vždy stejný počet řádek, tak u insertingu při editaci skrýváme poslední řádek
			if ((IsInsertingByInsertRow) && (insertRowIndex < 0) && (AllowPaging) && (rowIndex == (this.PageSize - 1)))
			{
				row.Visible = false;
			}

			if (row.RowType == DataControlRowType.Header)
			{ 
				row.TableSection = TableRowSection.TableHeader;
			}

			return row;
		}

		/// <summary>
		/// Vyvolá událost RowCreated.
		/// </summary>
		protected override void OnRowCreated(GridViewRowEventArgs e)
		{
			// Ve fake volání perform databinding nevyvoláváme události RowCreated (ale RowFilterCreated se vyvolává, to je účel, _filterRow je pak použit ve skutečném volání).
			if (!_performDataBindingDataInFakeCall)
			{
				base.OnRowCreated(e);
			}
		}

		/// <summary>
		/// Vyvolá událost RowDataBound
		/// </summary>
		protected override void OnRowDataBound(GridViewRowEventArgs e)
		{
			// Ve fake volání perform databinding nevyvoláváme události RowDataBound (ale RowFilterCreated se vyvolává, to je účel, _filterRow je pak použit ve skutečném volání).
			if (!_performDataBindingDataInFakeCall)
			{
				if ((e.Row.RowType == DataControlRowType.DataRow) && this.IsRowClickEnabled())
				{
					GridViewRowCustomizingCommandButtonEventArgs args = new GridViewRowCustomizingCommandButtonEventArgs(RowClickCommandName, e.Row.RowIndex, e.Row.DataItem);
					args.Enabled = !((RowClickCommandName == CommandNames.Edit) && ((e.Row.RowIndex == EditIndex) || (e.Row.RowIndex == InsertIndex))) && !((RowClickCommandName == CommandNames.Select) && (e.Row.RowIndex == SelectedIndex));
					args.Visible = true;
                    this.OnRowCustomizingCommandButton(args);
					if (args.Enabled && args.Visible)
					{
						e.Row.Attributes["data-rowclick"] = this.Page.ClientScript.GetPostBackEventReference(new PostBackOptions(this, "RowClick$" + e.Row.RowIndex.ToString()));
						if (!String.IsNullOrEmpty(RowClickCssClass))
						{
							e.Row.CssClass = (e.Row.CssClass + " " + RowClickCssClass).Trim();
                        }
					}
				}
				base.OnRowDataBound(e);
			}
		}

		/// <summary>
		/// Událost vyvolaná připadě rozkliknutí řádku.
		/// </summary>
		protected void OnRowClick(GridViewRowClickEventArgs eventArgs)
		{
			if (RowClick != null)
			{
				RowClick(this, eventArgs);
			}
		}

		/// <summary>
		/// Událost vyvolaná připadě rozkliknutí řádku.
		/// </summary>
		public event GridViewRowClickEventHandler RowClick;

		/// <summary>
		/// Pokud je povoleno AllowRowClick, pak nastavením atributu data-suppressrowclick zajistí, aby nemohlo být kliknutí propagováno do nadřazeného gridu.
		/// </summary>
		protected override void OnDataBound(EventArgs e)
		{
			if (this.AllowRowClick)
			{
				this.Attributes["data-suppressrowclick"] = "true";
			}
			else
			{
				this.Attributes.Remove("data-suppressrowclick");
			}

			base.OnDataBound(e);
		}

		/// <summary>
		/// Nastaví RequiresDataBinding na true.
		/// Zajistí zavolání databindingu ještě v aktuálním requestu. Běžně v OnPreRender,
		/// pokud je ale GridView schovaný, pak se DataBind volá z Page.PreRenderComplete.
		/// </summary>
		public void SetRequiresDatabinding()
		{
			RequiresDataBinding = true;
			_currentlyRequiresDataBinding = true;
		}

		/// <summary>
		/// Příznak, zda má dojít k databindingu ještě v tomto requestu.
		/// Nastavováno (na true) v metodě SetRequiresDataBinding, vypínáno v metodě PerformDataBinding.
		/// </summary>
		private bool _currentlyRequiresDataBinding = false;

		/// <summary>
		/// Vyvolá událost FilterRowCreated.
		/// </summary>
		protected virtual void OnFilterRowCreated(GridViewRowEventArgs e)
		{
			GridViewRowEventHandler handler = (GridViewRowEventHandler)Events[eventFilterRowCreated];
			if (handler != null)
			{
				handler(this, e);
			}
		}

		/// <summary>
		/// Vyvolá událost FiltrerRowDataBound.
		/// </summary>
		protected virtual void OnFilterRowDataBound(GridViewRowEventArgs e)
		{
			GridViewRowEventHandler handler = (GridViewRowEventHandler)Events[eventFilterRowDataBound];
			if (handler != null)
			{
				handler(this, e);
			}
		}

		/// <summary>
		/// Vyvolá událost GridViewDataFiltering.
		/// </summary>
		protected virtual void OnGridViewDataFiltering(GridViewDataFilteringEventArgs e)
		{
			GridViewDataFilteringEventHandler handler = (GridViewDataFilteringEventHandler)Events[eventGridViewDataFiltering];
			if (handler != null)
			{
				handler(this, e);
			}
		}

		/// <summary>
		/// Provede filtrování dat automatickými filtry.
		/// </summary>
		protected virtual IEnumerable PerformAutoFiltering(IEnumerable data)
		{
			IEnumerable result = data;
			foreach (IAutoFilterControl autoFilterControl in autoFilterControls)
			{
				result = autoFilterControl.FilterData(result);
			}
			return result;
		}

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
					HandleInsert(Convert.ToInt32(e.CommandArgument, CultureInfo.InvariantCulture), causesValidation);
					break;

				case CommandNames.New:
					HandleNew(causesValidation);
					break;

				default:
					break;
			}
		}

		/// <summary>
		/// Metoda, která řídí logiku příkazu Insert.
		/// </summary>
		/// <param name="rowIndex">index řádku, kde insert probíhá</param>
		/// <param name="causesValidation">příznak, zdali má probíhat validace</param>
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

		/// <summary>
		/// Metoda, která řídí logiku příkazu New.
		/// </summary>
		/// <param name="causesValidation">příznak, zdali má probíhat validace</param>
		protected virtual void HandleNew(bool causesValidation)
		{
			if ((!causesValidation || (this.Page == null)) || this.Page.IsValid)
			{
				GridViewNewProcessingEventArgs argsNewProcessing = new GridViewNewProcessingEventArgs();
				OnNewProcessing(argsNewProcessing);
				if (!argsNewProcessing.Cancel)
				{
					GridViewNewProcessedEventArgs argsNewProcessed = new GridViewNewProcessedEventArgs();
					OnNewProcessed(argsNewProcessed);
				}
			}
		}

		/// <summary>
		/// Zajistí obluhu události RowClick. Tj. vyvolá událost pomocí OnRowClick a pokud není zrušená (Cancel) a zároveň je zadáno RowClickCommandName, pak se pokusí vyvolat událost dle RowClickCommandName pomocí OnBubbleEvent.
		/// </summary>
		private void HandleRowClick(int rowIndex)
		{
			GridViewRowClickEventArgs args = new GridViewRowClickEventArgs { RowIndex = rowIndex };
			OnRowClick(args);

			if (!args.Cancel && !String.IsNullOrEmpty(RowClickCommandName))
			{
				CommandEventArgs originalArgs = new CommandEventArgs(RowClickCommandName, rowIndex.ToString());
				GridViewCommandEventArgs e = new GridViewCommandEventArgs(null, this, originalArgs);
				OnBubbleEvent(this, e);
			}
		}

		/// <summary>
		/// Determines whether the event for the Web server control is passed up the page's user interface (UI) server control hierarchy.
		/// </summary>
		/// <returns>
		/// true if the event has been canceled; otherwise, false.
		/// </returns>
		/// <param name="source">The source of the event. </param><param name="e">An <see cref="T:System.EventArgs"/> that contains event data. </param>
		protected override bool OnBubbleEvent(object source, EventArgs e)
		{
			bool result = base.OnBubbleEvent(source, e);

			// pokud se má událost propagovat, ověříme, zda nejde o insert
			// Insert nechceme propagovat, stejně jako se nepropaguje Update, Delete, atp.
			// ve frameworku 2.0/3.5 se nepropaguje, ve frameworku 4.0 se propaguje
			// framework kompilujeme pod .NET 3.5, problém se ale objeví jen v aplikaci běžící pod .NET 4.0

			if (e is AutoFilterControlCreatedEventArgs)
			{
				IAutoFilterControl autoFilterControl = source as IAutoFilterControl;
				if (autoFilterControl != null)
				{
					autoFilterControl.ValueChanged += (sender, eventArgs) => { this.SetRequiresDatabinding(); };
					autoFilterControls.Add(autoFilterControl);
					result = true; // ano, metoda se jmenuje onbubbleevent, ale true znamená cancel
				}
			}

			if (!result)
			{
				GridViewCommandEventArgs args = e as GridViewCommandEventArgs;
				if (args != null)
				{
					IButtonControl commandSource = args.CommandSource as IButtonControl;
					if (commandSource != null)
					{
						if (String.Equals(commandSource.CommandName, "Insert", StringComparison.InvariantCultureIgnoreCase))
						{
							result = true; // ano, metoda se jmenuje onbubbleevent, ale true znamená cancel
						}
					}
				}
			}

			return result;
		}

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
				if (EditorExtender != null)
				{
					this.EditorExtenderEditIndex = e.NewEditIndex;
					EditorExtender.StartEditor();

					e.Cancel = true;
				}
				else
				{
					int newEditIndex = e.NewEditIndex;
					if ((IsInsertingByInsertRow) && (InsertRowDataSourceIndex >= 0) && (insertRowIndex < newEditIndex))
					{
						newEditIndex = newEditIndex - 1;
					}

					// .NET 4.0 má vlastní logiku, která nastavuje EditIndex po volání této metody. 
					// My zde opravujeme nastavení NewEditIndex, čímž řešíme logiku změny při insert řádku nahoře

					// ASP.NET 3.5 

					//private void HandleEdit(int rowIndex)
					//{
					//    GridViewEditEventArgs e = new GridViewEditEventArgs(rowIndex);
					//    this.OnRowEditing(e);
					//    if (!e.Cancel)
					//    {
					//        if (base.IsBoundUsingDataSourceID)
					//        {
					//            this.EditIndex = e.NewEditIndex;
					//        }
					//        base.RequiresDataBinding = true;
					//    }
					//}

					// ASP.NET 4.0
					// zde zmizela podmínka na IsBoundUsingDataSourceID

					//private void HandleEdit(int rowIndex)
					//{
					//    GridViewEditEventArgs e = new GridViewEditEventArgs(rowIndex);
					//    this.OnRowEditing(e);
					//    if (!e.Cancel)
					//    {
					//        this.EditIndex = e.NewEditIndex;
					//        base.RequiresDataBinding = true;
					//    }
					//}

					this.EditIndex = newEditIndex;
					e.NewEditIndex = newEditIndex;

					SetRequiresDatabinding();
					this.InsertRowDataSourceIndex = -1;
				}
			}
		}

		/// <summary>
		/// Raises the System.Web.UI.WebControls.GridView.RowUpdating event.
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

		/// <summary>
		/// Spouští událost RowInserting.
		/// </summary>
		/// <param name="e">argumenty události</param>
		protected virtual void OnRowInserting(GridViewInsertEventArgs e)
		{
			GridViewInsertEventHandler h = (GridViewInsertEventHandler)Events[eventItemInserting];
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
			GridViewInsertedEventHandler h = (GridViewInsertedEventHandler)Events[eventItemInserted];
			if (h != null)
			{
				h(this, e);
			}
		}

		/// <summary>
		/// Spouští událost RowNewBeginning. Událost zahajuje začátek zakládání nového záznamu.		
		/// </summary>
		/// <remarks>
		/// Vzhledek k jazykové nemožnosti použití konvence Editing/Edited, Inserting/Inserted pro "New" je zde dvojice NewProcessing/NewProcessed.
		/// </remarks>
		protected virtual void OnNewProcessing(GridViewNewProcessingEventArgs e)
		{
			GridViewNewProcessingEventHandler h = (GridViewNewProcessingEventHandler)Events[eventNewProcessing];
			if (h != null)
			{
				h(this, e);
			}

			if (!e.Cancel && (EditorExtender != null))
			{
				EditorExtenderEditIndex = -1;
				EditorExtender.StartEditor();
			}
		}

		/// <summary>
		/// Spouští událost RowNewCompleted. Událost ukončuje začátek zakládání nového záznamu. Vzhledek k jazykové nemožnosti použití konvence Editing/Edited, Inserting/Inserted pro "New" je zde dvojice NewBeginning/NewCompleted.
		/// </summary>
		/// <remarks>
		/// Vzhledek k jazykové nemožnosti použití konvence Editing/Edited, Inserting/Inserted pro "New" je zde dvojice NewProcessing/NewProcessed.
		/// </remarks>
		protected virtual void OnNewProcessed(GridViewNewProcessedEventArgs e)
		{
			GridViewNewProcessedEventHandler h = (GridViewNewProcessedEventHandler)Events[eventNewProcessed];
			if (h != null)
			{
				h(this, e);
			}
		}

		/// <summary>
		/// Při požadavku na řazení si zapamatujeme, jak chtěl uživatel řadit a nastavíme RequiresDataBinding na true.
		/// </summary>
		/// <param name="e">argumenty události</param>
		protected override void OnSorting(GridViewSortEventArgs e)
		{
			base.OnSorting(e);

			if (!e.Cancel)
			{
				SortExpressions.AddSortExpression(e.SortExpression);
				base.RequiresDataBinding = true;
			}
		}

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

			this.RegisterClientScripts();
		}

		private void RegisterClientScripts()
		{
			HavitFrameworkClientScriptHelper.RegisterHavitFrameworkClientScript(this.Page);
			if (((this.EditorExtenderEditIndexInternal != null) || _editorExtenderEditIndexInternalChanged) && !String.IsNullOrEmpty(this.EditorExtenderEditCssClass))
			{
				// nemáme k dispozici Row.ClientID!
				string script = String.Format("havitGridViewExtensions.setExternalEditorEditedRow('{0}', {1}, '{2}');",
					this.ClientID, // 0
					((this.EditorExtenderEditIndexInternal == null) || (this.EditorExtenderEditIndex == -1)) ? -1 : this.Rows[this.EditorExtenderEditIndex].RowIndex, // 1
					this.EditorExtenderEditCssClass); // 2

				ScriptManager.RegisterStartupScript(this.Page, typeof(GridViewExt), "SelectExternalEditorEditedRow", script, true);
			}

			if (this.AllowRowClick)
			{
				string script = String.Format("havitGridViewExtensions.initializeRowClick('{0}');", this.ClientID);
				ScriptManager.RegisterStartupScript(this, typeof(GridViewExt), String.Format("InitializeRowClick-{0}", this.ClientID), script, true);
			}
		}

		private void Page_PreRenderComplete(object sender, EventArgs e)
		{
			// pokud je control schovaný (není visible), nevolá se jeho OnPreRender.
			// Pokud ale byla zavolána metoda SetRequiresDatabinding, určitě chceme, aby k databindingu došlo ještě v tomto requestu.
			// Proto se něvěsíme na Page.PreRenderComplete jako nouzové řešení.
			if (_currentlyRequiresDataBinding && AutoDataBind && RequiresDataBinding)
			{
				DataBind();
			}
		}

		/// <summary>
		/// Spouští událost <see cref="RowCustomizingCommandButton"/>.
		/// </summary>
		/// <param name="e">argumenty události</param>
		protected internal virtual void OnRowCustomizingCommandButton(GridViewRowCustomizingCommandButtonEventArgs e)
		{
			GridViewRowCustomizingCommandButtonEventHandler h = (GridViewRowCustomizingCommandButtonEventHandler)Events[eventRowCustomizingCommandButton];
			if (h != null)
			{
				h(this, e);
			}
		}

		/// <summary>
		/// Pokud je nastavena PagerTemplate nebo PagerRenderMode Standard, pouižje se vestavěný způsob generování pageru.
		/// Pokud je PagerRenderMode BootstrapPagination, použije se způsob renderování odpovídající Pagination v bootstrapu.
		/// </summary>
		protected override void InitializePager(GridViewRow row, int columnSpan, PagedDataSource pagedDataSource)
		{
			if (PagerTemplate != null)
			{
				InitializeStandardPager(row, columnSpan, pagedDataSource);
			}
			else
			{
				switch (PagerRenderMode)
				{
					case PagerRenderMode.Standard:
						InitializeStandardPager(row, columnSpan, pagedDataSource);
						break;

					case PagerRenderMode.BootstrapPagination:
						this.InitializeBootstrapPagination(row, columnSpan, pagedDataSource);
						break;

					default:
						throw new NotSupportedException("PagerRenderMode not supported.");
				}
			}
		}

		/// <summary>
		/// Vytvoří standardní pager a (pokud je to povoleno a není použito PagerTemplate) přidá do něj tlačítko pro zobrazení všech stránek.
		/// </summary>
		private void InitializeStandardPager(GridViewRow row, int columnSpan, PagedDataSource pagedDataSource)
		{
			base.InitializePager(row, columnSpan, pagedDataSource);
			// pokud je použita výchozí šablona a je povoleno tlačítko pro vypnutí stránkování, přidáme jej
			if ((this.PagerTemplate == null) && (this.PagerSettingsShowAllPagesButton))
			{
				// najdeme řádek tabulky, do které budeme přidávat "All Pages Button".
				TableRow row2 = null;
				if ((row.Controls.Count == 1) && (row.Controls[0].Controls.Count == 1) && (row.Controls[0].Controls[0].Controls.Count == 1))
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
						allPagesLinkButton.Text = HttpUtilityExt.GetResourceString(this.PagerSettingsAllPagesButtonText);
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

		/// <summary>
		/// Vytvoří stránkování pomocí Pagionation v bootstrapu.
		/// Tato metoda jen rozděluje práci na další metody (deleguje).
		/// </summary>
		private void InitializeBootstrapPagination(GridViewRow row, int columnSpan, PagedDataSource pagedDataSource)
		{
			TableCell cell = new TableCell();
			if (columnSpan > 1)
			{
				cell.ColumnSpan = columnSpan;
			}
			WebControl paginationControl = new WebControl(HtmlTextWriterTag.Ul);
			paginationControl.CssClass = "pagination";

			switch (PagerSettings.Mode)
			{
				case PagerButtons.NextPrevious:
					this.CreateBootstrapNextPrevPagination(paginationControl, pagedDataSource, false);
					break;

				case PagerButtons.Numeric:
					this.CreateBootstrapNumericPagination(paginationControl, pagedDataSource, false);
					break;

				case PagerButtons.NextPreviousFirstLast:
					this.CreateBootstrapNextPrevPagination(paginationControl, pagedDataSource, true);
					break;

				case PagerButtons.NumericFirstLast:
					this.CreateBootstrapNumericPagination(paginationControl, pagedDataSource, true);
					break;

				default:
					throw new NotSupportedException($"{PagerSettings.Mode} not supported.");
			}

			if (PagerSettingsShowAllPagesButton)
			{
				LinkButton allPagesLinkButton = new LinkButton();
				allPagesLinkButton.ID = "AllPagesLinkButton";
				allPagesLinkButton.Text = HttpUtilityExt.GetResourceString(this.PagerSettingsAllPagesButtonText);
				allPagesLinkButton.Click += new EventHandler(AllPagesLinkButton_Click);
				allPagesLinkButton.CausesValidation = false;
				WebControl liControl = new WebControl(HtmlTextWriterTag.Li);
				liControl.Controls.Add(allPagesLinkButton);
				paginationControl.Controls.Add(liControl);
			}

			cell.Controls.Add(paginationControl);
			row.Cells.Add(cell);
		}

		/// <summary>
		/// Vytvoří pagination s čísly stránek.
		/// </summary>
		private void CreateBootstrapNumericPagination(Control container, PagedDataSource pagedDataSource, bool addFirstLastPageButtons)
		{
			PagerSettings pagerSettings = this.PagerSettings;
			int pageCount = pagedDataSource.PageCount;
			int currentPageIndexDenormalized = pagedDataSource.CurrentPageIndex + 1;
			int pageButtonCount = pagerSettings.PageButtonCount;

			int num4 = pageButtonCount;
			int num5 = this.FirstDisplayedPageIndex + 1;
			if (pageCount < num4)
			{
				num4 = pageCount;
			}
			int firstVisiblePageIndexDenormalized = 1;
			int lastVisiblePageIndexDenormalized = num4;
			if (currentPageIndexDenormalized > lastVisiblePageIndexDenormalized)
			{
				int num8 = pagedDataSource.CurrentPageIndex / pageButtonCount;
				bool flag = ((currentPageIndexDenormalized - num5) >= 0) && ((currentPageIndexDenormalized - num5) < pageButtonCount);
				if ((num5 > 0) && flag)
				{
					firstVisiblePageIndexDenormalized = num5;
				}
				else
				{
					firstVisiblePageIndexDenormalized = (num8 * pageButtonCount) + 1;
				}
				lastVisiblePageIndexDenormalized = (firstVisiblePageIndexDenormalized + pageButtonCount) - 1;
				if (lastVisiblePageIndexDenormalized > pageCount)
				{
					lastVisiblePageIndexDenormalized = pageCount;
				}
				if (((lastVisiblePageIndexDenormalized - firstVisiblePageIndexDenormalized) + 1) < pageButtonCount)
				{
					firstVisiblePageIndexDenormalized = Math.Max(1, (lastVisiblePageIndexDenormalized - pageButtonCount) + 1);
				}
				this.FirstDisplayedPageIndex = firstVisiblePageIndexDenormalized - 1;
			}

			//int firstVisiblePageIndexDenormalized;
			//int lastVisiblePageIndexDenormalized;
			//if (pagedDataSource.PageCount <= pageButtonCount)
			//{
			//	firstVisiblePageIndexDenormalized = 1;
			//	lastVisiblePageIndexDenormalized = pagedDataSource.PageCount;
			//}
			//else if (pagedDataSource.CurrentPageIndex <= (pageButtonCount / 2M))
			//{
			//	firstVisiblePageIndexDenormalized = 1;
			//	lastVisiblePageIndexDenormalized = pageButtonCount;
			//}
			//else if (pagedDataSource.CurrentPageIndex > pagedDataSource.PageCount - (pageButtonCount / 2))
			//{
			//	firstVisiblePageIndexDenormalized = pagedDataSource.PageCount - pageButtonCount + 1;
			//	lastVisiblePageIndexDenormalized = pagedDataSource.PageCount;
			//}
			//else
			//{
			//	firstVisiblePageIndexDenormalized = currentPageIndexDenormalized - pageButtonCount / 2;
			//	lastVisiblePageIndexDenormalized = firstVisiblePageIndexDenormalized + pageButtonCount - 1;
			//}

			if (addFirstLastPageButtons)
			{
				AddBootstrapPagerButton(container, pagerSettings.FirstPageText, "first", "First", !pagedDataSource.IsFirstPage);
			}

			if (firstVisiblePageIndexDenormalized != 1)
			{
				AddBootstrapPagerButton(container, "...", null, (firstVisiblePageIndexDenormalized - 1).ToString(NumberFormatInfo.InvariantInfo));
			}

			for (int i = firstVisiblePageIndexDenormalized; i <= lastVisiblePageIndexDenormalized; i++)
			{
				AddBootstrapPagerButton(container, i.ToString(NumberFormatInfo.CurrentInfo), null, i.ToString(NumberFormatInfo.InvariantInfo), i != currentPageIndexDenormalized, i == currentPageIndexDenormalized);
			}

			if (pageCount > lastVisiblePageIndexDenormalized)
			{
				AddBootstrapPagerButton(container, "...", null, (lastVisiblePageIndexDenormalized + 1).ToString(NumberFormatInfo.InvariantInfo));
			}

			if (addFirstLastPageButtons)
			{
				AddBootstrapPagerButton(container, pagerSettings.LastPageText, "last", "Last", !pagedDataSource.IsLastPage);
			}
		}

		/// <summary>
		/// Vytvoří pagination s navigací na předchozí/následující stránku.
		/// </summary>
		private void CreateBootstrapNextPrevPagination(Control container, PagedDataSource pagedDataSource, bool addFirstLastPageButtons)
		{
			PagerSettings pagerSettings = this.PagerSettings;
			bool isFirstPage = pagedDataSource.IsFirstPage;
			bool isLastPage = pagedDataSource.IsLastPage;
			if (addFirstLastPageButtons)
			{
				AddBootstrapPagerButton(container, pagerSettings.FirstPageText, "first", "First", !isFirstPage);
			}
			
			AddBootstrapPagerButton(container, pagerSettings.PreviousPageText, "previous", "Prev", !isFirstPage);
			AddBootstrapPagerButton(container, pagerSettings.NextPageText, "next", "Next", !isLastPage);

			if (addFirstLastPageButtons)
			{
				AddBootstrapPagerButton(container, pagerSettings.LastPageText, "last", "Last", !isLastPage);
			}
		}

		/// <summary>
		/// Přidá do kontejnetu tlačítko pageru dle požadovaných parametrů.
		/// </summary>
		private void AddBootstrapPagerButton(Control container, string text, string cssClass, string commandArgument, bool enabled = true, bool active = false)
		{
			LinkButton pagingButton = new LinkButton();
			pagingButton.Text = text;
			pagingButton.CommandName = "Page";
			pagingButton.CommandArgument = commandArgument;
			pagingButton.Enabled = enabled;
			pagingButton.CausesValidation = false;

			string stateCssClass = "";
			if (active)
			{
				stateCssClass = "active";
			}
			else if (!enabled)
			{
				stateCssClass = "disabled";
			}

			WebControl liControl = new WebControl(HtmlTextWriterTag.Li);
			liControl.CssClass = (cssClass + " " + stateCssClass).Trim();
			liControl.Controls.Add(pagingButton);
			container.Controls.Add(liControl);
		}

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

		/// <summary>
		/// Vrací true, pokud je povoleno kliknutí do celého řádku.
		/// </summary>
		protected bool IsRowClickEnabled()
		{
			return AllowRowClick && IsEnabled;
		}

		/// <summary>
		/// Obsluha událost při obsluze kliknutí na tlačítko "All Pages" (tlačítko, vypínající stránkování). Dává možnost zrušit akci.
		/// </summary>
		protected virtual void OnAllPagesShowing(CancelEventArgs cancelEventArgs)
		{
			CancelEventHandler h = (CancelEventHandler)Events[eventAllPagesShowing];
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
			EventHandler h = (EventHandler)Events[eventAllPagesShown];
			if (h != null)
			{
				h(this, eventArgs);
			}
		}

		/// <summary>
		/// Vyzvedne z řádku GridView hodnoty, které jsou nabidnované způsobem pro two-way databinding.
		/// Hodnoty nastaví jako vlastnosti předanému datovému objektu.
		/// </summary>
		/// <param name="row">Řádek gridu, jehož hodnoty jsou z UI extrahovány a nastaveny datovému objektu.</param>
		/// <param name="dataObject">Datový objekt, jehož hodnoty jsou nastaveny.</param>
		public void ExtractRowValues(GridViewRow row, object dataObject)
		{
			Contract.Requires(row != null);
			Contract.Requires(dataObject != null);

			System.Collections.Specialized.IOrderedDictionary fieldValues = new System.Collections.Specialized.OrderedDictionary();			
			this.ExtractRowValues(fieldValues, row, false, false);
			DataBinderExt.SetValues(dataObject, fieldValues);
		}

		/// <summary>
		/// Renders the Web server control content to the client's browser using the specified System.Web.UI.HtmlTextWriter object.
		/// </summary>
		protected override void Render(HtmlTextWriter writer)
		{
			// Zde se teprve aplikují styly na řádky - jako začátek base.Render(HtmlTextWriter), ale ještě přede zavoláním RenderContent(HtmlTextWriter).
			// FilterRow je generován jako typ Header, takže je na něj aplikován styl pro Header. 
			// To potlačíme tak, že styl pro header vyčistíme, a aplikujeme sami styl pro header i pro filter ručně po nastavení hodnot v předkovi.

			// ViewState neřešíme, ten je již uložen.

			_renderHeaderStyle = new TableItemStyle();
			_renderHeaderStyle.MergeWith(HeaderStyle);
			HeaderStyle.Reset();

			base.Render(writer);

		}

		/// <summary>
		/// Renders the contents of the control to the specified writer. This method is used primarily by control developers.
		/// </summary>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			if (ShowHeader && HeaderRow != null)
			{
				HeaderRow.MergeStyle(this._renderHeaderStyle);
			}

			if (ShowFilter && FilterRow != null)
			{
				FilterRow.MergeStyle(FilterStyle);

				foreach (TableCell cell in FilterRow.Cells)
				{
					if (cell is DataControlFieldCell)
					{
						DataControlFieldCell dataControlFieldCell = (DataControlFieldCell)cell;
						DataControlField field = dataControlFieldCell.ContainingField;

						if (field is IFilterField)
						{
							IFilterField filterField = (IFilterField)field;
							TableItemStyle filterFieldStyle = filterField.FilterStyleInternal;
							if (filterFieldStyle != null)
							{
								cell.MergeStyle(filterFieldStyle);
							}
						}
					}
				}
			}

			if (!String.IsNullOrEmpty(ContainerCssClass))
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Class, ContainerCssClass);
				writer.RenderBeginTag(HtmlTextWriterTag.Div);
			}

			base.RenderContents(writer);

			if (!String.IsNullOrEmpty(ContainerCssClass))
			{
				writer.RenderEndTag();
			}
		}
		private TableItemStyle _renderHeaderStyle;

		/// <summary>
		/// Zajistí schování GridViewCommandField pro přidání nového záznamu, pokud je grid disablován a není žádný záznam k přidání.
		/// </summary>
		protected override void PrepareControlHierarchy()
		{
			// Viz komentář v PerformDataBinding.
			// O viewstate se nestaráme, jsme již po jeho uložení.
			if (commandFieldsToHide != null)
			{
				foreach (CommandField commandField in commandFieldsToHide)
				{

					commandField.Visible = false;
				}
			}
			base.PrepareControlHierarchy();
		}

		/// <summary>
		/// Registruje editor pro použití s GridView.
		/// </summary>
		public virtual void RegisterEditor(IEditorExtender editorExtender)
		{
			Contract.Requires(editorExtender != null);

			this.EditorExtender = editorExtender;
			editorExtender.EditClosed += EditorExtenderEditClosed;
			editorExtender.ItemSaved += EditorExtenderItemSaved;
			editorExtender.NewProcessing += EditorExtenderNewProcessing;
			editorExtender.GetCanCreateNew += EditorExtenderGetCanCreateNew;

			if (editorExtender is IEditorExtenderWithPreviousNextNavigation)
			{
				IEditorExtenderWithPreviousNextNavigation editorExtenderWithPreviousNextNavigation = (IEditorExtenderWithPreviousNextNavigation)editorExtender;
				editorExtenderWithPreviousNextNavigation.PreviousNavigating += EditorExtenderPreviousNavigating;
				editorExtenderWithPreviousNextNavigation.NextNavigating += EditorExtenderNextNavigating;
				editorExtenderWithPreviousNextNavigation.GetCanNavigatePrevious += EditorExtenderGetCanNavigatePrevious;
				editorExtenderWithPreviousNextNavigation.GetCanNavigateNext += EditorExtenderGetCanNavigateNext;
			}
		}

		/// <summary>
		/// Ukončí režim editace externím editorem.
		/// </summary>
		private void EditorExtenderEditClosed(object sender, EventArgs e)
		{
			EditorExtenderEditIndexInternal = null;
		}

		private void EditorExtenderItemSaved(object sender, EditorExtenderItemSavedEventArgs e)
		{
			// SetRequiresDatabinding(); - nemá význam - TrySetEditorExtenderEditIndex volá DataBind
			UpdateParentUpdatePanel();

			// pokud jsme uložili objekt, najdeme jeho pozici v nabidnovaném gridu (pozice editovaného řádku se mohla změnit)
			// zároveň je potřeba najít nově založený objekt, což je mj. ochrana proti opakovanému klikání na tlačítko save v editoru
			TrySetEditorExtenderEditIndex(e.SavedObject);
		}

		/// <summary>
		/// Aktualizuje nejbližší nadřazený update panel. Pokud neexistuje, je vyhozena výjimka.
		/// </summary>
		private void UpdateParentUpdatePanel()
		{
			if (ScriptManager.GetCurrent(this.Page).IsInAsyncPostBack)
			{
				Control parent = this.Parent;
				while (parent != null && (!(parent is UpdatePanel)))
				{
					parent = parent.Parent;
				}
				if (parent == null)
				{
					throw new HttpException("GridView must be inside UpdatePanel to be able to update its content.");
				}
				UpdatePanel updatePanel = (UpdatePanel)parent;
				if (updatePanel.UpdateMode == UpdatePanelUpdateMode.Conditional)
				{
					updatePanel.Update();
				}
			}
		}

		private bool TrySetEditorExtenderEditIndex(object dataObject)
		{
			int i = 0;
			int originalPageIndex = PageIndex;
			bool found = false;

			// funkce detekující, zda je na řádek bindován dataObject
			// pokud ano, nastavíme EditorExtenderEditIndex
			GridViewRowEventHandler detectDataObjectInRow = (sender, args) =>
				{
					if ((args.Row.RowType == DataControlRowType.DataRow) && (args.Row.DataItem == dataObject))
					{
						EditorExtenderEditIndex = args.Row.RowIndex;
						found = true;
					}
				};

            Action seachDataItemOnPage = () =>
            {
                RowDataBound += detectDataObjectInRow;
                // eliminujeme opakované volání události DataBind a tím třeba opakované vytahování dat z databáze
                if ((DataSource != null) && (DataSource is IEnumerable))
                {
                    PerformDataBinding((IEnumerable)DataSource);
                }
                else
                {
                    DataBind();
                }
                RowDataBound -= detectDataObjectInRow;
            };
            
            if (AllowCustomPaging && (VirtualItemCount > 0))
            {
                seachDataItemOnPage();
                return found;
            }

			while (!found)
			{
				if (i >= PageCount)
				{
					// pokud jsme objekt nenašli, vrátíme se tam, co jsme byli
					if (originalPageIndex != PageIndex)
					{
						PageIndex = originalPageIndex;
						if ((DataSource != null) && (DataSource is IEnumerable))
						{
							PerformDataBinding((IEnumerable)DataSource);
						}
						else
						{
							DataBind();
						}
					}
					return false;
				}

				PageIndex = i;

                seachDataItemOnPage();

                i = i + 1;
			}
			return true;
		}

		/// <summary>
		/// Vrací true, pokud lze založit nový objekt.
		/// </summary>
		private bool CanCreateNew()
		{
			return AllowInserting;
		}

		/// <summary>
		/// Obsluhuje událost EditorExtenderu dotazující se na to, zda lze založit novou položku.
		/// </summary>
		private void EditorExtenderGetCanCreateNew(object sender, DataEventArgs<bool> e)
		{
			e.Data = CanCreateNew();
		}

		/// <summary>
		/// Vrací true, pokud lze navigovat na předchozí položku.
		/// </summary>
		private bool CanNavigatePrevious()
		{
			// buď (nejsme na prvním záznamu, tj. můžeme jít zpět o záznam) 
			// nebo (jsme na prvním záznamu na "další" stránce, tj. můžeme jít o stránku zpět)
			return (EditorExtenderEditIndex > 0) || ((EditorExtenderEditIndex == 0) && (PageIndex > 0));
		}

		/// <summary>
		/// Vrací true, pokud lze navigovat na následující položku.
		/// </summary>
		private bool CanNavigateNext()
		{
			// nejsme v editaci a
			// buď (nejsme na poslední stránce a je povoleno stránkování, tj. můžeme jít na další stránku)
			// nebo (jsme na poslední stránce a můžeme jít na další záznam, pro přechod z nového objektu kontrolujeme ještě, že existuje záznam, na který můžeme jít)
			return (EditorExtenderEditIndex >= 0) && ((AllowPaging && (PageIndex < (PageCount - 1))) || (EditorExtenderEditIndex < (Rows.Count - 1) && (Rows.Count > 0)));
		}

		/// <summary>
		/// Obsluhuje událost EditorExtenderu dotazující se na to, zda lze navigovat na předchozí položku.
		/// </summary>
		private void EditorExtenderGetCanNavigatePrevious(object sender, DataEventArgs<bool> e)
		{
			e.Data = CanNavigatePrevious();
		}

		/// <summary>
		/// Obsluhuje událost EditorExtenderu dotazující se na to, zda lze navigovat na následující položku.
		/// </summary>
		private void EditorExtenderGetCanNavigateNext(object sender, DataEventArgs<bool> e)
		{
			e.Data = CanNavigateNext();
		}

		private void EditorExtenderNewProcessing(object sender, CancelEventArgs e)
		{
			if (!AllowInserting)
			{
				e.Cancel = true;
				return;
			}

			EditorExtenderEditIndex = -1;
		}

		/// <summary>
		/// Zajišťuje navigaci na předchozí položku.
		/// </summary>
		private void EditorExtenderPreviousNavigating(object sender, CancelEventArgs e)
		{
			if (!CanNavigatePrevious())
			{
				e.Cancel = true;
				return;
			}

			if (EditorExtenderEditIndex > 0)
			{
				EditorExtenderEditIndex = EditorExtenderEditIndex - 1;				
			}
			else if ((EditorExtenderEditIndex == 0) && (PageIndex > 0))
			{
				PageIndex = PageIndex - 1;
				DataBind();
				UpdateParentUpdatePanel();
				EditorExtenderEditIndex = Rows.Count - 1;
			}
		}

		/// <summary>
		/// Zajišťuje navigaci na předchozí položku.
		/// </summary>
		private void EditorExtenderNextNavigating(object sender, CancelEventArgs e)
		{
			if (!CanNavigateNext())
			{
				e.Cancel = true;
				return;
			}
			
			if (EditorExtenderEditIndex == (Rows.Count - 1))
			{
				PageIndex = PageIndex + 1;
				DataBind();
				UpdateParentUpdatePanel();
				EditorExtenderEditIndex = 0;
			}
			else
			{
				EditorExtenderEditIndex = EditorExtenderEditIndex + 1;
			}
		}
	}

	/// <summary>
	/// Reprezentuje metodu, která obsluhuje událost RowClick controlu GridViewExt.
	/// </summary>
	public delegate void GridViewRowClickEventHandler(object sender, GridViewRowClickEventArgs e);

	/// <summary>
	/// Reprezentuje metodu, která obsluhuje událost RowInserting controlu GridViewExt.
	/// </summary>
	public delegate void GridViewInsertEventHandler(object sender, GridViewInsertEventArgs e);

	/// <summary>
	/// Reprezentuje metodu, která obsluhuje událost RowInserted controlu GridViewExt.
	/// </summary>
	public delegate void GridViewInsertedEventHandler(object sender, GridViewInsertedEventArgs e);

	/// <summary>
	/// Reprezentuje metodu, která obsluhuje událost NewProcessing controlu GridViewExt.
	/// </summary>
	public delegate void GridViewNewProcessingEventHandler(object sender, GridViewNewProcessingEventArgs e);

	/// <summary>
	/// Reprezentuje metodu, která obsluhuje událost NewProcessed controlu GridViewExt.
	/// </summary>
	public delegate void GridViewNewProcessedEventHandler(object sender, GridViewNewProcessedEventArgs e);

	/// <summary>
	/// Reprezentuje metodu, která obsluhuje událost <see cref="GridViewExt.RowCustomizingCommandButton"/>.
	/// </summary>
	public delegate void GridViewRowCustomizingCommandButtonEventHandler(object sender, GridViewRowCustomizingCommandButtonEventArgs e);

	/// <summary>
	/// Delegát k metodě pro získávání data-item pro nový Insert řádek GridView.
	/// </summary>
	public delegate object GetInsertRowDataItemDelegate();
	
}
