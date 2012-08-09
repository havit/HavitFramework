using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// DataGrid rozšíøenı o další funkcionalitu:
	/// </summary>
	public class DataGridExt : System.Web.UI.WebControls.DataGrid
	{
		#region Data members (podle abecedy)
		/// <summary>
		/// Povoluje/zakazuje automatické zpracování události <see cref="DataGrid.CancelCommand"/>.
		/// </summary>
		/// <value><b>true</b>, pokud má bıt pouit <see cref="AutoCancelCommandHandler"/>, jinak <b>false</b>; Default je <b>true</b>.</value>
		public bool AutoHandleCancelCommand
		{
			get
			{
				object tmp = ViewState["AutoHandleCancelCommand"];
				if (tmp != null)
				{
					return (bool)tmp;
				}
				return true;
			}
			set
			{
				ViewState["AutoHandleCancelCommand"] = value;
			}
		}

		/// <summary>
		/// Povoluje/zakazuje automatické zpracování události <see cref="DataGrid.EditCommand"/>.
		/// </summary>
		/// <value><b>true</b>, pokud má bıt pouit <see cref="AutoEditCommandHandler"/>, jinak <b>false</b>; Default je <b>true</b>.</value>
		public bool AutoHandleEditCommand
		{
			get
			{
				object obj = ViewState["AutoHandleEditCommand"];
				if (obj != null)
				{
					return (bool)obj;
				}
				return true;
			}
			set
			{
				ViewState["AutoHandleEditCommand"] = value;
			}
		}

		/// <summary>
		/// Povoluje/zakazuje automatické zpracování události <see cref="DataGrid.PageIndexChanged"/>.
		/// </summary>
		/// <value><b>true</b>, pokud má bıt pouit <see cref="AutoPageIndexChangedHandler"/>, jinak <b>false</b>; Default je <b>true</b>.</value>
		public bool AutoHandlePageIndexChanged
		{
			get
			{
				object obj = ViewState["AutoHandlePageIndexChanged"];
				if (obj != null)
				{
					return (bool)obj;
				}
				return true;
			}
			set
			{
				ViewState["AutoHandlePageIndexChanged"] = value;
			}
		}

		/// <summary>
		/// Povoluje/zakazuje automatické zpracování události <see cref="DataGrid.SortCommand"/> ve vztahu k <see cref="SortExpression"/>.
		/// </summary>
		/// <value><b>true</b>, pokud má bıt pouit <see cref="AutoCancelCommandHandler"/>, jinak <b>false</b>; Default je <b>true</b>.</value>
		public bool AutoHandleSortCommand
		{
			get
			{
				object obj = ViewState["AutoHandleSortCommand"];
				if (obj != null)
				{
					return (bool)obj;
				}
				return true;
			}
			set
			{
				ViewState["AutoHandleSortCommand"] = value;
			}
		}

		/// <summary>
		/// SortExpression do DataView nebo SQL.
		/// Mìní se SortCommandem, ukládá do ViewState.
		/// Pro obousmìné sortìní je potøeba povolit ViewState !!!
		/// </summary>
		/// <value>Default je <see cref="String.Empty"/>.</value>
		public string SortExpression
		{
			get
			{
				string str = (string)ViewState["SortExpression"];
				if (str != null)
				{
					return str;
				}
				return String.Empty;
			}
			set
			{
				ViewState["SortExpression"] = value;
			}
		}

		/// <summary>
		/// CssClass symbolu sortìní do Header sloupce.
		/// </summary>
		/// <value>Default je <see cref="String.Empty"/>.</value>
		public string SortHeaderImageCssClass
		{
			get
			{
				string str = (string)ViewState["SortHeaderImageCssClass"];
				if (str != null)
				{
					return str;
				}
				return String.Empty;
			}
			set
			{
				ViewState["SortHeaderImageCssClass"] = value;
			}
		}

		/// <summary>
		/// URL obrázku symbolu vzestupného sortìní do Header sloupce.
		/// </summary>
		/// <value>Default je <see cref="String.Empty"/>.</value>
		public string SortHeaderImageUrlAsc
		{
			get
			{
				string str = (string)ViewState["SortImageUrlAsc"];
				if (str != null)
				{
					return str;
				}
				return String.Empty;
			}
			set
			{
				ViewState["SortImageUrlAsc"] = value;
			}
		}

		/// <summary>
		/// URL obrázku symbolu sestupného sortìní do Header sloupce.
		/// </summary>
		/// <value>Default je <see cref="String.Empty"/>.</value>
		public string SortHeaderImageUrlDesc
		{
			get
			{
				string str = (string)ViewState["SortHeaderImageUrlDesc"];
				if (str != null)
				{
					return str;
				}
				return String.Empty;
			}
			set
			{
				ViewState["SortHeaderImageUrlDesc"] = value;
			}
		}

		/// <summary>
		/// Povoluje/zakazuje validaci na pøíkazu Update 
		/// </summary>
		/// <value>Default je <see cref="String.Empty"/>.</value>
		public bool UpdateCausesValidation
		{
			get
			{
				object obj = ViewState["CausesValidation"];
				if (obj != null)
				{
					return (bool)obj;
				}
				return true;
			}
			set
			{
				ViewState["CausesValidation"] = value;
			}
		}

		#endregion

		#region DataBindRequest (event)
		/// <summary>
		/// Vyskytne se, kdy DataGridExt potøebuje pøebindovat data.
		/// </summary>
		/// <remarks>
		/// Událost je vyvolávána smart-extenzemi v situaci, kdy je potøeba na DataGrid znovu nabindovat data.
		/// Napø. po standardní obsluze událostí EditCommand, CancelCommand, PageIndexChanged, atp.
		/// </remarks>
		public event EventHandler DataBindRequest;

		/// <summary>
		/// Vyvolá událost <see cref="Havit.Web.UI.WebControls.DataGridExt.DataBindRequest"/>.
		/// </summary>
		/// <param name="e">data události</param>
		protected virtual void OnDataBindRequest(EventArgs e)
		{
			if (this.DataBindRequest != null)
			{
				this.DataBindRequest(this, e);
			}
		}
		#endregion

		#region OnSortCommand
		/// <summary>
		/// Default implementace sortìní.
		/// Pøi zapnutém ViewStatu zajišuje obousmìrné pøepínání SortExpression.
		/// Pøi vypnutém ViewStatu zajišuje pouze bìné pøepínání SortExpression.
		/// Lze zapnout/vypnout pomocí AutoHandleSortCommand.
		/// </summary>
		/// <param name="e">DataGirdSortCommandEventArgs</param>
		protected override void OnSortCommand(DataGridSortCommandEventArgs e)
		{
			if (this.AutoHandleSortCommand)
			{
				this.AutoSortCommandHandler(e);
			}
			base.OnSortCommand(e);
		}

		/// <summary>
		/// Automatickı handler SortCommandu.
		/// Mùe bıt pøepsán v dceøinné implementaci.
		/// </summary>
		/// <param name="e">args</param>
		protected virtual void AutoSortCommandHandler(DataGridSortCommandEventArgs e)
		{
			string[] previousSorts = this.SortExpression.Split(',');
			string[] wantedSorts = e.SortExpression.Split(',');

			if (wantedSorts.Length == 0)
			{
				this.SortExpression = String.Empty;
			}
			else if ((previousSorts.Length != wantedSorts.Length)
				|| (!this.EnableViewState)	// není-li pouitelnı ViewState, funguje jen jednosmìrnı sortìní
				|| (!Page.EnableViewState)	// není zøejmé, co se stane, kdy DG bude jako child-control s vypnutım ViewState
				|| (this.SortExpression == null)
				|| (this.SortExpression.Length == 0)) // rùznı poèet znamená rùzné sortìní
			{
				// jednosmìrné sortìní
				this.SortExpression = e.SortExpression;
			}
			else
			{
				// obousmìrné sortìní
				int length = wantedSorts.Length;
				for (int i = 0; i < length; i++) // porovnání sloek
				{
					string wpart = wantedSorts[i].ToLower();
					if (wpart.Trim().IndexOf(' ') != -1)
					{
						wpart = wpart.Substring(0, wpart.Trim().IndexOf(' '));
					}
					if (!previousSorts[i].ToLower().StartsWith(wpart))
					{
						this.SortExpression = e.SortExpression;
						return;
					}
				}
				this.SortExpression = String.Empty;
				foreach (string part in previousSorts)
				{
					string newPart = part;
					if (newPart.IndexOf(' ') < 0)
					{
						newPart = newPart + " asc";
					}
					newPart = newPart.Replace("desc", "!a!");
					newPart = newPart.Replace("asc", "desc");
					newPart = newPart.Replace("!a!", "asc");
					this.SortExpression = this.SortExpression + newPart + ",";
				}
				this.SortExpression = this.SortExpression.Trim(',');
			}

			this.OnDataBindRequest(EventArgs.Empty);
		}
		#endregion

		#region OnPageIndexChanged
		/// <summary>
		/// Zajišuje volání události PageIndexChanged.
		/// Pøi povoleném AutoHandlePageIndexChanged nejprve zavolá AutoPageIndexChangedHandler.
		/// </summary>
		/// <param name="e">DataGridPageChangedEventArgs</param>
		protected override void OnPageIndexChanged(DataGridPageChangedEventArgs e)
		{
			if (this.AutoHandlePageIndexChanged)
			{
				AutoPageIndexChangedHandler(e);
			}
			base.OnPageIndexChanged (e);
		}

		/// <summary>
		/// Automatickı handler - nastavuje this.CurrentPageIndex = e.NewPageIndex a DataBind().
		/// Lze pøepsat v odvozeném controlu.
		/// </summary>
		/// <param name="e">DataGridPageChangedEventArgs</param>
		protected virtual void AutoPageIndexChangedHandler(DataGridPageChangedEventArgs e)
		{
			this.CurrentPageIndex = e.NewPageIndex;
			this.OnDataBindRequest(EventArgs.Empty);
		}
		#endregion

		#region OnItemCreated
		/// <summary>
		/// Zajišuje volání události ItemCreated.
		/// Implementace pøidává do Headeru symboly smìru sortìní.
		/// </summary>
		/// <param name="e">args</param>
		protected override void OnItemCreated(DataGridItemEventArgs e)
		{
			DataGridItem item = e.Item;

			base.OnItemCreated (e);

			// vypnutí CausesValidation u Update pøíkazu
			if ((e.Item.ItemIndex == this.EditItemIndex) && (item.HasControls()))
			{
				for (int i = 0; i < item.Cells.Count; i++)
				{
					foreach (Control ctrl in item.Cells[i].Controls)
					{
						if ((ctrl is LinkButton) && (((LinkButton)ctrl).CommandName == DataGrid.UpdateCommandName))
						{
							((LinkButton)ctrl).CausesValidation = this.UpdateCausesValidation;
						}
						else if ((ctrl is Button) && (((Button)ctrl).CommandName == DataGrid.UpdateCommandName))
						{
							((Button)ctrl).CausesValidation = this.UpdateCausesValidation;
						}
					}
				}
			}

			// SortImage v Headeru
			if ((this.AllowSorting == true) && (item.ItemType == ListItemType.Header))
			{
				string se = this.SortExpression.ToLower().Replace("asc", "").Replace("desc", "").Replace(" ","");
				int colnum = -1;
				for (int i = 0; i < this.Columns.Count; i++)
				{
					if (this.Columns[i].SortExpression.ToLower().Replace("asc", "").Replace("desc", "").Replace(" ", "") == se)
					{
						colnum = i;
					}
				}
				if (colnum >= 0)
				{
					int iasc = this.SortExpression.ToLower().IndexOf(" asc");
					int idesc = this.SortExpression.ToLower().IndexOf(" desc");

					if ( (idesc > 0) && ((iasc < 0) || (idesc < iasc)) )
					{
						if ((this.SortHeaderImageUrlDesc != null) && (this.SortHeaderImageUrlDesc.Length > 0))
						{
							HtmlImage img = new HtmlImage();
							img.ID = "SortImg";
							img.Src = this.SortHeaderImageUrlDesc;
							img.Attributes["class"] = this.SortHeaderImageCssClass;
							img.Alt = "A";
							item.Cells[colnum].Controls.Add(img);
						}
					}
					else
					{
						if ((this.SortHeaderImageUrlAsc != null) && (this.SortHeaderImageUrlAsc.Length > 0))
						{
							HtmlImage img = new HtmlImage();
							img.ID = "SortImg";
							img.Src = this.SortHeaderImageUrlAsc;
							img.Attributes["class"] = this.SortHeaderImageCssClass;
							img.Alt = "A";
							item.Cells[colnum].Controls.Add(img);
						}
					}
				}
			}
		}
		#endregion

		#region OnEditCommand
		/// <summary>
		/// Zajistí volání obsluhy události EditCommand.
		/// Pokud je AutoHandleEditCommand true, provede default obsluhu AutoEditCommandHandler().
		/// </summary>
		/// <param name="e">DataGridCommandEventArgs</param>
		protected override void OnEditCommand(DataGridCommandEventArgs e)
		{
			if (this.AutoHandleEditCommand)
			{
				AutoEditCommandHandler(e);
			}
			base.OnEditCommand (e);
		}

		/// <summary>
		/// Default obsluha EditCommand události.
		/// </summary>
		/// <param name="e">DataGridCommandEventArgs</param>
		protected virtual void AutoEditCommandHandler(DataGridCommandEventArgs e)
		{
			this.EditItemIndex = e.Item.ItemIndex;
			this.OnDataBindRequest(EventArgs.Empty);
		}
		#endregion

		#region OnCancelCommand
		/// <summary>
		/// Zajistí volání obsluhy události CancelCommand.
		/// Pokud je AutoHandleCancelCommand true, provede default obsluhu AutoCancelCommandHandler().
		/// </summary>
		/// <param name="e">DataGridCommandEventArgs</param>
		protected override void OnCancelCommand(DataGridCommandEventArgs e)
		{
			if (this.AutoHandleCancelCommand)
			{
				AutoCancelCommandHandler(e);
			}
			base.OnCancelCommand(e);
		}

		/// <summary>
		/// Default obsluha EditCommand události.
		/// </summary>
		/// <param name="e">DataGridCommandEventArgs</param>
		protected virtual void AutoCancelCommandHandler(DataGridCommandEventArgs e)
		{
			this.EditItemIndex = -1;
			this.OnDataBindRequest(EventArgs.Empty);
		}
		#endregion
	}
}