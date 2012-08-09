using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using Havit.Business;
using System.Web.UI;
using Havit.Collections;
using System.Collections;

namespace Havit.Web.UI.WebControls
{
    /// <summary>
    /// EnterpriseListBox zajišuje pohodlnìjší práci s ListBoxem, jeho poloky pøedstavují business objekty.	
    /// </summary>
	public class EnterpriseListBox: ListBoxExt
	{
		#region ItemPropertyInfo
		/// <summary>
		/// ReferenceFieldPropertyInfo property, její hodnota se tímto ListBoxem vybírá.
		/// Nastavení této hodnoty rovnì pøepíše hodnoty vlastností ItemObjectInfo a Nullable.
		/// Hodnota této property nepøeívá postback.
		/// </summary>
		public ReferenceFieldPropertyInfo ItemPropertyInfo
		{
			get
			{
				return itemPropertyInfo;
			}
			set
			{
				if ((itemObjectInfo != null) && (itemObjectInfo != value.TargetObjectInfo))
				{
					throw new ArgumentException("Nekonzistence ItemPropertyInfo.TargetObjectInfo a ItemObjectInfo");
				}
				itemPropertyInfo = value;
				itemObjectInfo = itemPropertyInfo.TargetObjectInfo;
				//				Nullable = itemPropertyInfo.Nullable;
			}
		}
		private ReferenceFieldPropertyInfo itemPropertyInfo;
		#endregion

		#region ItemObjectInfo
		/// <summary>
		/// Udává metodu, kterou se získá objekt na základì ID.
		/// Hodnota vlastnosti je automaticky nastavena nastavením vlastnosti PropertyInfo.
		/// Hodnota vlastnosti nepøeívá postback.
		/// </summary>
		public ObjectInfo ItemObjectInfo
		{
			get
			{
				return itemObjectInfo;
			}
			set
			{
				if ((itemPropertyInfo != null) && (value != itemPropertyInfo.TargetObjectInfo))
				{
					throw new ArgumentException("Nekonzistence ItemPropertyInfo.TargetObjectInfo a ItemObjectInfo");
				}
				itemObjectInfo = value;
			}
		}
		private ObjectInfo itemObjectInfo;

		#endregion

		#region AutoSort
		/// <summary>
		/// Udává, zda je zapnuto automatické øazení poloek pøi databindingu. Vıchozí hodnota je true.
		/// </summary>
		public bool AutoSort
		{
			get { return (bool)(ViewState["AutoSort"] ?? true); }
			set { ViewState["AutoSort"] = value; }
		}
		#endregion

		#region AutoDataBind
		/// <summary>
		/// Udává, zda je zapnuto automatické nabindování poloek pøi prvním naètení stránky. Vıchozí hodnota je false.
		/// </summary>
		public bool AutoDataBind
		{
			get { return (bool)(ViewState["AutoDataBind"] ?? false); }
			set { ViewState["AutoDataBind"] = value; }
		}
		#endregion

		#region DataSortField
		/// <summary>
		/// Urèuje, podle jaké property jsou øazena. Pokud není ádná hodnota nastavena pouije se hodnota vlastnosti DataTextField.
		/// </summary>
		public string DataSortField
		{
			get { return (string)(ViewState["DataSortField"] ?? DataTextField); }
			set { ViewState["DataSortField"] = value; }
		}
		#endregion

		#region SortDirection
		/// <summary>
		/// Udává smìr øazení poloek.
		/// Vıchozí je vzestupné øazení (Ascending).
		/// </summary>
		public Havit.Collections.SortDirection SortDirection
		{
			get { return (Havit.Collections.SortDirection)(ViewState["SortDirection"] ?? Havit.Collections.SortDirection.Ascending); }
			set { ViewState["SortDirection"] = value; }
		}
		#endregion

		#region SelectedIds
		/// <summary>
		/// Vrací ID vybrané poloky. Není-li ádná poloka vybraná, vrací null.
		/// </summary>
		public int[] SelectedIds
		{
			get
			{
				List<int> result = new List<int>();
				foreach (ListItem item in this.Items)
				{
					if (item.Selected)
					{
						result.Add(int.Parse(item.Value));
					}
				}
				return result.ToArray();
			}
		}
		#endregion

