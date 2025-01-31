using System.Web.UI;
using System.Web.UI.WebControls;
using Havit.Business;
using Havit.Web.UI.WebControls.ControlsValues;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// EnterpriseDropDownList zajišťuje pohodlnější práci s DropDownListem, jehož prvky představují business objekty.	
/// </summary>
public class EnterpriseDropDownList : DropDownListExt
{
	/// <summary>
	/// ReferenceFieldPropertyInfo property, jejíž hodnota se tímto DropDownListem vybírá.
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
	/// Udává, zda má být na výběr prázdná hodnota. Výchozí hodnota je true.
	/// </summary>
	public bool Nullable
	{
		get { return (bool)(ViewState["Nullable"] ?? true); }
		set { ViewState["Nullable"] = value; }
	}

	/// <summary>
	/// Udává text prázdné hodnoty. Výchozí hodnota je "---".
	/// </summary>
	public string NullableText
	{
		get { return (string)(ViewState["NullableText"] ?? "---"); }
		set { ViewState["NullableText"] = value; }
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
	/// Vrací ID vybrané položky. Není-li žádná položka vybraná, vrací null.
	/// </summary>
	public int? SelectedId
	{
		get
		{
			return (String.IsNullOrEmpty(SelectedValue) ? (int?)null : int.Parse(SelectedValue));
		}
	}

	/// <summary>
	/// Vrací objekt na základě vybrané položky v DropDownListu. Objekt se získává metodou ve vlastnosti ItemObjectInfo.
	/// Není-li žádná položka vybrána, vrací null.
	/// </summary>
	public BusinessObjectBase SelectedObject
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
				// pokud jsme v databindingu, odložíme nastavení hodnoty, protože ještě nemusíme mít DataSource ani data v Items.
				ClearSelection(); // potřebujeme potlačit chování v předkovi - cachedSelectedIndex a cachedSelectedValue (Už tam může být nastavena hodnota, ale my chceme jinou, jenže díky delayedXXX ji nastavíme až za chvilku. Takže by nám to bez tohoto řádku mohlo padat.)
				delayedSetSelectedObjectSet = true;
				delayedSetSelectedObject = value;
				return;
			}

			if (value == null)
			{
				EnsureAutoDataBind(); // jinak následný databinding zlikviduje vybranou hodnotu
									  // pokud nastavujeme null, zajistime, aby existoval prazdny radek a vybereme jej
				EnsureEmptyItem();
				SelectedValue = "";
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
					if (DataOptionGroupField.Length > 0)
					{
						newListItem.SetOptionGroup(DataBinder.Eval(value, DataOptionGroupField).ToString());
					}

					Items.Add(newListItem);
					SelectedValue = newListItem.Value;
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
	private BusinessObjectBase delayedSetSelectedObject = null;

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

	static EnterpriseDropDownList()
	{
		Havit.Web.UI.WebControls.ControlsValues.PersisterControlExtenderRepository.Default.Add(new EnterpriseDropDownListPersisterControlExtender());
	}

	/// <summary>
	/// Inicializuje DataValueField na "ID".
	/// </summary>
	public EnterpriseDropDownList()
	{
		DataValueField = "ID";
		AutoSort = true;
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
			SelectedObject = delayedSetSelectedObject;
			delayedSetSelectedObjectSet = false;
			delayedSetSelectedObject = null;
		}
	}

	/// <summary>
	/// Naváže na DropDownList všechny (nasmazané) business objekty určitého typu
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
	/// Zajistí nabindování dat pro režit AutoDataBind.
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

		base.PerformDataBinding(dataSource);

		CheckNullableConsistency();
		if (IsNullable)
		{
			EnsureEmptyItem();
			// SelectedIndex = 0;
		}
		DataBindPerformed = true;

	}

	/// <summary>
	/// Ověří konzistentní zadání ItemPropertyInfo.Nullable a Nullable.
	/// 
	/// </summary>
	private void CheckNullableConsistency()
	{
		bool? nullable = (bool?)ViewState["Nullable"];
		if ((nullable != null) && (itemPropertyInfo != null))
		{
			if (itemPropertyInfo.Nullable != nullable)
			{
				throw new ApplicationException("Je-li nastavena hodnota ItemPropertyInfo a Nullable, musí být ItemPropertyInfo.Nullable a Nullable shodné. Nyní se liší.");
			}
		}
	}

	/// <summary>
	/// Přidá na začátek seznamu řádek pro výběr prázdné hodnoty, pokud tam již není.
	/// </summary>
	public void EnsureEmptyItem()
	{
		if ((Items.Count == 0) || (Items[0].Value != String.Empty))
		{
			Items.Insert(0, new ListItem(NullableText, String.Empty));
		}
	}

	/// <summary>
	/// Vybere objekt dle ID, pokud je objekt s tímto ID mezi daty.
	/// Pokud není, neprovede nic.
	/// Vrací true/false indikující, zda se podařilo objekt vybrat.
	/// Metoda je určena pro vnitřní implementaci ukládání hondot filtrů.
	/// </summary>
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
