using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Havit.Business;
using Havit.Collections;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// EnterpriseDropDownList zajišuje pohodlnìjší práci s DropDownListem, jeho prvky mají vazbu ne business objekty.	
	/// </summary>
	public class EnterpriseDropDownList : DropDownList
	{
		#region Constructor
		/// <summary>
		/// Inicialuze DataValueField na "ID".
		/// </summary>
		public EnterpriseDropDownList()
		{
			DataValueField = "ID";
		}
		#endregion

		#region Properties
		/// <summary>
		/// ReferenceFieldPropertyInfo property, její hodnota se tímto DropDownListem vybírá.
		/// Nastavení této hodnoty rovnì pøepíše hodnoty vlastností ItemsObjectInfo a Nullable.
		/// Hodnota této property nepøeívá postback.
		/// </summary>
		public ReferenceFieldPropertyInfo PropertyInfo
		{
			get
			{
				return propertyInfo;
			}
			set
			{
				propertyInfo = value;
				itemsObjectInfo = propertyInfo.TargetObjectInfo;
				Nullable = propertyInfo.Nullable;
			}
		}
		private ReferenceFieldPropertyInfo propertyInfo;

		/// <summary>
		/// Udává metodu, kterou se získá objekt na základì ID.
		/// Hodnota vlastnosti je automaticky nastavena nastavením vlastnosti PropertyInfo.
		/// Hodnota vlastnosti nepøeívá postback.
		/// </summary>
		public ObjectInfo ItemsObjectInfo
		{
			get
			{
				return itemsObjectInfo;
			}
			set
			{
				itemsObjectInfo = value;
			}
		}
		private ObjectInfo itemsObjectInfo;

		/// <summary>
		/// Udává, zda má bıt na vıbìr prázdná hodnota. Vıchozí hodnota je true.
		/// </summary>
		public bool Nullable
		{
			get { return (bool)(ViewState["Nullable"] ?? true); }
			set { ViewState["Nullable"] = value; }
		}

		/// <summary>
		/// Udává, zda je zapnuto automatické øazení poloek pøi databindingu. Vıchozí hodnota je true.
		/// </summary>
		public bool AutoSort
		{
			get { return (bool)(ViewState["AutoSort"] ?? true); }
			set { ViewState["AutoSort"] = value; }
		}

		/// <summary>
		/// Udává, zda je zapnuto automatické nabindování poloek pøi prvním naètení stránky. Vıchozí hodnota je false.
		/// </summary>
		public bool AutoDataBind
		{
			get { return (bool)(ViewState["AutoDataBind"] ?? false); }
			set { ViewState["AutoDataBind"] = value; }
		}

		/// <summary>
		/// Urèuje, podle jaké property jsou øazena. Pokud není ádná hodnota nastavena pouije se hodnota vlastnosti DataTextField.
		/// </summary>
		public string DataSortField
		{
			get { return (string)(ViewState["DataSortField"] ?? DataTextField); }
			set { ViewState["DataSortField"] = value; }
		}
		
		/// <summary>
		/// Vrací ID vybrané poloky. Není-li ádná poloka vybraná, vrací null.
		/// </summary>
		public int? SelectedId
		{
			get
			{
				return (SelectedValue == String.Empty) ? (int?)null : int.Parse(SelectedValue);
			}
		}

		/// <summary>
		/// Vrací objekt na základì vybrané poloky v DropDownListu. Objekt se získává metodou ve vlastnosti ItemsObjectInfo.
		/// Není-li ádná poloka vybrána, vrací null.
		/// </summary>
		public BusinessObjectBase SelectedObject
		{
			get
			{
				if (itemsObjectInfo == null)
					throw new InvalidOperationException("Není nastavena vlastnost ObjectInfo.");

				return (SelectedId == null) ? null : itemsObjectInfo.GetObjectMethod(SelectedId.Value);
			}
			set
			{
				if (value == null)
				{
					// pokud nastavujeme null, zajistime, aby existoval prazdny radek a vybereme jej
					EnsureEmptyItem();
					SelectedIndex = 0;
				}
				else
				{
					if (value.IsNew)
					{
						throw new ArgumentException("Nelze vybrat neuloenı objekt.");
					}

					// pokud nastavujeme objekt
					ListItem listItem = Items.FindByValue(value.ID.ToString());
					if (listItem != null)
					{
						// nastavovany objekt je v seznamu
						SelectedValue = listItem.Value;
					}
					else
					{
						// nastavovany objekt neni v seznamu, pridame jej
						bool oldAppendDataBoundItems = AppendDataBoundItems;
						AppendDataBoundItems = true;
						DataSource = new object[] { value };
						DataBind();
						AppendDataBoundItems = oldAppendDataBoundItems;
						SelectedIndex = Items.Count - 1;
					}
				}
			}
		}
		#endregion

		#region DataBinding

		/// <summary>
		/// Pokud jde o první naètení stránky a není nastaveno AutoDataBind, zavolá DataBindAll.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (!Page.IsPostBack && AutoDataBind)
			{
				DataBindAll();
			}
		}

		/// <summary>
		/// Naváe na DropDownList všechny (nasmazané) business objekty urèitého typu
		/// (zavolá metodu GetAll(), nastaví vısledek je jako DataSource a zavolá DataBind).
		/// </summary>
		protected void DataBindAll()
		{
			if (itemsObjectInfo == null)
				throw new InvalidOperationException("Není nastavena vlastnost ItemsObjectInfo.");

			DataSource = itemsObjectInfo.GetAllMethod();
			DataBind();
		}

		/// <summary>
		/// Zajistí, aby byl po databindingu doplnìn øádek pro vıbìr prázdné hodnoty.
		/// </summary>
		/// <param name="dataSource"></param>
		protected override void PerformDataBinding(System.Collections.IEnumerable dataSource)
		{
			if ((dataSource != null) && AutoSort)
			{
				IEnumerable sortedData = SortHelper.PropertySort(dataSource, DataSortField);
				base.PerformDataBinding(sortedData);
			}
			else
			{
				base.PerformDataBinding(dataSource);
			}
			if (Nullable)
			{
				EnsureEmptyItem();
			}
		}

		/// <summary>
		/// Pøidá na zaèátek seznamu øádek pro vıbìr prázdné hodnoty, pokud tam ji není.
		/// </summary>
		protected void EnsureEmptyItem()
		{
			if ((Items.Count == 0) || (Items[0].Value != String.Empty))
				Items.Insert(0, new ListItem("---", String.Empty));
		}
		#endregion
	}
}
