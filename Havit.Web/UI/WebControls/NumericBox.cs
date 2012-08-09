using System;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Threading;

namespace Havit.DsvCommerce.WebBase.UI.WebControls
{
	/// <summary>
	/// NumericBox slouží k zadání èísla.
	/// </summary>
	[Themeable(true)]
	[ValidationProperty("NumberText")]	
	public class NumericBox : Control, INamingContainer
	{
		#region Constants
		private const string clientScriptBlockName = "Havit.DsvCommerce.WebBase.UI.WebControls.NumericBox_Script";
		private readonly Regex whitespaceremover = new Regex("\\s");
		private const string InvalidMemento = "invalid";
		#endregion

		#region Nested controls
		private TextBox valueTextBox;
		#endregion

		#region Behavior properties

		#region AutoPostBack
		/// <summary>
		/// Udává, zda má po zmìnì hodnoty v UI dojít k postbacku.
		/// </summary>
		public bool AutoPostBack
		{
			get { return (bool)(ViewState["AutoPostBack"] ?? false); }
			set { ViewState["AutoPostBack"] = value; }
		}
		#endregion

		#region Enabled
		/// <summary>
		/// Udává, zda je control pro zadání èísla povolen.
		/// Pokud je zakázán, není možné v UI zadávat hodnotu.
		/// </summary>
		public bool Enabled
		{
			get { return (bool)(ViewState["_Enabled"] ?? true); }
			set { ViewState["_Enabled"] = value; }
		}
		#endregion

		#region Decimals
		/// <summary>
		/// Nastavuje poèet desetinných míst, které lze v UI zadat. Na tento poèet desetinných míst se èíslo formátuje pro zobrazení.
		/// Výchozí hodnota je 0.
		/// </summary>
		public int Decimals
		{
			get { return (int)(ViewState["Decimals"] ?? 0); }
			set { ViewState["Decimals"] = value; }
		}

		#endregion

		#region AllowNegativeNumber
		/// <summary>
		/// Udává, zda je povoleno zadávat v UI záporná èísla (tj. znak "-").
		/// Výchozí hodnota je false.
		/// </summary>
		public bool AllowNegativeNumber
		{
			get { return (bool)(ViewState["AllowNegativeNumber"] ?? false); }
			set { ViewState["AllowNegativeNumber"] = value; }
		}

		#endregion

		#region ZeroAsEmpty
		/// <summary>
		/// Zobrazí editovací okno jako prázdné, pokud je v nastavena hodnota nula a naopak (prázdná hodnota je vracena jako nula).
		/// Výchozí hodnota je false.
		/// </summary>
		public bool ZeroAsEmpty
		{
			get { return (bool)(ViewState["ZeroAsEmpty"] ?? false); }
			set { ViewState["ZeroAsEmpty"] = value; }
		}
		#endregion

		#endregion

		#region Appereance properties
		#region Style
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

		#endregion		#endregion
		#endregion

		#region Function properties

		#region Value
		[Themeable(false)]
		/// <summary>
		/// Vrací zadané èíslo. Není-li zadán žádný text, vrací null (pokud je ZeroAsEmpty, vrací nulu).
		/// Je-li zadáno neplatné èíslo, vyhodí výjimku.
		/// </summary>
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
		#endregion

		#region ValueAsInt
		/// <summary>
		/// Vrací zadané èíslo jako Int32. Není-li zadán žádný text, vrací null (pokud je ZeroAsEmpty, vrací nulu).
		/// Je-li zadáno neplatné èíslo, vyhodí výjimku.
		/// </summary>
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
		}
		#endregion

		#region IsValid
		/// <summary>
		/// Vrací true, pokud obsahuje platné èíslo (tj. prázdnou hodnotu NEBO "validní" datum).
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

