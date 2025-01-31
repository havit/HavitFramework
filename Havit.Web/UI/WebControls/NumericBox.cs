using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using Havit.Web.UI.ClientScripts;

namespace Havit.Web.UI.WebControls;

/// <summary>
/// NumericBox slouží k zadání čísla.
/// </summary>
[Themeable(true)]
[ValidationProperty("NumberText")]
public class NumericBox : Control, INamingContainer
{
	private readonly Regex whitespaceremover = new Regex("\\s");
	private const string InvalidMemento = "invalid";

	private readonly TextBox valueTextBox;

	/// <summary>
	/// Udává, zda má po změně hodnoty v UI dojít k postbacku.
	/// </summary>
	public bool AutoPostBack
	{
		get { return valueTextBox.AutoPostBack; }
		set { valueTextBox.AutoPostBack = value; }
	}

	/// <summary>
	/// Udává, zda je control pro zadání čísla povolen.
	/// Pokud je zakázán, není možné v UI zadávat hodnotu.
	/// </summary>
	public bool Enabled
	{
		get { return (bool)(ViewState["_Enabled"] ?? true); }
		set { ViewState["_Enabled"] = value; }
	}

	/// <summary>
	/// Nastavuje počet desetinných míst, které lze v UI zadat. Na tento počet desetinných míst se číslo formátuje pro zobrazení.
	/// Výchozí hodnota je 0.
	/// </summary>
	public int Decimals
	{
		get { return (int)(ViewState["Decimals"] ?? 0); }
		set { ViewState["Decimals"] = value; }
	}

	/// <summary>
	/// Udává, zda je povoleno zadávat v UI záporná čísla (tj. znak "-").
	/// Výchozí hodnota je false.
	/// </summary>
	public bool AllowNegativeNumber
	{
		get { return (bool)(ViewState["AllowNegativeNumber"] ?? false); }
		set { ViewState["AllowNegativeNumber"] = value; }
	}

	/// <summary>
	/// Zobrazí editovací okno jako prázdné, pokud je v nastavena hodnota nula a naopak (prázdná hodnota je vracena jako nula).
	/// Výchozí hodnota je false.
	/// </summary>
	public bool ZeroAsEmpty
	{
		get { return (bool)(ViewState["ZeroAsEmpty"] ?? false); }
		set { ViewState["ZeroAsEmpty"] = value; }
	}

	/// <summary>
	/// Udává, zda lze hodnotu v textovém políčku editovat.
	/// </summary>
	public bool ReadOnly
	{
		get
		{
			return valueTextBox.ReadOnly;
		}
		set
		{
			valueTextBox.ReadOnly = value;
		}
	}

	/// <summary>
	/// Udává, zda se pro zapouzdřený textbox použije javascript, který brání vložení nepovolených znaků. Výchozí hodnota je true.
	/// </summary>
	public bool KeyBlockingClientScriptEnabled
	{
		get
		{
			return (bool)(ViewState["KeyBlockingClientScriptEnabled"] ?? true);
		}
		set
		{
			ViewState["KeyBlockingClientScriptEnabled"] = value;
		}
	}

	/// <summary>
	/// Indikuje, zda se při kliknutí do textboxu označí vepsaný text.
	/// Výchozí hodnota je true.
	/// </summary>
	public bool SelectOnClick
	{
		get
		{
			return (bool)(ViewState["SelectOnClick"] ?? true);
		}
		set
		{
			ViewState["SelectOnClick"] = value;
		}
	}

	/// <summary>
	/// Stylování ValueTextBoxu.
	/// </summary>
	public Style ValueBoxStyle
	{
		get
		{
			return valueTextBox.ControlStyle;
		}
	}

	/// <summary>
	/// Gets or sets the text displayed when the mouse pointer hovers over the Web server control.
	/// </summary>
	public string ToolTip
	{
		get
		{
			return valueTextBox.ToolTip;
		}
		set
		{
			valueTextBox.ToolTip = value;
		}
	}

	/// <summary>
	/// Maximální délka <strong>textu</strong> zapsatelná do NumericBoxu.
	/// </summary>
	public int MaxLength
	{
		get { return valueTextBox.MaxLength; }
		set { valueTextBox.MaxLength = value; }
	}

