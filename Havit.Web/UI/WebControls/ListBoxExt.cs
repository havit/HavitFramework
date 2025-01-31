using System.Web.UI.WebControls;
using System.Collections;
using System.Web.UI;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// Vylepšený <see cref="DropDownList"/>.
/// Podporuje lepší zpracování hodnoty DataTextField při databindingu.
/// </summary>
public class ListBoxExt : ListBox
{
	/// <summary>
	/// Událost, která se volá po vytvoření itemu a jeho data-bindingu.
	/// </summary>
	public event EventHandler<ListControlItemDataBoundEventArgs> ItemDataBound
	{
		add
		{
			Events.AddHandler(eventItemDataBound, value);
		}
		remove
		{
			Events.RemoveHandler(eventItemDataBound, value);
		}
	}
	private static readonly object eventItemDataBound = new object();

	private int cachedSelectedIndex = -1;
	private string cachedSelectedValue;

	/// <summary>
	/// Gets or sets the index of the selected item in the <see cref="T:System.Web.UI.WebControls.DropDownList"/> control.
	/// </summary>
	/// <value></value>
	/// <returns>The index of the selected item in the <see cref="T:System.Web.UI.WebControls.DropDownList"/> control. The default value is 0, which selects the first item in the list.</returns>
	public override int SelectedIndex
	{
		get
		{
			return base.SelectedIndex;
		}
		set
		{
			base.SelectedIndex = value;
			cachedSelectedIndex = value;
		}
	}

	/// <summary>
	/// Gets the value of the selected item in the list control, or selects the item in the list control that contains the specified value.
	/// </summary>
	/// <value></value>
	/// <returns>The value of the selected item in the list control. The default is an empty string ("").</returns>
	public override string SelectedValue
	{
		get
		{
			return base.SelectedValue;
		}
		set
		{
			base.SelectedValue = value;
			cachedSelectedValue = value;
		}
	}

	/// <summary>
	/// Gets or sets the field of the data source that provides the opting group content of the list items.
	/// </summary>
	public string DataOptionGroupField
	{
		get
		{
			return (string)(ViewState["DataOptionGroupField"] ?? String.Empty);
		}
		set
		{
			ViewState["DataOptionGroupField"] = value;
		}
	}

	/// <summary>
	/// Konstruktor. Nastavuje výchozí režim výběru na Multiple.
	/// </summary>
	public ListBoxExt()
	{
		SelectionMode = ListSelectionMode.Multiple;
	}

	/// <summary>
	/// Binds the specified data source to the control that is derived from the <see cref="T:System.Web.UI.WebControls.ListControl"/> class.
	/// </summary>
	/// <param name="dataSource">An <see cref="T:System.Collections.IEnumerable"/> that represents the data source.</param>
	protected override void PerformDataBinding(IEnumerable dataSource)
	{
		if (dataSource != null)
		{
			if (!this.AppendDataBoundItems)
			{
				this.Items.Clear();
			}
			ICollection @null = dataSource as ICollection;
			if (@null != null)
			{
				this.Items.Capacity = @null.Count + this.Items.Count;
			}
			foreach (object dataItem in dataSource)
			{
				ListItem item = CreateItem(dataItem);
				this.Items.Add(item);
				OnItemDataBound(new ListControlItemDataBoundEventArgs(item, dataItem));
			}
		}
		if (this.cachedSelectedValue != null)
		{
			int num = -1;
			num = FindItemIndexByValue(this.Items, this.cachedSelectedValue);
			if (-1 == num)
			{
				throw new InvalidOperationException("ListBoxEx neobsahuje hodnotu SelectedValue.");
			}
			if ((this.cachedSelectedIndex != -1) && (this.cachedSelectedIndex != num))
			{
				throw new ArgumentException("Hodnoty SelectedValue a SelectedIndex se navzájem vylučují.");
			}
			this.SelectedIndex = num;
			this.cachedSelectedValue = null;
			this.cachedSelectedIndex = -1;
		}
		else if (this.cachedSelectedIndex != -1)
		{
			this.SelectedIndex = this.cachedSelectedIndex;
			this.cachedSelectedIndex = -1;
		}
	}

	/// <summary>
	/// Vytvoří ListItem, součást PerformDataBindingu.
	/// </summary>
	protected virtual ListItem CreateItem(object dataItem)
	{
		return ListControlExtensions.CreateListItem(dataItem, DataTextField, DataTextFormatString, DataValueField, String.Empty /* CheckBoxList nepoužívá OptionGroups */);
	}
	/// <summary>
	/// Implementace nahrazující internal metody ListItemCollection.FindByValueInternal()
	/// </summary>
	/// <param name="listItemCollection">prohledávaná ListItemCollection</param>
	/// <param name="value">hledaná hodnota</param>
	private int FindItemIndexByValue(ListItemCollection listItemCollection, string value)
	{
		ListItem item = listItemCollection.FindByValue(value);
		if (item != null)
		{
			return listItemCollection.IndexOf(item);
		}
		return -1;
	}

	/// <summary>
	/// Raises the <see cref="ItemDataBound"/> event.
	/// </summary>
	/// <param name="e">The <see cref="Havit.Web.UI.WebControls.ListControlItemDataBoundEventArgs"/> instance containing the event data.</param>
	protected virtual void OnItemDataBound(ListControlItemDataBoundEventArgs e)
	{
		EventHandler<ListControlItemDataBoundEventArgs> h = (EventHandler<ListControlItemDataBoundEventArgs>)Events[eventItemDataBound];
		if (h != null)
		{
			h(this, e);
		}
	}

	/// <summary>
	/// Renderuje položky ListBoxu.
	/// Podporuje option groups.
	/// </summary>
	protected override void RenderContents(HtmlTextWriter writer)
	{
		// no base call
		ListControlExtensions.RenderContents(writer, this, this.Page, this.VerifyMultiSelect);
	}

	/// <summary>
	/// Uloží viewstate. Persistuje Option Groups položek.
	/// </summary>
	protected override object SaveViewState()
	{
		return ListControlExtensions.SaveViewState(base.SaveViewState, this.Items);
	}

	/// <summary>
	/// Načte viewstate vč. Option Groups položek.
	/// </summary>
	protected override void LoadViewState(object savedState)
	{
		ListControlExtensions.LoadViewState(savedState, base.LoadViewState, () => this.Items);
	}
}