		#region SelectedObjects
		/// <summary>
		/// Vrací vıèet s vybranımi objekty (na základì zaškrtnutí CheckBoxù). Objekt se získává metodou ve vlastnosti ItemObjectInfo.
		/// </summary>
		public IEnumerable SelectedObjects
		{
			get
			{
				if (itemObjectInfo == null)
				{
					throw new InvalidOperationException("Není nastavena vlastnost ItemObjectInfo.");
				}

				List<BusinessObjectBase> result = new List<BusinessObjectBase>();
				foreach (int id in SelectedIds)
				{
					result.Add(itemObjectInfo.GetObjectMethod(id));
				}
				return result;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}

				if (isDataBinding)
				{
					// pokud jsme v databindingu, odloíme nastavení hodnoty, protoe ještì nemusíme mít DataSource ani data v Items.
					delayedSetSelectedObjectSet = true;
					delayedSetSelectedObjects = value;
					return;
				}

				EnsureAutoDataBind(); // jinak následnı databinding zlikviduje vybranou hodnotu
				this.ClearSelection();

				foreach (object selectObject in value)
				{
					if (!(selectObject is BusinessObjectBase))
					{
						throw new ArgumentException("Data obsahují prvek, kterı není potomkem BusinessObjectBase.");
					}

					BusinessObjectBase businessObject = (BusinessObjectBase)selectObject;
					if (businessObject.IsNew)
					{
						throw new ArgumentException("Nelze vybrat neuloenı objekt.");
					}

					// pokud nastavujeme objekt
					ListItem listItem = Items.FindByValue(businessObject.ID.ToString());
					if (listItem != null)
					{
						// nastavovany objekt je v seznamu
						listItem.Selected = true;
					}
					else
					{
						ListItem newListItem = new ListItem();
						newListItem.Text = DataBinder.Eval(businessObject, DataTextField).ToString();
						newListItem.Value = DataBinder.Eval(businessObject, DataValueField).ToString();
						newListItem.Selected = true;
						Items.Add(newListItem);
					}
				}
			}
		}
		#endregion

		#region EnsureAutoDataBind
		/// <summary>
		/// Zajistí nabindování dat pro reim AutoDataBind.
		/// </summary>
		protected void EnsureAutoDataBind()
		{
			if (AutoDataBind && !DataBindPerformed)
			{
				DataBindAll();
			}
		}
		#endregion

		#region Private properties
		/// <summary>
		/// Indikuje, zda ji došlo k navázání dat.
		/// </summary>
		private bool DataBindPerformed
		{
			get { return (bool)(ViewState["DataBindPerformed"] ?? false); }
			set { ViewState["DataBindPerformed"] = value; }
		}

		/// <summary>
		/// Indikuje právì porobíhající databinding.
		/// </summary>
		bool isDataBinding = false;

		/// <summary>
		/// Objekt, kterı má bıt nastaven jako vybranı, ale jeho nastavení bylo odloeno.
		/// </summary>
		/// <remarks>
		/// Pokud nastavujeme SelectedObject bìhem DataBindingu (ve stránce pomocí &lt;%# ... %&gt;),
		/// odloí se nastavení hodnoty a na konec DataBindingu. To protoe v okamiku nastavování SelectedObject 
		/// nemusí bıt v Items ještì data.
		/// </remarks>
		IEnumerable delayedSetSelectedObjects = null;

		/// <summary>
		/// Udává, zda máme nastaven objekt pro odloené nastavení vybraného objektu.
		/// </summary>
		/// <remarks>
		/// Pokud nastavujeme SelectedObject bìhem DataBindingu (ve stránce pomocí &lt;%# ... %&gt;),
		/// odloí se nastavení hodnoty a na konec DataBindingu. To protoe v okamiku nastavování SelectedObject 
		/// nemusí bıt v Items ještì data. 
		/// </remarks>
		bool delayedSetSelectedObjectSet = false;

