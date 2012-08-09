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
	public class EnterpriseCheckBoxList: CheckBoxListExt
	{
		#region ItemPropertyInfo
		/// <summary>
		/// ReferenceFieldPropertyInfo property, jejíž hodnota se tímto DropDownListem vybírá.
		/// Nastavení této hodnoty rovnìž pøepíše hodnoty vlastností ItemObjectInfo a Nullable.
		/// Hodnota této property nepøežívá postback.
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

		#region SortDirection
		/// <summary>
		/// Udává smìr øazení položek.
		/// Výchozí je vzestupné øazení (Ascending).
		/// </summary>
		public Havit.Collections.SortDirection SortDirection
		{
			get { return (Havit.Collections.SortDirection)(ViewState["SortDirection"] ?? Havit.Collections.SortDirection.Ascending); }
			set { ViewState["SortDirection"] = value; }
		}
		#endregion

		#region SelectedIds
		/// <summary>
		/// Vrací ID vybrané položky. Není-li žádná položka vybraná, vrací null.
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
		/// Vrací výèet s vybranými objekty (na základì zaškrtnutí CheckBoxù). Objekt se získává metodou ve vlastnosti ItemObjectInfo.
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
					// pokud jsme v databindingu, odložíme nastavení hodnoty, protože ještì nemusíme mít DataSource ani data v Items.
					delayedSetSelectedObjectSet = true;
					delayedSetSelectedObjects = value;
					return;
				}

				EnsureAutoDataBind(); // jinak následný databinding zlikviduje vybranou hodnotu
				this.ClearSelection();

				foreach (object selectObject in value)
				{
					if (!(selectObject is BusinessObjectBase))
					{
						throw new ArgumentException("Data obsahují prvek, který není potomkem BusinessObjectBase.");
					}

					BusinessObjectBase businessObject = (BusinessObjectBase)selectObject;
					if (businessObject.IsNew)
					{
						throw new ArgumentException("Nelze vybrat neuložený objekt.");
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
		/// Zajistí nabindování dat pro režim AutoDataBind.
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
		/// Indikuje, zda již došlo k navázání dat.
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
		/// Objekt, který má být nastaven jako vybraný, ale jeho nastavení bylo odloženo.
		/// </summary>
		/// <remarks>
		/// Pokud nastavujeme SelectedObject bìhem DataBindingu (ve stránce pomocí &lt;%# ... %&gt;),
		/// odloží se nastavení hodnoty až na konec DataBindingu. To protože v okamžiku nastavování SelectedObject 
		/// nemusí být v Items ještì data.
		/// </remarks>
		IEnumerable delayedSetSelectedObjects = null;

		/// <summary>
		/// Udává, zda máme nastaven objekt pro odložené nastavení vybraného objektu.
		/// </summary>
		/// <remarks>
		/// Pokud nastavujeme SelectedObject bìhem DataBindingu (ve stránce pomocí &lt;%# ... %&gt;),
		/// odloží se nastavení hodnoty až na konec DataBindingu. To protože v okamžiku nastavování SelectedObject 
		/// nemusí být v Items ještì data. 
		/// </remarks>
		bool delayedSetSelectedObjectSet = false;

		#endregion

		#region Constructor
		/// <summary>
		/// Vytvoøí instanci EnterpriseCheckBoxList.
		/// </summary>
		public EnterpriseCheckBoxList()
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
		/// Provádí databinding a øeší odložené nastavení SelectedObject.
		/// </summary>
		public override void DataBind()
		{
			// v pøípadì použití z GridView (v EnterpriseGV bez AutoDataBind)
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
		/// Naváže na DropDownList všechny (nasmazané) business objekty urèitého typu
		/// (zavolá metodu GetAll(), nastaví výsledek je jako DataSource a zavolá DataBind).
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
		/// Zajistí, aby byl po databindingu doplnìn øádek pro výbìr prázdné hodnoty.
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

		#region SelectObjectsIfPresent
		/// <summary>
		/// Vybere objekt dle ID, pokud je objekt s tímto ID mezi daty.
		/// Pokud není, neprovede nic. Výslední kolekce objektù nastaví výbìr (SelectedObjects).
		/// Metoda je urèena pro vnitøní implementaci ukládání hodnot.
		/// </summary>
		/// <param name="objectIDs"></param>
		public void SelectObjectsIfPresent(int[] objectIDs)
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

			// Nastavení vybraných položek
			SelectedObjects = objectsList.ToArray();
		} 
		#endregion
	}
}
