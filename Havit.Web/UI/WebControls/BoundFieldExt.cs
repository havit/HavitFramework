using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web;
using System.Web.UI;
using System.ComponentModel;
using System.Reflection;
using Havit.Diagnostics.Contracts;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Sloupec pro heterogenní seznamy.
	/// </summary>
	public class BoundFieldExt : System.Web.UI.WebControls.BoundField, IIdentifiableField, IFilterField
	{
		/// <summary>
		/// Identifikátor fieldu na který se lze odkazovat pomocí <see cref="GridViewExt.FindColumn(string)"/>.
		/// </summary>
		public string ID
		{
			get
			{
				object tmp = ViewState["ID"];
				if (tmp != null)
				{
					return (string)tmp;
				}
				return String.Empty;
			}
			set
			{
				ViewState["ID"] = value;
			}
		}

		/// <summary>
		/// Css třída vygenerované buňky.	
		/// Zamýšleno např. pro omezení šírky sloupce.
		/// Viz	<see cref="InitializeDataCell(System.Web.UI.WebControls.DataControlFieldCell, System.Web.UI.WebControls.DataControlRowState)" />.
		/// </summary>
		internal string CellCssClass
		{
			get { return (string)(ViewState["CellCssClass"] ?? String.Empty); }
			set { ViewState["CellCssClass"] = value; }
		}

		/// <summary>
		/// Pokud se metodě GetValue nepodaří získat hodnotu z dat, použije se tato hodnota, pokud není null.
		/// Rozlišuje se null a prázdný řetězec.
		/// Viz metody GetNotFoundDataItem a GetValue.
		/// </summary>
		public string EmptyText
		{
			get
			{
				return (string)ViewState["EmptyText"];
			}
			set
			{
				ViewState["EmptyText"] = value;
			}
		}

		/// <summary>
		/// Gets or sets the string that specifies the display format for the value of the field.
		/// </summary>
		/// <remarks>
		/// Nastavením se přepne výchozí hodnota HtmlEncode na false.
		/// </remarks>
		public override string DataFormatString
		{
			get
			{
				return base.DataFormatString;
			}
			set
			{
				base.DataFormatString = value;
				
				// pokud není explicitně nastaveno HtmlEncode, pak ho vypneme
				if (ViewState["HtmlEncode"] == null)
				{
					this.HtmlEncode = false;
				}
			}
		}

		/// <summary>
		/// Režim automatického filteru.
		/// Výchozí hodnota je None.
		/// </summary>
		public AutoFilterMode FilterMode
		{
			get
			{
				return (AutoFilterMode)(ViewState["FilterMode"] ?? AutoFilterMode.None);
			}
			set
			{
				ViewState["FilterMode"] = value;
			}
		}

		/// <summary>
		/// Field, který se používá ve filtru.
		/// Pokud není nastaveno, použije se DataField.
		/// </summary>
		public string DataFilterField
		{
			get
			{
				return (string)(ViewState["DataFilterField"] ?? DataField);
			}
			set
			{
				ViewState["DataFilterField"] = value;
			}
		}

		/// <summary>
		/// Styl buňky filtru.
		/// </summary>
		[DefaultValue(null)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TableItemStyle FilterStyle
		{
			get
			{
				if (this._filterStyle == null)
				{
					this._filterStyle = new TableItemStyle();
					if (this.IsTrackingViewState)
					{
						((IStateManager)this._filterStyle).TrackViewState();
					}
				}
				return this._filterStyle;
			}
		}
		private TableItemStyle _filterStyle;

		/// <summary>
		/// Vyžadováno implementací Fieldu v .NETu. V potomcích nutno přepsat.
		/// </summary>
		/// <returns>Instance této třídy.</returns>
		protected override DataControlField CreateField()
		{
			return new Havit.Web.UI.WebControls.BoundFieldExt();
		}

		private static readonly char[] indexExprStartChars = new char[] { '[', '(' };

		/// <summary>
		/// Získá hodnotu pro zobrazení na základě datového zdroje a DataFieldu.
		/// </summary>
		/// <param name="controlContainer">Control container (řádek GridView), kterému se získává hodnota.</param>
		protected override object GetValue(Control controlContainer)
		{
			Contract.Requires<ArgumentNullException>(controlContainer != null, nameof(controlContainer));

			object dataItem = DataBinder.GetDataItem(controlContainer);

			if (DesignMode)
			{
				return GetDesignTimeValue();
			}

			if (dataItem == null)
			{
				throw new Exception("Nepodařilo se získat objekt s daty.");
			}

			if (DataField == ThisExpression)
			{
				return dataItem;
			}

			object value = DataBinderExt.GetValue(dataItem, DataField);
			if ((value == null) || (value == DBNull.Value))
			{
				return GetNotFoundDataItem();
			}
			return value;
		}

		///// <summary>
		///// Získá hodnotu pro zobrazení z předaného objektu a dataField.
		///// </summary>
		///// <param name="dataItem">Položka dat z DataSource</param>
		///// <param name="dataField">DataField</param>
		///// <returns></returns>
		//protected object GetValue(object dataItem, string dataField)
		//{
		//    string[] expressionParts = dataField.Split('.');

		//    object currentValue = dataItem;

		//    int i = 0;
		//    int lastExpressionIndex = expressionParts.Length - 1;
		//    for (i = 0; i <= lastExpressionIndex; i++)
		//    {
		//        string expression = expressionParts[i];

		//        if (expression.IndexOfAny(indexExprStartChars) < 0)
		//        {
		//            currentValue = DataBinder.GetPropertyValue(currentValue, expression);
		//        }
		//        else
		//        {
		//            currentValue = DataBinder.GetIndexedPropertyValue(currentValue, expression);
		//        }

		//        if (currentValue == null) // && (i < lastExpressionIndex))
		//        {
		//            return GetNotFoundDataItem();
		//        }
		//    }

		//    return currentValue;
		//}

		/// <summary>
		/// Formátuje hodnotu k zobrazení.
		/// </summary>
		/// <param name="value">Data</param>
		/// <returns>Text k zobrazení.</returns>
		public virtual string FormatDataValue(object value)
		{
			// v tento okamžik je zde jako plán k přepsání (override)

			//if (NumberFormatter.IsNumber(value))
			//    return NumberFormatter.Format((IFormattable)value, DataFormatString);
			//else
			return FormatDataValue(value, SupportsHtmlEncode && HtmlEncode);
		}

		/// <summary>
		/// Metoda je volána, pokud se metodě GetValue nepodaří získat hodnotu.
		/// Není-li EmptyText rovno null, vrací se hodnota Empty text. 
		/// Jinak je vyhozena výjimka MissingMemberException.
		/// </summary>
		protected virtual object GetNotFoundDataItem()
		{
			if (EmptyText != null)
			{
				return EmptyText;
			}

			throw new InvalidOperationException(String.Format("Při zpracování hodnoty z DataFieldu \"{0}\" byla získána hodnota null nebo DBNull.Value, ale není nastavena hodnota vlastnosti EmptyText.", DataField));
		}

		/// <summary>
		/// Pokud není CellCssClass prázdné, generuje se do buňky tabulky &lt;div="CellCssClass"&gt;...&lt;/div&gt;.
		/// Jinak se použije normálně samotná buňka tabulky.
		/// </summary>
		protected override sealed void InitializeDataCell(System.Web.UI.WebControls.DataControlFieldCell cell, System.Web.UI.WebControls.DataControlRowState rowState)
		{
			if (!String.IsNullOrEmpty(CellCssClass))
			{
				Panel panel = new Panel();
				panel.CssClass = CellCssClass;
				cell.Controls.Add(panel);
				InitializeDataCellContent(panel, rowState);
			}
			else
			{
				InitializeDataCellContent(cell, rowState);
			}
		}

		/// <summary>
		/// Inicializuje obsah buňky daty.
		/// </summary>
		/// <param name="control">Control, do kterého se má obsah inicializovat.</param>
		/// <param name="rowState">RowState.</param>
		protected virtual void InitializeDataCellContent(Control control, DataControlRowState rowState)
		{			
			Literal literal = new Literal();
			control.Controls.Add(literal);
			literal.DataBinding += delegate(object sender, EventArgs e)
			{
				object value = GetValue(literal.NamingContainer);
				literal.Text = FormatDataValue(value);
			};
		}

		/// <summary>
		/// Copies the properties of the current System.Web.UI.WebControls.BoundField object to the specified System.Web.UI.WebControls.DataControlField object.
		/// </summary>
		protected override void CopyProperties(DataControlField newField)
		{
			base.CopyProperties(newField);
			if (newField is IFilterField)
			{
				((IFilterField)newField).FilterStyle.CopyFrom(this.FilterStyle);
			}
		}

		/// <summary>
		/// Saves the changes made to the System.Web.UI.WebControls.DataControlField view state since the time the page was posted back to the server.
		/// </summary>
		protected override object SaveViewState()
		{
			return new object[]
			{
				base.SaveViewState(),
				(_filterStyle != null) ? ((IStateManager)_filterStyle).SaveViewState() : null
			};
		}

		/// <summary>
		/// Restores the previously stored view-state information for this field.
		/// </summary>
		protected override void LoadViewState(object savedState)
		{
			object[] saveStateData = (object[])savedState;

			base.LoadViewState(saveStateData[0]);
			if (saveStateData[1] != null)
			{
				((IStateManager)this.FilterStyle).LoadViewState(saveStateData[1]);
			}
		}

		/// <summary>
		/// Causes the System.Web.UI.WebControls.DataControlField object to track changes to its view state so they can be stored in the control's
		/// System.Web.UI.WebControls.DataControlField.ViewState property and persisted across requests for the same page.
		/// </summary>
		protected override void TrackViewState()
		{
			base.TrackViewState();
			if (_filterStyle != null)
			{
				((IStateManager)_filterStyle).TrackViewState();
			}
		}

		TableItemStyle IFilterField.FilterStyleInternal
		{
			get { return _filterStyle; }
		}

		/// <summary>
		/// Vloží do buňky instanci controlu automatického filtru.
		/// </summary>
		void IFilterField.InitializeFilterCell(DataControlFieldCell cell)
		{
			switch (FilterMode)
			{
				case AutoFilterMode.None:
					// NOOP
					break;

				case AutoFilterMode.TextBox:
					AutoFilterTextBox autoFilterTextBox = new AutoFilterTextBox();
					autoFilterTextBox.DataFilterField = this.DataFilterField;
					cell.Controls.Add(autoFilterTextBox);
					break;

				case AutoFilterMode.DropDownList:
					AutoFilterDropDownList autoFilterDropDownList = new AutoFilterDropDownList();
					autoFilterDropDownList.DataTextField = this.DataFilterField;
					autoFilterDropDownList.DataTextFormatString = this.DataFormatString;
					cell.Controls.Add(autoFilterDropDownList);
					break;

				default: throw new ApplicationException("Neznámá hodnota vlastnosti FilterMode.");
			}
		}
	}
}