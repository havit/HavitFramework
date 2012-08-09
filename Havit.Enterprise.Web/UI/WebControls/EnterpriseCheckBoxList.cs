using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using Havit.Business;

namespace Havit.Enterprise.Web.UI.WebControls
{
	public class EnterpriseCheckBoxList: CheckBoxList
	{
		#region ItemObjectInfo
		/// <summary>
		/// Udává metodu, kterou se získá objekt na základì ID.
		/// Hodnota vlastnosti nepøežívá postback.
		/// </summary>
		public ObjectInfo ItemObjectInfo
		{
			get
			{
				return itemObjectInfo;
			}
			set
			{
				itemObjectInfo = value;
			}
		}
		private ObjectInfo itemObjectInfo;
		#endregion

		#region AutoSort
		/// <summary>
		/// Udává, zda je zapnuto automatické øazení položek pøi databindingu. Výchozí hodnota je true.
		/// </summary>
		public bool AutoSort
		{
			get { return (bool)(ViewState["AutoSort"] ?? true); }
			set { ViewState["AutoSort"] = value; }
		}
		#endregion

		#region AutoDataBind
		/// <summary>
		/// Udává, zda je zapnuto automatické nabindování položek pøi prvním naètení stránky. Výchozí hodnota je false.
		/// </summary>
		public bool AutoDataBind
		{
			get { return (bool)(ViewState["AutoDataBind"] ?? false); }
			set { ViewState["AutoDataBind"] = value; }
		}
		#endregion

		#region DataSortField
		/// <summary>
		/// Urèuje, podle jaké property jsou øazena. Pokud není žádná hodnota nastavena použije se hodnota vlastnosti DataTextField.
		/// </summary>
		public string DataSortField
		{
			get { return (string)(ViewState["DataSortField"] ?? DataTextField); }
			set { ViewState["DataSortField"] = value; }
		}
		#endregion		

		#region SelectedId
		/// <summary>
		/// Vrací ID vybrané položky. Není-li žádná položka vybraná, vrací null.
		/// </summary>
		public int? SelectedId
		{
			get
			{
				return (SelectedValue == String.Empty) ? (int?)null : int.Parse(SelectedValue);
			}
		}
		#endregion

		#region SelectedObjects
		/// <summary>
		/// Vrací objekt na základì vybrané položky v DropDownListu. Objekt se získává metodou ve vlastnosti ItemsObjectInfo.
		/// Není-li žádná položka vybrána, vrací null.
		/// </summary>
		public BusinessObjectBase[] SelectedObjects
		{
			get
			{
				if (itemObjectInfo == null)
				{
					throw new InvalidOperationException("Není nastavena vlastnost ItemObjectInfo.");
				}

				return (SelectedId == null) ? null : itemObjectInfo.GetObjectMethod(SelectedId.Value);
			}
			set
			{
				if (isDataBinding)
				{
					// pokud jsme v databindingu, odložíme nastavení hodnoty, protože ještì nemusíme mít DataSource ani data v Items.
					delayedSetSelectedObjectSet = true;
					delayedSetSelectedObject = value;
					return;
				}

				if (value == null)
				{
					EnsureAutoDataBind(); // jinak následný databinding zlikviduje vybranou hodnotu
					// pokud nastavujeme null, zajistime, aby existoval prazdny radek a vybereme jej
					EnsureEmptyItem();
					SelectedIndex = 0;
				}
				else
				{
					if (value.IsNew)
					{
						throw new ArgumentException("Nelze vybrat neuložený objekt.");
					}

					EnsureAutoDataBind();

					// pokud nastavujeme objekt
					ListItem listItem = Items.FindByValue(value.ID.ToString());
					if (listItem != null)
					{
						// nastavovany objekt je v seznamu
						SelectedValue = listItem.Value;
					}
					else
					{
						ListItem newListItem = new ListItem();
						newListItem.Text = DataBinder.Eval(value, DataTextField).ToString();
						newListItem.Value = DataBinder.Eval(value, DataValueField).ToString();
						Items.Add(newListItem);
						SelectedIndex = Items.Count - 1;
					}
				}
			}
		}
		#endregion

		#region DataBindPerformed
		/// <summary>
		/// Indikuje, zda již došlo k navázání dat.
		/// </summary>
		private bool DataBindPerformed
		{
			get { return (bool)(ViewState["DataBindPerformed"] ?? false); }
			set { ViewState["DataBindPerformed"] = value; }
		}
		#endregion

		#region Constructor
		public EnterpriseCheckBoxList()
		{
			DataValueField = "ID";
		}
		#endregion

		#region EnsureAutoDataBind
		/// <summary>
		/// Zajistí nabindování dat pro režit AutoDataBind.
		/// </summary>
		protected void EnsureAutoDataBind()
		{
			if (AutoDataBind && !DataBindPerformed)
			{
				DataBindAll();
			}
		}
		#endregion

	}
}