	/// <summary>
	/// Vrací zadané číslo. Není-li zadán žádný text, vrací null (pokud je ZeroAsEmpty, vrací nulu).
	/// Je-li zadáno neplatné číslo, vyhodí výjimku.
	/// </summary>
	[Themeable(false)]
	public decimal? Value
	{
		get
		{
			if (String.IsNullOrEmpty(NumberText))
			{
				return ZeroAsEmpty ? (decimal?)0 : null;
			}

			return Decimal.Parse(NumberText, Thread.CurrentThread.CurrentCulture.NumberFormat);
		}
		set
		{
			if ((value == null) || ((value == 0) && ZeroAsEmpty))
			{
				valueTextBox.Text = "";
			}
			else
			{
				valueTextBox.Text = value.Value.ToString("N" + Decimals.ToString());
			}
		}
	}

	/// <summary>
	/// Vrací zadané číslo jako Int32. Není-li zadán žádný text, vrací null (pokud je ZeroAsEmpty, vrací nulu).
	/// Je-li zadáno neplatné číslo, vyhodí výjimku.
	/// </summary>
	[Themeable(false)]
	public int? ValueAsInt
	{
		get
		{
			decimal? currentValue = Value;
			if (currentValue == null)
			{
				return null;
			}
			return Convert.ToInt32(currentValue.Value);
		}
		set
		{
			this.Value = value;
		}
	}

	/// <summary>
	/// Vrací true, pokud obsahuje platné číslo (tj. prázdnou hodnotu NEBO "validní" číslo).
	/// </summary>
	public bool IsValid
	{
		get
		{
			string numberText = NumberText;

			if (numberText == String.Empty)
			{
				return true;
			}

			// pokud nemůžeme číslo převést na decimal, je to špatně
			Decimal resultDecimal;
			return Decimal.TryParse(numberText, NumberStyles.Number, Thread.CurrentThread.CurrentCulture.NumberFormat, out resultDecimal);
		}
	}

	/// <summary>
	/// Hodnota zadaná v textovém políčku ořezanou o whitespaces (i z prostředka textu, nikoliv prostý trim).
	/// Vlastnost není určena pro zpracování, slouží pro validátory a pro parsování hodnoty na číslo.
	/// </summary>
	public string NumberText
	{
		get
		{
			return whitespaceremover.Replace(valueTextBox.Text, String.Empty);
		}
	}

	/// <summary>
	/// Událost je vyvolána, kdykoliv uživatel změní editovanou hodnotu, resp. kdykoliv se po uživatelově zásahu změní hodnota Value.
	/// (programová změna Value nevyvolá událost ValueChanged).
	/// Událost je vyvolána v situacích:
	/// <list>
	///		<item>hodnota 1 &lt;--^gt; hodnota 2 (např. "1" na "2")</item> 
	///		<item>hodnota 1 &lt;--^gt; chybná hodnota (např. "1" na "xx")</item> 
	///		<item>hodnota 1 &lt;--^gt; žádná hodnota (např. "" na "xx")</item> 
	/// </list>
	/// Událost NENÍ vyvolána při změně formátu hodnoty, např.
	/// <list>
	///		<item>Změna formátu data: "1" na "01")</item> 
	///		<item>Částečná (neúplná) korekce chyby (např. "xx1" na "x1")</item> 
	///		<item>Úprava prázdné hodnoty (např. "" na "(mezera)" )</item>
	/// </list>
	/// </summary>
	public event EventHandler ValueChanged
	{
		add
		{
			_valueChanged += value;
		}
		remove
		{
			_valueChanged -= value;
		}
	}
	private EventHandler _valueChanged;

	/// <summary>
	/// Gets or sets the tab order of the control within its container.		
	/// </summary>
	public short TabIndex
	{
		get
		{
			return valueTextBox.TabIndex;
		}
		set
		{
			valueTextBox.TabIndex = value;
		}
	}

	/// <summary>
	/// ValidationGroup pro validaci.
	/// </summary>
	public string ValidationGroup
	{
		get
		{
			return valueTextBox.ValidationGroup;
		}
		set
		{
			valueTextBox.ValidationGroup = value;
		}
	}

	/// <summary>
	/// Určuje, zda dochází k validaci při postbacku způsobeným tímto controlem (autopostback).
	/// </summary>
	public bool CausesValidation
	{
		get
		{
			return valueTextBox.CausesValidation;
		}
		set
		{
			valueTextBox.CausesValidation = value;
		}
	}

