using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using Havit.Business;
using System.Web.UI;
using Havit.Collections;
using System.Collections;
using Havit.Diagnostics.Contracts;

namespace Havit.Web.UI.WebControls
{
    /// <summary>
    /// EnterpriseListBox zajišťuje pohodlnější práci s ListBoxem, jehož položky představují business objekty.	
    /// </summary>
	public class EnterpriseListBox : ListBoxExt
	{
		/// <summary>
		/// ReferenceFieldPropertyInfo property, jejíž hodnota se tímto ListBoxem vybírá.
		/// Nastavení této hodnoty rovněž přepíše hodnoty vlastností ItemObjectInfo a Nullable.
		/// Hodnota této property nepřežívá postback.
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

		/// <summary>
		/// Udává metodu, kterou se získá objekt na základě ID.
		/// Hodnota vlastnosti je automaticky nastavena nastavením vlastnosti PropertyInfo.
		/// Hodnota vlastnosti nepřežívá postback.
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

		/// <summary>
		/// Udává, zda je zapnuto automatické řazení položek při databindingu. Výchozí hodnota je true.
		/// </summary>
		public bool AutoSort
		{
			get { return (bool)(ViewState["AutoSort"] ?? true); }
			set { ViewState["AutoSort"] = value; }
		}

		/// <summary>
		/// Udává, zda je zapnuto automatické nabindování položek při prvním načtení stránky. Výchozí hodnota je false.
		/// </summary>
		public bool AutoDataBind
		{
			get { return (bool)(ViewState["AutoDataBind"] ?? false); }
			set { ViewState["AutoDataBind"] = value; }
		}

		/// <summary>
		/// Určuje, podle jaké property jsou řazena. Pokud není žádná hodnota nastavena použije se hodnota vlastnosti DataTextField.
		/// </summary>
		[Obsolete("Nahrazeno SortExpression.")]
		public string DataSortField
		{
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
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
		/// Udává směr řazení položek.
		/// Výchozí je vzestupné řazení (Ascending).
		/// </summary>
		[Obsolete("Nahrazeno SortExpression.")]
		public Havit.Collections.SortDirection SortDirection
		{
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
		}

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

		/// <summary>
		/// Vrací výčet s vybranými objekty (na základě zaškrtnutí CheckBoxů). Objekt se získává metodou ve vlastnosti ItemObjectInfo.
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
                Contract.Requires(value != null);

				if (isDataBinding)
				{
					// pokud jsme v databindingu, odložíme nastavení hodnoty, protože ještě nemusíme mít DataSource ani data v Items.
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
						if (DataOptionGroupField.Length > 0)
						{
							newListItem.SetOptionGroup(DataBinder.Eval(value, DataOptionGroupField).ToString());
						}
						newListItem.Selected = true;
						Items.Add(newListItem);
					}
				}
			}
		}

		/// <summary>
		/// Indikuje, zda již došlo k navázání dat.
		/// </summary>
		private bool DataBindPerformed
		{
			get { return (bool)(ViewState["DataBindPerformed"] ?? false); }
			set { ViewState["DataBindPerformed"] = value; }
		}

		/// <summary>
		/// Indikuje právě porobíhající databinding.
		/// </summary>
		private bool isDataBinding = false;

		/// <summary>
		/// Objekt, který má být nastaven jako vybraný, ale jeho nastavení bylo odloženo.
		/// </summary>
		/// <remarks>
		/// Pokud nastavujeme SelectedObject během DataBindingu (ve stránce pomocí &lt;%# ... %&gt;),
		/// odloží se nastavení hodnoty až na konec DataBindingu. To protože v okamžiku nastavování SelectedObject 
		/// nemusí být v Items ještě data.
		/// </remarks>
		private IEnumerable delayedSetSelectedObjects = null;

		/// <summary>
		/// Udává, zda máme nastaven objekt pro odložené nastavení vybraného objektu.
		/// </summary>
		/// <remarks>
		/// Pokud nastavujeme SelectedObject během DataBindingu (ve stránce pomocí &lt;%# ... %&gt;),
		/// odloží se nastavení hodnoty až na konec DataBindingu. To protože v okamžiku nastavování SelectedObject 
		/// nemusí být v Items ještě data. 
		/// </remarks>
		private bool delayedSetSelectedObjectSet = false;

		/// <summary>
		/// Vytvoří instanci EnterpriseCheckBoxList.
		/// </summary>
		public EnterpriseListBox()
		{
			DataValueField = "ID";
		}

		/// <summary>
		/// Pokud jde o první načtení stránky a není nastaveno AutoDataBind, zavolá DataBindAll.
		/// </summary>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			EnsureAutoDataBind();
		}

		/// <summary>
		/// Provádí databinding a řeší odložené nastavení SelectedObject.
		/// </summary>
		public override void DataBind()
		{
			// v případě použití z GridView (v EnterpriseGV bez AutoDataBind)
			// se vyvolá nejdřív DataBind a poté teprve OnLoad.
			// musíme proto zajistit naplnění hodnot seznamu i zde
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

		/// <summary>
		/// Naváže na ListBox všechny (nasmazané) business objekty určitého typu
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

		/// <summary>
		/// Zajistí, aby byl po databindingu doplněn řádek pro výběr prázdné hodnoty.
		/// </summary>
		protected override void PerformDataBinding(System.Collections.IEnumerable dataSource)
		{
			if (String.IsNullOrEmpty(DataTextField))
			{
				throw new InvalidOperationException(String.Format("Není nastavena hodnota vlastnosti DataTextField controlu {0}.", ID));
			}

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

			DataBindPerformed = true;

		}

		/// <summary>
		/// Vybere objekt dle ID, pokud je objekt s tímto ID mezi daty.
		/// Pokud není, neprovede nic. Výslední kolekce objektů nastaví výběr (SelectedObjects).
		/// Metoda je určena pro vnitřní implementaci ukládání hodnot.
		/// </summary>
		internal void SelectExistingItems(int[] objectIDs)
		{
			List<object> objectsList = new List<object>();

			EnsureAutoDataBind();

			foreach (int objectID in objectIDs)
			{
				// pokud se objectID nachází mezi prvkama ERBL, získáme objekt a přidáme ho do kolekce
				if (Items.FindByValue(objectID.ToString()) != null)
				{
					Object obj = ItemObjectInfo.GetObjectMethod(objectID);
					objectsList.Add(obj);
				}
			}

			// Nastavení vybraných položek
			SelectedObjects = objectsList.ToArray();
		}
	}
}