				// pokud nemùžeme èíslo pøevést na decimal, je to špatnì
				Decimal resultDecimal;
				return Decimal.TryParse(numberText, NumberStyles.Number, Thread.CurrentThread.CurrentCulture.NumberFormat, out resultDecimal);
			}
		}
		#endregion

		#region NumberText
		/// <summary>
		/// Hodnota zadaná v textovém políèku oøezanou o whitespaces (i z prostøedka textu, nikoliv prostý trim).
		/// Vlastnost není urèena pro zpracování, slouží pro validátory a pro parsování hodnoty na èíslo.
		/// </summary>
		public string NumberText
		{
			get
			{
				return whitespaceremover.Replace(valueTextBox.Text, String.Empty);
			}
		}
		#endregion
		#endregion

		#region ValueChanged
		/// <summary>
		/// Událost je vyvolána, kdykoliv uživatel zmìní editovanou hodnotu, resp. kdykoliv se po uživatelovì zásahu zmìní hodnota Value.
		/// (programová zmìna Value nevyvolá událost ValueChanged).
		/// Událost je vyvolána v situacích:
		/// <list>
		///		<item>hodnota 1 &lt;--^gt; hodnota 2 (napø. "1" na "2")</item> 
		///		<item>hodnota 1 &lt;--^gt; chybná hodnota (napø. "1" na "xx")</item> 
		///		<item>hodnota 1 &lt;--^gt; žádná hodnota (napø. "" na "xx")</item> 
		/// </list>
		/// Událost NENÍ vyvolána pøi zmìnì formátu hodnoty, napø.
		/// <list>
		///		<item>Zmìna formátu data: "1" na "01")</item> 
		///		<item>Èásteèná (neúplná) korekce chyby (napø. "xx1" na "x1")</item> 
		///		<item>Úprava prázdné hodnoty (napø. "" na "(mezera)" )</item>
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
		#endregion

		#region --------------------------------------------------------------------------------
		#endregion

		#region Constructor
		/// <summary>
		/// Kontruktor.
		/// </summary>
		public NumericBox()
		{
			valueTextBox = new TextBox();
			valueTextBox.ID = "ValueTextBox";
		}
		#endregion

		#region OnInit
		/// <summary>
		/// OnInit (overriden).
		/// </summary>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			valueTextBox.TextChanged += new EventHandler(ValueTextBox_TextChanged);
		}
		#endregion

		#region Controls
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
		#endregion

		#region CreateChildControls
		/// <summary>
		/// CreateChildControls (overriden).
		/// Vytvoøí kolekci controlù obsahující TextBox, LiteralControl (mezera), Image a na nìj navìšený DynarchCalendar.
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.Controls.Add(valueTextBox);
		}
		#endregion

		#region OnPreRender
		/// <summary>
		/// OnPreRender (overriden).
		/// </summary>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			valueTextBox.Enabled = this.Enabled;
			valueTextBox.Style.Add("text-align", "right");

			if (Enabled)
			{
				RegisterScripts();

				if (valueTextBox.MaxLength == 0)
				{
					valueTextBox.MaxLength = 12;
				}

				valueTextBox.Attributes.Add("onkeypress", String.Format("HavitNumericBox_KeyPress({0}, {1})", AllowNegativeNumber.ToString().ToLower(), Decimals));
				valueTextBox.Attributes.Add("onchange", String.Format("HavitNumericBox_Change({0}, {1})", AllowNegativeNumber.ToString().ToLower(), Decimals));
			}
		}		
		#endregion

		#region RegisterScripts
		/// <summary>
		/// Registruje klientské skripty omezující vstup z klávesnice.
		/// </summary>
		private void RegisterScripts()
		{
			char decimalSeparator = (Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]);

			string thousandsSeparator = (Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator);
			if (String.IsNullOrEmpty(thousandsSeparator) || (thousandsSeparator[0] == (char)160))
			{
				thousandsSeparator = " ";
			}
			else
			{
				thousandsSeparator = thousandsSeparator.Substring(0, 1);
			}

			string javaScript = @"
                    <script language=""JavaScript"">
                        function HavitNumericBox_KeyPress(allowNegativeNumber, decimals)
						{
	                        event.returnValue = (event.keyCode == " + (byte)thousandsSeparator[0] + @") 
								|| ((event.keyCode >= 48) && (event.keyCode <= 57))
		                        || (allowNegativeNumber && (event.keyCode == 45))
		                        || ((decimals > 0) && (event.keyCode == " + (byte)decimalSeparator + @") && event.srcElement.value.indexOf(String.fromCharCode(event.keyCode)) == -1);
                        }
                        
						function HavitNumericBox_Change(allowNegativeNumber, decimals)
						{
							value = event.srcElement.value;

							var position = value.indexOf('" + decimalSeparator + @"');
							if (position >= 0)
							{
								value = value.substr(0, position + decimals + 1);
								event.srcElement.value = value;
							}
                        }
                    </script>
				    ";

			ScriptManager.RegisterClientScriptBlock(this.Page, typeof(NumericBox), clientScriptBlockName, javaScript, false);
		}
		#endregion

		#region ClientID
		/// <summary>
		/// ClientID (overriden).
		/// Vrací ClientID obsaženého TextBoxu pro zadávání hodnoty.
		/// To øeší klientské validátory, které natrvdo pøedpokládají, že validovaný control (podle ClientID)
		/// obsahuje klientskou vlastnost "value". Tímto klientskému validátoru místo DateTimeBoxu podstrèíme nested TextBox.
		/// </summary>
		public override string ClientID
		{
			get
			{
				return valueTextBox.ClientID;
			}
		}
		#endregion

		#region ValueTextBox_TextChanged
		/// <summary>
		/// Obsluha zmìny textu v nested controlu.
		/// Ovìøuje, zda došlo ke zmìnì hodnoty a pokud ano, vyvolá prostøednictvím metody OnValueChanged událost ValueChanged.
		/// </summary>
		private void ValueTextBox_TextChanged(object sender, EventArgs e)
		{
			if (ViewState["ValueMemento"] != GetValueMemento())
			{
				OnValueChanged(EventArgs.Empty);
			}
		}
		#endregion

		#region GetValueMemento
		/// <summary>
		/// Metoda vrací editovanou hodnotu jako stav.
		/// Slouží k detekci, zda došlo ke zmìnì hodnoty mezi postbacky.
		/// </summary>
		private object GetValueMemento()
		{
			if (!IsValid)
			{
				return InvalidMemento;
			}
			else
			{
				return Value;
			}
		}
		#endregion

		#region OnValueChanged
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
		#endregion
	}
}