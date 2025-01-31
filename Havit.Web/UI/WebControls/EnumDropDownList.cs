using System.Web.UI.WebControls;
using System.Web.UI;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// DropDownList pro práci s výčtovým datovým typem enum
/// </summary>
public class EnumDropDownList : DropDownListExt
{
	private const string EnumValueFormatString = "d";

	/// <summary>
	/// Indikuje, zda již došlo k navázání dat.
	/// </summary>
	private bool DataBindPerformed
	{
		get
		{
			return (bool)(ViewState["DataBindPerformed"] ?? false);
		}
		set
		{
			ViewState["DataBindPerformed"] = value;
		}
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
	private object delayedSetSelectedEnumValue = null;

	/// <summary>
	/// Udává, zda máme nastaven objekt pro odložené nastavení vybraného objektu.
	/// </summary>
	/// <remarks>
	/// Pokud nastavujeme SelectedObject během DataBindingu (ve stránce pomocí &lt;%# ... %&gt;),
	/// odloží se nastavení hodnoty až na konec DataBindingu. To protože v okamžiku nastavování SelectedObject 
	/// nemusí být v Items ještě data. 
	/// </remarks>
	private bool delayedSetSelectedEnumValueNeeded = false;

	/// <summary>
	/// Typ enum, který obsluhujeme.
	/// </summary>
	public Type EnumType
	{
		get
		{
			return _enumType;
		}
		set
		{
			if (!value.IsEnum)
			{
				throw new ArgumentException("Parametr musí být výčtovým typem.");
			}
			_enumType = value;

		}
	}
	private Type _enumType;

	/// <summary>
	/// Hodnota typu enum, která je nastavena DropDownListu
	/// </summary>
	public object SelectedEnumValue
	{
		set
		{
			if (isDataBinding)
			{
				// pokud jsme v databindingu, odložíme nastavení hodnoty, protože ještě nemusíme mít DataSource ani data v Items.
				delayedSetSelectedEnumValueNeeded = true;
				delayedSetSelectedEnumValue = value;
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
				if (value.GetType() != EnumType)
				{
					throw new ArgumentException("Hodnota není typu EnumType.", "value");
				}
				EnsureAutoDataBind();
				SelectedIndex = Items.IndexOf(Items.FindByValue(((Enum)value).ToString(EnumValueFormatString)));
			}
		}
		get
		{
			if (String.IsNullOrEmpty(this.SelectedValue))
			{
				return null;
			}
			else
			{
				if (this.EnumType == null)
				{
					throw new InvalidOperationException("Není nastavena vlastnost EnumType.");
				}
				return Enum.Parse(this.EnumType, this.SelectedValue);
			}
		}
	}

	/// <summary>
	/// Udává, zda má být na výběr prázdná hodnota. Výchozí hodnota je true.
	/// </summary>
	public bool Nullable
	{
		get
		{
			return (bool)(ViewState["Nullable"] ?? true);
		}
		set
		{
			ViewState["Nullable"] = value;
		}
	}

	/// <summary>
	/// Udává text prázdné hodnoty. Výchozí hodnota je "---".
	/// </summary>
	public string NullableText
	{
		get
		{
			return (string)(ViewState["NullableText"] ?? "---");
		}
		set
		{
			ViewState["NullableText"] = value;
		}
	}

	/// <summary>
	/// Format-string pro Text itemů. Lze skinovat a použít syntaxi $resources. Jako data jsou poskytnuta {0} = dataItem, {1} = EnumType.Name, {2} = EnumType.Namespace.
	/// </summary>
	/// <remarks>
	/// Hidujeme kvůli povolení skinování.
	/// </remarks>
	[Themeable(true)]
	public new string DataTextFormatString
	{
		get
		{
			return base.DataTextFormatString;
		}
		set
		{
			base.DataTextFormatString = value;
		}
	}

	/// <summary>
	/// Na EnumDropDownList není vlastnost podporována, vyhazuje výjimku NotSupportedException.
	/// </summary>
	/// <exception cref="NotSupportedException">Vždy, při jakémkoliv přístupu.</exception>
	public override string DataOptionGroupField
	{
		get
		{
			throw new NotSupportedException("EnumDropDownList nepodporuje DataOptionGroupField.");
		}
		set
		{
			throw new NotSupportedException("EnumDropDownList nepodporuje DataOptionGroupField.");
		}
	}

	/// <summary>
	/// Handles the <see cref="E:System.Web.UI.Control.Load"/> event.
	/// </summary>
	/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains event data.</param>
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		EnsureAutoDataBind();
	}

	/// <summary>
	/// Zajistí nabindování dat.
	/// </summary>
	private void EnsureAutoDataBind()
	{
		if (!this.DataBindPerformed)
		{
			DataBindAll();
		}
	}

	/// <summary>
	/// Provádí databinding a řeší odložené nastavení SelectedObject.
	/// </summary>
	public override void DataBind()
	{
		isDataBinding = true;
		base.DataBind();
		isDataBinding = false;

		if (delayedSetSelectedEnumValueNeeded)
		{
			SelectedEnumValue = delayedSetSelectedEnumValue;
			delayedSetSelectedEnumValueNeeded = false;
			delayedSetSelectedEnumValue = null;
		}
	}

	/// <summary>
	/// Naváže na DropDownList všechny hodnoty enumu.
	/// </summary>
	private void DataBindAll()
	{
		if (this.EnumType == null)
		{
			throw new InvalidOperationException("Není nastavena vlastnost EnumType");
		}
		PerformDataBinding(Enum.GetValues(this.EnumType));
	}

	/// <summary>
	/// Zajistí, aby byl po databindingu doplněn řádek pro výběr prázdné hodnoty.
	/// </summary>
	protected override void PerformDataBinding(System.Collections.IEnumerable dataSource)
	{
		base.PerformDataBinding(dataSource);

		if (dataSource != null)
		{
			if (this.Nullable)
			{
				EnsureEmptyItem();
				SelectedIndex = 0;
			}
			DataBindPerformed = true;
		}
	}

	/// <summary>
	/// Vytvoří ListItem, součást PerformDataBindingu.
	/// </summary>
	/// <param name="dataItem">The data item.</param>
	protected override ListItem CreateItem(object dataItem)
	{
		Enum enumDataItem = (Enum)dataItem;

		ListItem item = new ListItem();
		item.Value = enumDataItem.ToString(EnumValueFormatString);

		if (!String.IsNullOrEmpty(DataTextFormatString))
		{
			item.Text = HttpUtilityExt.GetResourceString(String.Format(DataTextFormatString, dataItem, EnumType.Name, EnumType.Namespace));
		}
		else
		{
			item.Text = enumDataItem.ToString();
		}

		return item;
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
}
