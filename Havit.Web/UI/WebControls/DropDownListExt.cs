using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Collections;
using System.Web.UI;
using System.Globalization;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Vylepšený <see cref="DropDownList"/>.
	/// </summary>
	public class DropDownListExt : DropDownList
	{
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
				bool flag = false;
				bool flag2 = false;
				string dataTextField = this.DataTextField;
				string dataValueField = this.DataValueField;
				string dataTextFormatString = this.DataTextFormatString;
				if (!this.AppendDataBoundItems)
				{
					this.Items.Clear();
				}
				ICollection @null = dataSource as ICollection;
				if (@null != null)
				{
					this.Items.Capacity = @null.Count + this.Items.Count;
				}
				if ((dataTextField.Length != 0) || (dataValueField.Length != 0))
				{
					flag = true;
				}
				if (dataTextFormatString.Length != 0)
				{
					flag2 = true;
				}
				foreach (object dataItem in dataSource)
				{
					ListItem item = new ListItem();
					if (flag)
					{
						if (dataTextField.Length > 0)
						{
							item.Text = DataBinderExt.GetValue(dataItem, dataTextField, dataTextFormatString);
						}
						if (dataValueField.Length > 0)
						{
							item.Value = DataBinderExt.GetValue(dataItem, dataValueField, null);
						}
					}
					else
					{
						if (flag2)
						{
							item.Text = string.Format(CultureInfo.CurrentCulture, dataTextFormatString, new object[] { dataItem });
						}
						else
						{
							item.Text = dataItem.ToString();
						}
						item.Value = dataItem.ToString();
					}
					this.Items.Add(item);
				}
			}
			if (this.cachedSelectedValue != null)
			{
				int num = -1;
				num = FindItemIndexByValue(this.Items, this.cachedSelectedValue);
				if (-1 == num)
				{
					throw new ArgumentOutOfRangeException("value", "DropDownList neobsahuje hodnotu SelectedValue.");
				}
				if ((this.cachedSelectedIndex != -1) && (this.cachedSelectedIndex != num))
				{
					throw new ArgumentException("Hodnoty SelectedValue a SelectedIndex se navzájem vyluèují.");
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
	}
}
