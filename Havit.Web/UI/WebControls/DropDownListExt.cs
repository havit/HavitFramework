using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Collections;
using System.Web.UI;
using System.Globalization;
using System.Linq;
using System.Web;
using Havit.Collections;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// Vylepšený <see cref="DropDownList"/>.
/// Podporuje lepší zpracování hodnoty DataTextField při databindingu a rozšiřuje možnost renderování o optgroups.
/// </summary>
/// <remarks>
/// Known issue:
/// Pokud je v jednom requestu nejprve nastaven SelectedIndex/SelectedValue a poté je proveden DataBind, pokusí se DataBind znovu vybrat položku, která byla vybrána.
/// Pokud se to nepovede (není nalezena), pak je vyhozena výjimka.
/// Není šance toto potlačit (a podle RH nemáme toto chování rušit, přestože nemám tušení, k čemu to je),
/// proto do ClearSelection doplňujeme vymazání příznaků, kterými se řídí DataBind.
/// </remarks>
public class DropDownListExt : DropDownListBaseExt
{
	/// <summary>
	/// Udává, zda je zapnuto automatické řazení položek při databindingu. Výchozí hodnota je false pro DropDownListExt, true pro EnterpriseDropDownList.
	/// </summary>
	public bool AutoSort
	{
		get { return (bool)(ViewState["AutoSort"] ?? false); }
		set { ViewState["AutoSort"] = value; }
	}

	/// <summary>
	/// Určuje, podle jaké property jsou řazena. Pokud není žádná hodnota nastavena použije se hodnota vlastnosti DataSortField a SortDirection.
	/// Může obsahovat více vlastností oddělených čárkou, směr řazení ASC/DESC. Má tedy význam podobný jako DefaultSortExpression u GridViewExt.
	/// </summary>
	public string SortExpression
	{
		get { return (string)(ViewState["SortExpression"] ?? ((DataOptionGroupField.Length > 0) ? DataOptionGroupField + ", " + DataTextField : DataTextField)); }
		set { ViewState["SortExpression"] = value; }
	}

	/// <summary>
	/// Binds the specified data source to the control that is derived from the <see cref="T:System.Web.UI.WebControls.ListControl"/> class.
	/// </summary>
	/// <param name="dataSource">An <see cref="T:System.Collections.IEnumerable"/> that represents the data source.</param>
	protected override void PerformDataBinding(IEnumerable dataSource)
	{
		if ((dataSource != null) && AutoSort)
		{
			if (String.IsNullOrEmpty(SortExpression))
			{
				throw new InvalidOperationException(String.Format("AutoSort je true, ale není nastavena hodnota vlastnosti SortExpression controlu {0}.", ID));
			}

			SortExpressions sortExpressions = new SortExpressions();
			sortExpressions.AddSortExpression(SortExpression);
			IEnumerable sortedData = SortHelper.PropertySort(dataSource, sortExpressions.SortItems);

			base.PerformDataBinding(sortedData);
		}
		else
		{
			base.PerformDataBinding(dataSource);
		}
	}
}