		#endregion

		#region Constructor
		/// <summary>
		/// Vytvoøí instanci EnterpriseCheckBoxList.
		/// </summary>
		public EnterpriseListBox()
		{
			DataValueField = "ID";
		} 
		#endregion

		#region OnLoad
		/// <summary>
		/// Pokud jde o první naètení stránky a není nastaveno AutoDataBind, zavolá DataBindAll.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			EnsureAutoDataBind();
		}
		#endregion

		#region DataBind
		/// <summary>
		/// Provádí databinding a øeší odloené nastavení SelectedObject.
		/// </summary>
		public override void DataBind()
		{
			// v pøípadì pouití z GridView (v EnterpriseGV bez AutoDataBind)
			// se vyvolá nejdøív DataBind a poté teprve OnLoad.
			// musíme proto zajistit naplnìní hodnot seznamu i zde
			EnsureAutoDataBind();

			isDataBinding = true;
			base.DataBind();
			isDataBinding = false;

			if (delayedSetSelectedObjectSet)
			{
				this.SelectedObjects = delayedSetSelectedObjects;
				delayedSetSelectedObjectSet = false;
				delayedSetSelectedObjects = null;
			}
		}
		#endregion

		#region DataBindAll
		/// <summary>
		/// Naváe na ListBox všechny (nasmazané) business objekty urèitého typu
		/// (zavolá metodu GetAll(), nastaví vısledek je jako DataSource a zavolá DataBind).
		/// </summary>
		protected void DataBindAll()
		{
			if (itemObjectInfo == null)
			{
				throw new InvalidOperationException("Není nastavena vlastnost ItemObjectInfo.");
			}

			PerformDataBinding(itemObjectInfo.GetAllMethod());
		}
		#endregion

		#region PerformDataBinding
		/// <summary>
		/// Zajistí, aby byl po databindingu doplnìn øádek pro vıbìr prázdné hodnoty.
		/// </summary>
		/// <param name="dataSource"></param>
		protected override void PerformDataBinding(System.Collections.IEnumerable dataSource)
		{
			if (String.IsNullOrEmpty(DataTextField))
			{
				throw new InvalidOperationException(String.Format("Není nastavena hodnota vlastnosti DataTextField controlu {0}.", ID));
			}

			if ((dataSource != null) && AutoSort)
			{
				if (String.IsNullOrEmpty(DataSortField))
				{
					throw new InvalidOperationException(String.Format("AutoSort je true, ale není nastavena hodnota vlastnosti DataSortField controlu {0}.", ID));
				}

				SortItemCollection sorting = new SortItemCollection();
				sorting.Add(new SortItem(this.DataSortField, this.SortDirection));
				IEnumerable sortedData = SortHelper.PropertySort(dataSource, sorting);

				base.PerformDataBinding(sortedData);
			}
			else
			{
				base.PerformDataBinding(dataSource);
			}

			DataBindPerformed = true;

		}
		#endregion

		#region SelectExistingItems
		/// <summary>
		/// Vybere objekt dle ID, pokud je objekt s tímto ID mezi daty.
		/// Pokud není, neprovede nic. Vıslední kolekce objektù nastaví vıbìr (SelectedObjects).
		/// Metoda je urèena pro vnitøní implementaci ukládání hodnot.
		/// </summary>
		/// <param name="objectIDs"></param>
		internal void SelectExistingItems(int[] objectIDs)
		{
			List<object> objectsList = new List<object>();

			EnsureAutoDataBind();

			foreach (int objectID in objectIDs)
			{
				// pokud se objectID nachází mezi prvkama ERBL, získáme objekt a pøidáme ho do kolekce
				if (Items.FindByValue(objectID.ToString()) != null)
				{
					Object obj = ItemObjectInfo.GetObjectMethod(objectID);
					objectsList.Add(obj);
				}
			}

			// Nastavení vybranıch poloek
			SelectedObjects = objectsList.ToArray();
		} 
		#endregion
	}
}