	/// <summary>
	/// ClientID (overriden).
	/// Vrací ClientID obsaženého TextBoxu pro zadávání hodnoty.
	/// To řeší klientské validátory, které natrvdo předpokládají, že validovaný control (podle ClientID)
	/// obsahuje klientskou vlastnost "value". Tímto klientskému validátoru místo DateTimeBoxu podstrčíme nested TextBox.
	/// </summary>
	public override string ClientID
	{
		get
		{
			return valueTextBox.ClientID;
		}
	}

	/// <summary>
	/// Kontruktor.
	/// </summary>
	public NumericBox()
	{
		valueTextBox = new TextBoxExt();
		valueTextBox.ID = "ValueTextBox";
	}

	/// <summary>
	/// OnInit (overriden).
	/// </summary>
	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);
		valueTextBox.TextChanged += new EventHandler(ValueTextBox_TextChanged);
		EnsureChildControls();
	}

	/// <summary>
	/// Controls (overriden).
	/// </summary>
	public override ControlCollection Controls
	{
		get
		{
			EnsureChildControls();
			return base.Controls;
		}
	}

	/// <summary>
	/// CreateChildControls (overriden).
	/// </summary>
	protected override void CreateChildControls()
	{
		base.CreateChildControls();
		this.Controls.Add(valueTextBox);
	}

	/// <summary>
	/// OnPreRender (overriden).
	/// </summary>
	protected override void OnPreRender(EventArgs e)
	{
		base.OnPreRender(e);

		if (Enabled)
		{
			HavitFrameworkClientScriptHelper.RegisterHavitFrameworkClientScript(this.Page);
		}
		ViewState["ValueMemento"] = GetValueMemento();
	}

	/// <summary>
	/// Sends server control content to a provided HtmlTextWriter object, which writes the content to be rendered on the client.
	/// </summary>
	protected override void Render(HtmlTextWriter writer)
	{
		// nastavení se projeví pro Render, ale nejsou renderovány do ViewState

		valueTextBox.Enabled = this.Enabled;

		valueTextBox.Style.Add("text-align", "right");

		if (valueTextBox.MaxLength == 0)
		{
			valueTextBox.MaxLength = Math.Max(12, valueTextBox.Text.Length); // kdyby nějakým zázrakem byl nastaven delší text (stává se), zabráníme nemožnosti odeslat zobrazený control
		}

		if (Enabled)
		{
			string thousandsSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator.Left(1);
			if (String.IsNullOrEmpty(thousandsSeparator) || (thousandsSeparator[0] == (char)160))
			{
				thousandsSeparator = " ";
			}

			valueTextBox.Attributes["data-havitnumericbox"] = "true";
			valueTextBox.Attributes["data-havitnumericbox-allownegative"] = this.AllowNegativeNumber.ToString().ToLower();
			valueTextBox.Attributes["data-havitnumericbox-thousandsseparator"] = thousandsSeparator;
			valueTextBox.Attributes["data-havitnumericbox-decimals"] = this.Decimals.ToString();
			if (this.Decimals > 0)
			{
				valueTextBox.Attributes["data-havitnumericbox-decimalseparator"] = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.Left(1);
			}

			if (!KeyBlockingClientScriptEnabled)
			{
				valueTextBox.Attributes["data-havitnumericbox-suppressfilterkey"] = "true";
			}

			if (SelectOnClick)
			{
				valueTextBox.Attributes["data-havitnumericbox-selectonclick"] = "true";
			}

		}

		base.Render(writer);
	}

	/// <summary>
	/// Obsluha změny textu v nested controlu.
	/// Ověřuje, zda došlo ke změně hodnoty a pokud ano, vyvolá prostřednictvím metody OnValueChanged událost ValueChanged.
	/// </summary>
	private void ValueTextBox_TextChanged(object sender, EventArgs e)
	{
		if (ViewState["ValueMemento"] != GetValueMemento())
		{
			OnValueChanged(EventArgs.Empty);
		}
	}

	/// <summary>
	/// Metoda vrací editovanou hodnotu jako stav.
	/// Slouží k detekci, zda došlo ke změně hodnoty mezi postbacky.
	/// </summary>
	private object GetValueMemento()
	{
		if (IsValid)
		{
			return Value ?? (object)"null";
		}
		else
		{
			return InvalidMemento;
		}
	}

	/// <summary>
	/// Vyvolává událost ValueChanged. Více viz ValueChanged.
	/// </summary>
	private void OnValueChanged(EventArgs eventArgs)
	{
		if (_valueChanged != null)
		{
			_valueChanged(this, eventArgs);
		}
	}
}