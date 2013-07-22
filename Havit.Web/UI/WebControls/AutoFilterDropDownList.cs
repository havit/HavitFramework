using Havit.Collections;
using Havit.Diagnostics.Contracts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// DropDownList pro automatické filtry.
	/// Sám se spojí s GridView, získá bidnovaná data a z nich získá distinct hodnoty pro filtrování. Sám (ve spojení s gridem) filtruje jeho bindované hodnoty.
	/// Pro funkčnost je třeba controlu nastavit vlastnost DataFilterField a NoFilterText.
	/// </summary>
	public class AutoFilterDropDownList : DropDownListExt, IAutoFilterControl
	{
		#region NoFilterText
		/// <summary>
		/// Text pro řádek "nerozhoduje".
		/// Pokud bude hodnota zadána, použije se text "---".	
		/// </summary>
		public string NoFilterText
		{
			get
			{
				return (string)(ViewState["NoFilterText"] ?? String.Empty);
			}
			set
			{
				ViewState["NoFilterText"] = value;				
			}
		}
		#endregion

		#region DataFilterField
		/// <summary>
		/// Vlastnost, ve které se vyhledává.
		/// </summary>
		public string DataFilterField
		{
			get
			{
				return (string)(ViewState["DataFilterField"] ?? this.DataTextField);
			}
			set
			{
				ViewState["DataFilterField"] = value;
			}
		}
		#endregion

		#region SortExpression
		/// <summary>
		/// Určuje, podle jaké property jsou řazena.
		/// Může obsahovat více vlastností oddělených čárkou, směr řazení ASC/DESC. Má tedy význam podobný jako DefaultSortExpression u GridViewExt.
		/// </summary>
		public string SortExpression
		{
			get { return (string)ViewState["SortExpression"] ?? String.Empty; }
			set { ViewState["SortExpression"] = value; }
		}
		#endregion

		#region ValueChanged, OnValueChanged
		/// <summary>
		/// Událost oznamuje změnu hodnoty filtru.
		/// </summary>
		public event EventHandler ValueChanged
		{
			add
			{
				Events.AddHandler(eventValueChanged, value);
			}
			remove
			{
				Events.RemoveHandler(eventValueChanged, value);
			}
		}
		private static readonly object eventValueChanged = new object();
		
		/// <summary>
		/// Vyvolá událost ValueChanged.
		/// </summary>
		protected void OnValueChanged(EventArgs e)
		{
			EventHandler handler = (EventHandler)Events[eventValueChanged];
			if (handler != null)
			{
				handler(this, e);
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Konstruktor.
		/// </summary>
		public AutoFilterDropDownList()
		{
			AutoPostBack = true;
		}
		#endregion

		#region FilterData
		/// <summary>
		/// Provede filtrování dat na základě nastavení filtru.
		/// </summary>
		public IEnumerable FilterData(IEnumerable data)
		{
			if (!String.IsNullOrEmpty(this.SelectedValue))
			{
				string filterValue = this.SelectedItem.Text;
				return data.Cast<object>().Where(item => DataBinderExt.GetValue(item, this.DataFilterField, this.DataTextFormatString) == filterValue).ToList();
			}
			else
			{
				return data;
			}
		}
		#endregion

		#region OnInit
		/// <summary>
		/// Inicializuje control.
		/// Vyvoláním události s argumentem AutoFilterControlCreatedEventArgs.Empty se registruje jak control pro automatický databind.
		/// </summary>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.RaiseBubbleEvent(this, AutoFilterControlCreatedEventArgs.Empty);
		}
		#endregion

		#region OnSelectedIndexChanged
		/// <summary>
		/// Při změně hodnoty DDL vyvolá událost ValueChanged.
		/// </summary>
		protected override void OnSelectedIndexChanged(EventArgs e)
		{
			base.OnSelectedIndexChanged(e);
			this.OnValueChanged(EventArgs.Empty);
		}
		#endregion

		#region DataBind
		/// <summary>
		/// Naplní DDL hodnotami pro filtr.
		/// </summary>
		public override void DataBind()
		{
			// nepoužíváme override PerformDatabinding, protože to dostává dataSource, který je vždy null (nic nám tedy nepřinese).
			Contract.Assert(!String.IsNullOrEmpty(DataFilterField));

			this.ClearSelection();
			Items.Clear();

			// přidání textu pro nerozhoduje
			string noFilterText = HttpUtilityExt.GetResourceString(NoFilterText);
			if (String.IsNullOrEmpty(noFilterText))
			{
				noFilterText = "---";
			}
			Items.Add(new ListItem(noFilterText, ""));

			// sesbírání a nastavení ostatních položek
			IDataItemContainer dataItemContainer = this.BindingContainer as IDataItemContainer;
			if (dataItemContainer != null)
			{
				string formatString = String.IsNullOrEmpty(DataTextFormatString) ? null : DataTextFormatString;
				object data = dataItemContainer.DataItem;
				if ((data != null) && (data is IEnumerable))
				{
					List<string> values;

					if (!String.IsNullOrEmpty(SortExpression))
					{
						// pokud máme SortExpression, data seřadíme podle tohoto výrazu

						SortExpressions sortExpressions = new SortExpressions();
						sortExpressions.AddSortExpression(SortExpression);
						IEnumerable sortedData = SortHelper.PropertySort((IEnumerable)data, sortExpressions.SortItems);

						// values vybereme ze seřazených dat
						// výsledek je stabilní vůči řazení, vč. metody Distinct
						values = ((IEnumerable)sortedData).Cast<object>()
							.Select(item => DataBinderExt.GetValue(item, this.DataFilterField))
							.Where(item => (item != null) && (item != DBNull.Value))
							.Select(item => (formatString == null) ? item.ToString() : String.Format(formatString, item))
							.Distinct()
							.ToList();
					}
					else
					{
						// pokud nemáme dáno pořadí, seřadíme automaticky až hodnoty pro DDL
						values = ((IEnumerable)data).Cast<object>()
							.Select(item => DataBinderExt.GetValue(item, this.DataFilterField))
							.Where(item => (item != null) && (item != DBNull.Value))
							.OrderBy(item => item)
							.Select(item => (formatString == null) ? item.ToString() : String.Format(formatString, item))
							.Distinct()
							.ToList();
					}

					for (int i = 0; i < values.Count; i++)
					{
						Items.Add(new ListItem(values[i], i.ToString()));
					}
				}
			}
		}
		#endregion
	}
}
