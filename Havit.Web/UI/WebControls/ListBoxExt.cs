using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Collections;
using System.Globalization;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Vylepšený <see cref="DropDownList"/>.
	/// Podporuje lepší zpracování hodnoty DataTextField při databindingu.
	/// </summary>
	public class ListBoxExt: ListBox
	{
		#region ItemDataBound (event)
		/// <summary>
		/// Událost, která se volá po vytvoření itemu a jeho data-bindingu.
		/// </summary>
		public event EventHandler<ListControlItemDataBoundEventArgs> ItemDataBound
		{
			add
			{
				base.Events.AddHandler(eventItemDataBound, value);
			}
			remove
			{
				base.Events.RemoveHandler(eventItemDataBound, value);
			}
		}
		private static readonly object eventItemDataBound = new object();
		#endregion

		#region SelectedIndex, SelectedValue (override)
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
		#endregion

		#region PerformDataBinding (override)
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
					throw new ArgumentOutOfRangeException("value", "ListBoxEx neobsahuje hodnotu SelectedValue.");
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
			bool flag = false;
			bool flag2 = false;
			if ((DataTextField.Length != 0) || (DataValueField.Length != 0))
			{
				flag = true;
			}
			if (DataTextFormatString.Length != 0)
			{
				flag2 = true;
			}
			ListItem item = new ListItem();
			if (flag)
			{
				if (DataTextField.Length > 0)
				{
					item.Text = DataBinderExt.GetValue(dataItem, DataTextField, DataTextFormatString);
				}
				if (DataValueField.Length > 0)
				{
					item.Value = DataBinderExt.GetValue(dataItem, DataValueField, null);
				}
			}
			else
			{
				if (flag2)
				{
					item.Text = string.Format(CultureInfo.CurrentCulture, DataTextFormatString, new object[] { dataItem });
				}
				else
				{
					item.Text = dataItem.ToString();
				}
				item.Value = dataItem.ToString();
			}
			return item;
		}

		/// <summary>
		/// Implementace nahrazující internal metody ListItemCollection.FindByValueInternal()
		/// </summary>
		/// <param name="listItemCollection">prohledávaná ListItemCollection</param>
		/// <param name="value">hledaná hodnota</param>
		/// <returns></returns>
		private int FindItemIndexByValue(ListItemCollection listItemCollection, string value)
		{
			ListItem item = listItemCollection.FindByValue(value);
			if (item != null)
			{
				return listItemCollection.IndexOf(item);
			}
			return -1;
		}
		#endregion

		#region OnItemDataBound
		/// <summary>
		/// Raises the <see cref="E:ItemDataBound"/> event.
		/// </summary>
		/// <param name="e">The <see cref="Havit.Web.UI.WebControls.ListControlItemDataBoundEventArgs"/> instance containing the event data.</param>
		protected virtual void OnItemDataBound(ListControlItemDataBoundEventArgs e)
		{
			EventHandler<ListControlItemDataBoundEventArgs> h = (EventHandler<ListControlItemDataBoundEventArgs>)base.Events[eventItemDataBound];
			if (h != null)
			{
				h(this, e);
			}
		}
		#endregion
	}
}
