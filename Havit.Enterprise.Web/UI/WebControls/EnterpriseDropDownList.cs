using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Havit.Business;
using Havit.Collections;
using Havit.Web.UI.WebControls.ControlsValues;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// EnterpriseDropDownList zajišuje pohodlnìjší práci s DropDownListem, jeho prvky pøedstavují business objekty.	
	/// </summary>
	public class EnterpriseDropDownList : DropDownListExt
	{
		#region Constructors (static)
		static EnterpriseDropDownList()
		{
			Havit.Web.UI.WebControls.ControlsValues.PersisterControlExtenderRepository.Default.Add(new EnterpriseDropDownListPersisterControlExtender());
		} 
		#endregion
		
		#region ItemPropertyInfo
		/// <summary>
		/// ReferenceFieldPropertyInfo property, její hodnota se tímto DropDownListem vybírá.
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

		#region Nullable
		/// <summary>
		/// Udává, zda má bıt na vıbìr prázdná hodnota. Vıchozí hodnota je true.
		/// </summary>
		public bool Nullable
		{
			get { return (bool)(ViewState["Nullable"] ?? true); }
			set { ViewState["Nullable"] = value; }
		}		
		#endregion

		#region NullableText
		/// <summary>
		/// Udává text prázdné hodnoty. Vıchozí hodnota je "---".
		/// </summary>
		public string NullableText
		{
			get { return (string)(ViewState["NullableText"] ?? "---"); }
			set { ViewState["NullableText"] = value; }
		}		
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

		#region SelectedId
		/// <summary>
		/// Vrací ID vybrané poloky. Není-li ádná poloka vybraná, vrací null.
		/// </summary>
		public int? SelectedId
		{
			get
			{
				return (String.IsNullOrEmpty(SelectedValue) ? (int?)null : int.Parse(SelectedValue));
			}
		}		
		#endregion

		#region SelectedObject
		/// <summary>
		/// Vrací objekt na základì vybrané poloky v DropDownListu. Objekt se získává metodou ve vlastnosti ItemObjectInfo.
		/// Není-li ádná poloka vybrána, vrací null.
		/// </summary>
		public BusinessObjectBase SelectedObject
		{
			get
			{
				if (itemObjectInfo == null)
					throw new InvalidOperationException("Není nastavena vlastnost ItemObjectInfo.");

				return (SelectedId == null) ? null : itemObjectInfo.GetObjectMethod(SelectedId.Value);
			}
			set
			{
				if (isDataBinding)
				{
					// pokud jsme v databindingu, odloíme nastavení hodnoty, protoe ještì nemusíme mít DataSource ani data v Items.
					ClearSelection(); // potøebujeme potlaèit chování v pøedkovi - cachedSelectedIndex a cachedSelectedValue (U tam mùe bıt nastavena hodnota, ale my chceme jinou, jene díky delayedXXX ji nastavíme a za chvilku. Take by nám to bez tohoto øádku mohlo padat.)
					delayedSetSelectedObjectSet = true;
					delayedSetSelectedObject = value;
					return;
				}

				if (value == null)
				{
					EnsureAutoDataBind(); // jinak následnı databinding zlikviduje vybranou hodnotu
					// pokud nastavujeme null, zajistime, aby existoval prazdny radek a vybereme jej
					EnsureEmptyItem();
					SelectedValue = "";
				}
				else
				{
					if (value.IsNew)
					{
						throw new ArgumentException("Nelze vybrat neuloenı objekt.");
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
						SelectedValue = newListItem.Value;
					}
				}
			}
		}		
		#endregion

		#region EnsureAutoDataBind
		/// <summary>
		/// Zajistí nabindování dat pro reit AutoDataBind.
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
		BusinessObjectBase delayedSetSelectedObject = null;

		/// <summary>
		/// Udává, zda máme nastaven objekt pro odloené nastavení vybraného objektu.
		/// </summary>
		/// <remarks>
		/// Pokud nastavujeme SelectedObject bìhem DataBindingu (ve stránce pomocí &lt;%# ... %&gt;),
		/// odloí se nastavení hodnoty a na konec DataBindingu. To protoe v okamiku nastavování SelectedObject 
		/// nemusí bıt v Items ještì data. 
		/// </remarks>
		bool delayedSetSelectedObjectSet = false;

		#region IsNullable
		/// <summary>
		/// Udává, zda je EDDL nullable, viz kód...
		/// </summary>
		private bool IsNullable
		{
			get
			{
				if (itemPropertyInfo != null)
				{
					return itemPropertyInfo.Nullable;
				}
				else
				{
					return Nullable;
				}
			}
		}
		#endregion

		#endregion

		#region ---------------------------------------------------------------------------------------------
		#endregion

		#region Constructor
		/// <summary>
		/// Inicialuze DataValueField na "ID".
		/// </summary>
		public EnterpriseDropDownList()
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
				SelectedObject = delayedSetSelectedObject;
				delayedSetSelectedObjectSet = false;
				delayedSetSelectedObject = null;
			}
		}
		#endregion

		#region DataBindAll
		/// <summary>
		/// Naváe na DropDownList všechny (nasmazané) business objekty urèitého typu
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

			CheckNullableConsistency();
			if (IsNullable)
			{
				EnsureEmptyItem();
				// SelectedIndex = 0;
			}
			DataBindPerformed = true;

		}		
		#endregion

		#region CheckNullableConsistency
		/// <summary>
		/// Ovìøí konzistentní zadání ItemPropertyInfo.Nullable a Nullable.
		/// 
		/// </summary>
		private void CheckNullableConsistency()
		{
			bool? nullable = (bool?)ViewState["Nullable"];
			if ((nullable != null) && (itemPropertyInfo != null))
			{
				if (itemPropertyInfo.Nullable != nullable)
				{
					throw new ApplicationException("Je-li nastavena hodnota ItemPropertyInfo a Nullable, musí bıt ItemPropertyInfo.Nullable a Nullable shodné. Nyní se liší.");
				}
			}
		}

		#endregion

		#region EnsureEmptyItem
		/// <summary>
		/// Pøidá na zaèátek seznamu øádek pro vıbìr prázdné hodnoty, pokud tam ji není.
		/// </summary>
		public void EnsureEmptyItem()
		{
			if ((Items.Count == 0) || (Items[0].Value != String.Empty))
			{
				Items.Insert(0, new ListItem(NullableText, String.Empty));
			}
		}
		#endregion

        /// <summary>
        /// Vybere objekt dle ID, pokud je objekt s tímto ID mezi daty.
        /// Pokud není, neprovede nic.
        /// Vrací true/false indikující, zda se podaøilo objekt vybrat.
        /// Metoda je urèena pro vnitøní implementaci ukládání hondot filtrù.
        /// </summary>
#warning Po pøesunu ukládání hodnot filtrù z DSV do frameworku udìlat metodu interní.
        public bool SelectObjectIfPresent(int? objectID)
        {

            if ((objectID == null) && Nullable)
            {
                EnsureAutoDataBind();
                EnsureEmptyItem();
                SelectedValue = "";
                return true;
            }

            if (objectID != null)
            {
                EnsureAutoDataBind();
                // pokud nastavujeme objekt
                ListItem listItem = Items.FindByValue(objectID.Value.ToString());
                if (listItem != null)
                {
                    // nastavovany objekt je v seznamu
                    SelectedValue = listItem.Value;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }   
	}
}
