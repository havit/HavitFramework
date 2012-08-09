using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using Havit.Web.UI.WebControls;
using System.Web.UI;
using System.Threading;
using System.Globalization;

[assembly: WebResource("Havit.Web.UI.WebControls.DateTimeBox_DateTimePickerEnabled.gif", "image/gif")]
[assembly: WebResource("Havit.Web.UI.WebControls.DateTimeBox_DateTimePickerDisabled.gif", "image/gif")]

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// DateTimeBox slouží k zadání data/data a času pomocí TextBoxu a kalendáříku (DynarchCalendar).
	/// </summary>
	[ValidationProperty("ValueText")]
	[Themeable(true)]
	public class DateTimeBox : Control, INamingContainer
	{
		#region Private consts
		/// <summary>
		/// Slouží k reprezentaci chybné hodnoty v metodě GetValueMemento.
		/// </summary>
		private const string InvalidValueMemento = "invalid";
        private const string clientScriptBlockName = "Havit.DsvCommerce.WebBase.UI.WebControls.DateTimeBox_Script";
        #endregion

		#region Nested controls (private)

		private TextBox valueTextBox;
		private LiteralControl seperatorLiteralControl;
		private Image dateTimePickerImage;
		private DynarchCalendar dateTimePickerDynarchCalendar;

		#endregion

		#region Behavior properties

		#region AutoPostBack
		/// <summary>
		/// Udává, zda má po změně hodnoty V ui dojít k postbacku.
		/// </summary>
		public bool AutoPostBack
		{
			get { return (bool)(ViewState["AutoPostBack"] ?? false); }
			set { ViewState["AutoPostBack"] = value; }
		}
		#endregion

		#region Enabled
		/// <summary>
		/// Udává, zda je control pro výběr data/data a času povolen.
		/// Pokud je zakázán, není možné zadávat hodnotu ani v textboxu, ani pomocí kalendáře.
		/// </summary>
		public bool Enabled
		{
			get { return (bool)(ViewState["_Enabled"] ?? true); }
			set { ViewState["_Enabled"] = value; }
		}
		#endregion

		#region DateTimeMode
		/// <summary>
		/// Režim zobrazení data/data a času.
		/// </summary>
		public DateTimeMode DateTimeMode
		{
			get { return (DateTimeMode)(ViewState["DateTimeMode"] ?? DateTimeMode.Date); }
			set { ViewState["DateTimeMode"] = value; }
		}		
		#endregion

		#region KeyBlockingClientScriptEnabled
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
		#endregion 

		#endregion
		
		#region Appereance properties

		#region ShowDateTimePicker
		/// <summary>
		/// True, pokud má být zobrazen kalendář pro výběr data.
		/// </summary>
		public bool ShowDateTimePicker
		{
			get { return (bool)(ViewState["ShowDateTimePicker"] ?? true); }
			set { ViewState["ShowDateTimePicker"] = value; }
		}
		#endregion

		#region DateTimePickerStyle
		/// <summary>
		/// Styl obrázku (ikonky) pro zobrazení kalendáře.
		/// </summary>
		public Style DateTimePickerStyle
		{
			get
			{
				return dateTimePickerImage.ControlStyle;
			}
		}
		#endregion

		#region DateTimePickerEnabledImageUrl
		/// <summary>
		/// Url obrázku pro ikonku vyvolávající kalendář (pokud je DateTimeBox povolen).
		/// Pokud není Url nastavena, použije se výchozí obrázek z resources.
		/// </summary>
		public string DateTimePickerEnabledImageUrl
		{
			get { return (string)ViewState["DateTimePickerEnabledImageUrl"] ?? String.Empty; }
			set { ViewState["DateTimePickerEnabledImageUrl"] = value; }
		}
		#endregion

		#region DateTimePickerDisabledImageUrl
		/// <summary>
		/// Url obrázku pro ikonku vyvolávající kalendář (pokud je DateTimeBox zakázán).
		/// Pokud není Url nastavena, použije se výchozí obrázek z resources.
		/// </summary>
		public string DateTimePickerDisabledImageUrl
		{
			get { return (string)(ViewState["DateTimePickerDisabledImageUrl"] ?? String.Empty); }
			set { ViewState["DateTimePickerDisabledImageUrl"] = value; }
		}
		#endregion

		#region ValueBoxStyle
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
		#endregion

		#region TabIndex
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
		#endregion

		#region ToolTip
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
		#endregion

		#endregion

		#region Value properties

		#region Value
		/// <summary>
		/// Vrací zadané datum. Není-li zadán žádný text, vrací null.
		/// Je-li zadán neplatný datum, vyhodí výjimku.
		/// </summary>
		[Themeable(false)]
		public DateTime? Value
		{
			get
			{
				if (!IsValid)
				{
					throw new InvalidOperationException("Nelze číst Value, pokud IsValid je false.");
				}

//				DateTime? result = (DateTime?)ViewState["Value"];
				DateTime? result = (DateTime?)_value;
				// pokud jsme v řežimu zobrazení data bez času, nevracíme čas (mohl být zadaný setterem property Value nebo mohlo po postbacku dojít k přepnutí vlastnosti DateTimeMode)
				if ((result != null) && (DateTimeMode == DateTimeMode.Date)) 
				{
					return result.Value.Date;
				}
				return result;
			}
			set
			{
				SetValue(value, true);
			}
		}
		#endregion

		#region ValueText
		/// <summary>
		/// Hodnota zadaná v textovém políčku.
		/// Vlastnost není určena pro zpracování, slouží jen pro validátory.
		/// </summary>
		public string ValueText
		{
			get
			{
				return valueTextBox.Text;
			}
		}
		#endregion

		#region IsValid
		/// <summary>
		/// Vrací true, pokud obsahuje platné datum (tj. prázdnou hodnotu NEBO "validní" datum).
		/// </summary>
		public bool IsValid
		{
			get
			{
				return _isValid;
			}
		}
		#endregion

		private DateTime? _value;
		private bool _isValid = true;
		private bool _setValueCallsSetValueToNestedTextBox = false;

		#endregion

		#region ValueChanged
		/// <summary>
		/// Událost je vyvolána, kdykoliv uživatel změní editovanou hodnotu, resp. kdykoliv se po uživatelově zásahu změní hodnota Value.
		/// (programová změna Value nevyvolá událost ValueChanged).
		/// Událost je vyvolána v situacích:
		/// <list>
		///		<item>hodnota 1 &lt;--^gt; hodnota 2 (např. "1.1.2000" na "1.1.2001")</item> 
		///		<item>hodnota 1 &lt;--^gt; chybná hodnota (např. "1.1.2000" na "31.31.2000")</item> 
		///		<item>hodnota 1 &lt;--^gt; žádná hodnota (např. "" na "1.1.2000")</item> 
		/// </list>
		/// Událost NENÍ vyvolána při změně formátu hodnoty, např.
		/// <list>
		///		<item>Změna formátu data: "1.1.2000" na "01.01.2000")</item> 
		///		<item>Částečná (neúplná) korekce chyby (např. "50.50.2000" na "1.50.2000")</item> 
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
		private event EventHandler _valueChanged;
		#endregion

		#region ValidationGroup
		/// <summary>
		/// ValidationGroup pro validaci.
		/// </summary>
		public string ValidationGroup
		{
			get
			{
				return (string)ViewState["ValidationGroup"] ?? String.Empty;
			}
			set
			{
				ViewState["ValidationGroup"] = value;
			}
		}
		#endregion

		#region CausesValidation
		/// <summary>
		/// Určuje, zda dochází k validaci při postbacku způsobeným tímto controlem (autopostback).
		/// </summary>
		public bool CausesValidation
		{
			get
			{
				return (bool)(ViewState["CausesValidation"] ?? false);
			}
			set
			{
				ViewState["CausesValidation"] = value;
			}
		}
		#endregion

		#region --------------------------------------------------------------------------------
		#endregion

		#region Constructor
		/// <summary>
		/// Konstruktor. Vytvoří instance nested controlů.
		/// </summary>
		public DateTimeBox()
		{
			valueTextBox = new TextBox();
			valueTextBox.ID = "ValueTextBox";

			seperatorLiteralControl = new LiteralControl("&nbsp;");
			seperatorLiteralControl.ID = "SeparatorLiteralControl";

			dateTimePickerImage = new Image();
			dateTimePickerImage.ID = "DateTimePickerImage";
			
			dateTimePickerDynarchCalendar = new DynarchCalendar();
			dateTimePickerDynarchCalendar.ID = "DateTimePickerDynarchCalendar";
			dateTimePickerDynarchCalendar.Electric = false;
		}
		#endregion
		
		#region OnInit
		/// <summary>
		/// OnInit (overriden).
		/// </summary>
		protected override void OnInit(EventArgs e)
		{			
			base.OnInit(e);
			this.Page.PreLoad += new EventHandler(Page_PreLoad);			
			valueTextBox.TextChanged += new EventHandler(ValueTextBox_TextChanged);
			EnsureChildControls();
		}
		#endregion
		
		#region Page_PreLoad
		private void Page_PreLoad(object sender, EventArgs e)
		{
			if (ViewState["ValueMemento"] != null)
			{
				// pokud již alespoň jednou proběhl životní cyklus controlu (repeatery, gridy, databinding, apod.)
				// přečteme hodnotu z vnořeného textboxu
				SetValueFromNestedTextBox();
			}
		}
		#endregion

		#region OnLoad
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			// nastavíme hodnotu z předchozích kroků životního cyklu do vnořeného textboxu
			SetValueToNestedTextBox();
			this._setValueCallsSetValueToNestedTextBox = true;
		} 
		#endregion

		#region SetValueFromNestedTextBox
		/// <summary>
		/// Nastaví Value a IsValue na základě hodnoty ve vnořeném texboxu.
		/// </summary>
		private void SetValueFromNestedTextBox()
		{
			string text = valueTextBox.Text.Trim();
			if (String.IsNullOrEmpty(text))
			{
				SetValue(null, true);
			}
			else
			{
				DateTime dt;
				if (DateTime.TryParse(text, Thread.CurrentThread.CurrentCulture.DateTimeFormat, DateTimeStyles.None, out dt))
				{
					SetValue(dt, true);
				}
				else
				{
					SetValue(null, false);
				}
			}
		} 
		#endregion

		#region SetValueToNestedTextBox
		/// <summary>
		/// Nastaví hodnotu z Value do DateTimeBoxu.		
		/// </summary>
		private void SetValueToNestedTextBox()
		{
			if (IsValid)
			{
				DateTime? value = Value;
				if ((value == null) || (value == DateTime.MinValue))
				{
					valueTextBox.Text = String.Empty;
				}
				else
				{
					switch (this.DateTimeMode)
					{
						case DateTimeMode.Date: valueTextBox.Text = value.Value.ToShortDateString(); break;
						case DateTimeMode.DateTime: valueTextBox.Text = value.Value.ToString("g"); break;
						default: throw new ApplicationException("Neznámá hodnota DateTimeMode.");
					}
				}
			}
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
		/// Vytvoří kolekci controlů obsahující TextBox, LiteralControl (mezera), Image a na něj navěšený DynarchCalendar.
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			this.Controls.Add(valueTextBox);
			this.Controls.Add(seperatorLiteralControl);
			this.Controls.Add(dateTimePickerImage);
			this.Controls.Add(dateTimePickerDynarchCalendar);
		}
		#endregion

		#region OnPreRender
		/// <summary>
		/// OnPreRender (overriden).
		/// </summary>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			valueTextBox.Enabled = this.IsEnabled;
			valueTextBox.AutoPostBack = this.AutoPostBack;
			valueTextBox.ValidationGroup = this.ValidationGroup;
			valueTextBox.CausesValidation = this.CausesValidation;
		
			seperatorLiteralControl.Visible = ShowDateTimePicker;

			dateTimePickerImage.Visible = ShowDateTimePicker;

			#region Nastavení DateTimePickerImage.ImageUrl
			if (this.IsEnabled)
			{
				string url = DateTimePickerEnabledImageUrl;
				if (!String.IsNullOrEmpty(url))
				{
					dateTimePickerImage.ImageUrl = url;
				}
				else
				{
					dateTimePickerImage.ImageUrl = Page.ClientScript.GetWebResourceUrl(typeof(DateTimeBox), "Havit.Web.UI.WebControls.DateTimeBox_DateTimePickerEnabled.gif");
				}
			}
			else
			{
				string url = DateTimePickerDisabledImageUrl;
				if (!String.IsNullOrEmpty(url))
				{
					dateTimePickerImage.ImageUrl = url;
				}
				else
				{
					dateTimePickerImage.ImageUrl = Page.ClientScript.GetWebResourceUrl(typeof(DateTimeBox), "Havit.Web.UI.WebControls.DateTimeBox_DateTimePickerDisabled.gif");
				}
			}
			#endregion

			dateTimePickerDynarchCalendar.Enabled = IsEnabled;
			dateTimePickerDynarchCalendar.Visible = ShowDateTimePicker;
			dateTimePickerDynarchCalendar.InputField = "ValueTextBox";
			dateTimePickerDynarchCalendar.Button = "DateTimePickerImage";
			
			switch (DateTimeMode)
			{
				case DateTimeMode.Date: 
					dateTimePickerDynarchCalendar.ShowsTime = false;
					if (valueTextBox.MaxLength == 0)
					{
						valueTextBox.MaxLength = new DateTime(2000, 12, 31).ToShortDateString().Length;
					}
					break;
				case DateTimeMode.DateTime:
					dateTimePickerDynarchCalendar.ShowsTime = true;
					if (valueTextBox.MaxLength == 0)
					{
						valueTextBox.MaxLength = new DateTime(2000, 12, 31, 23, 59, 59, 999).ToString("g").Length;
					}
					break;
				default:
					throw new ApplicationException("Neznámý DateTimeMode.");
			}

			if (IsEnabled)
			{
				RegisterClientScript();
				if (KeyBlockingClientScriptEnabled)
				{
					valueTextBox.Attributes.Add("onkeypress", String.Format("HavitDateTimeBox_KeyPress(event, {0});", (this.DateTimeMode == DateTimeMode.DateTime).ToString().ToLower()));
				}
				//if (!ReadOnly)
				//{
				valueTextBox.Attributes.Add("onchange", "if (!HavitDateTimeBox_Change(event)) return false;");
				valueTextBox.Attributes.Add("onfocus", "HavitDateTimeBox_Focus(event);");
				//}
			}

			ViewState["ValueMemento"] = GetValueMemento();
		}
		#endregion

		#region Render
		/// <summary>
		/// Render (overriden).
		/// </summary>
		protected override void Render(HtmlTextWriter writer)
		{
			writer.WriteBeginTag("span");
			writer.WriteAttribute("style", "white-space: nowrap;");
			writer.Write(">");
			base.Render(writer);
			writer.WriteEndTag("span");
		}

		#endregion

		#region ClientID
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
		#endregion

		#region IsEnabled
		/// <summary>
		/// Vrací false, pokud je control sám zakázaný nebo pokud některý z parentů controlu je zakázaným WebControlem.
		/// Jinak vrací true.
		/// </summary>
		protected bool IsEnabled
		{
			get
			{
				if (!Enabled)
				{
					return false;
				}

				Control control = this;
				while (control != null) // projdeme od nás výše
				{
					if (control is WebControl) // pokud máme WebControl
					{
						if (!((WebControl)control).Enabled) // a WebControl je zakázaný  
						{
							return false; // vrátíme false
						}
					}
					control = control.Parent;
				}
				return true;
			}
		}
		#endregion

		#region RegisterClientScript
		/// <summary>
		/// Zaregistruje klientské skripty pro omezení vstupu z klávesnice, doplnění datumu podle masky
		/// </summary>
		private void RegisterClientScript()
		{
			// ziskat ASCII kody oddelovacu datumu a casu
			System.Globalization.DateTimeFormatInfo dateTimeFormatInfo = CultureInfo.CurrentCulture.DateTimeFormat;
			string dateSeparatorCode = System.Text.Encoding.ASCII.GetBytes(dateTimeFormatInfo.DateSeparator)[0].ToString();
			string timeSeparatorCode = System.Text.Encoding.ASCII.GetBytes(dateTimeFormatInfo.TimeSeparator)[0].ToString();
			string javaScript =
@"function HavitDateTimeBox_KeyPress(e, allowTime)
{
	var charCode = (window.event) ? window.event.keyCode : e.charCode;
" + ((dateTimeFormatInfo.DateSeparator == ".") ? "	if (window.event && (charCode == 44)) { charCode = 46; e.keyCode = 46; }\r\n" : "")
+ @"var validChar = ((charCode >= 48) && (charCode <= 57))
		|| (charCode == " + dateSeparatorCode + @")
		|| (charCode <= 31)
		|| (allowTime
			&& ((charCode == 32)
				|| (charCode == " + timeSeparatorCode + @")))
		|| (!window.event && e.ctrlKey && !e.altKey && !e.shiftKey && (charCode == 65 || charCode == 97 || charCode == 88 || charCode == 120 || charCode == 67 || charCode == 99 || charCode == 86 || charCode == 118));
	if (!validChar)
	{
		if (window.event) { window.event.returnValue = null; } else { e.preventDefault(); }
	}
}

function HavitDateTimeBox_Change(e)
{
	var element = (e && e.target) ? e.target : window.event.srcElement;
	var datum = new Date();
	var datumMesic = datum.getMonth() + 1;
	var datumRok = datum.getFullYear();
	var datumSeparator = '" + dateTimeFormatInfo.DateSeparator + @"';
	var regDen = '([1-9]|0[1-9]|[1-2]\\d|3[01])';
	var regDenCely = '(0[1-9]|[1-2]\\d|3[01])';
	var regMesic = '([1-9]|0[1-9]|1[0-2])';
	var regMesicCely = '(0[1-9]|1[0-2])';
	var regRok = '((19|20)\\d\\d)';
	var patterns = new Array
					(
							new Array(regDen + '(\\' + datumSeparator + ')?',
										'\$1' + datumSeparator + datumMesic + datumSeparator + datumRok
							),
							new Array(regDen + '(\\' + datumSeparator + ')' + regMesic + '(\\' + datumSeparator + ')?',
										'\$1' + datumSeparator + '\$3' + datumSeparator + datumRok
							),
							new Array(regDenCely + regMesicCely + '(\\' + datumSeparator + ')?',
										'\$1' + datumSeparator + '\$2' + datumSeparator + datumRok
							),
							new Array(regDenCely + '(\\' + datumSeparator + ')?' + regMesicCely + '(\\' + datumSeparator + ')?' + regRok,
										'\$1' + datumSeparator + '\$3' + datumSeparator + '\$5'
							)
					);
	var i = 0;
	var found = false;
	while (i < patterns.length && !found)
	{
		var regPattern = new RegExp('^' + patterns[i][0] + '$');
		var replacement = patterns[i][1];
		var value = element.value;
		if (regPattern.test(value)) 
		{
			var newValue = value.replace(regPattern, replacement);
			element.value = newValue
			if (value != newValue)
			{
				found = true;
			}
		}
		i++;
	}"
				// před výměnou hodnoty se aplikují validátory
	// potřebujeme zavolat druhé volání onchange, po výměně hodnoty, které validátory zase vypne
	// musíme ale zastavit propagaci události, protože jinak by se mohl volat dvakrát postback
	// v případě autopostbacku, proto return false
	+ @"
	if (found)
	{
		if (element.fireEvent)
		{
			element.fireEvent('onchange');
		}
		else
		{
			var evt = document.createEvent('HTMLEvents');
			evt.initEvent('change', true, true);
			element.dispatchEvent(evt);
		}
 		return false;
	}
	return true;
}

function HavitDateTimeBox_Focus(e)
{
	var element = (e && e.target) ? e.target : window.event.srcElement;
	if ((element != null) && element.createTextRange)
	{
		element.createTextRange().select();
	}
}";
            ScriptManager.RegisterClientScriptBlock(this.Page, typeof(DateTimeBox), clientScriptBlockName, javaScript, true);
		}
		#endregion

		#region ValueTextBox_TextChanged
		/// <summary>
		/// Obsluha změny textu v nested controlu.
		/// Ověřuje, zda došlo ke změně hodnoty a pokud ano, vyvolá prostřednictvím metody OnValueChanged událost ValueChanged.
		/// </summary>
		private void ValueTextBox_TextChanged(object sender, EventArgs e)
		{
			if (!Object.Equals(ViewState["ValueMemento"], GetValueMemento()))
			{
				OnValueChanged(EventArgs.Empty);
			}
		}
		#endregion

		#region GetValueMemento
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
				return DateTimeBox.InvalidValueMemento;
			}
		}
		#endregion

		#region SetValue
		/// <summary>
		/// Nastaví hodnoty vlastností Value a IsValid.
		/// </summary>
		protected void SetValue(DateTime? value, bool isValid)
		{
			this._value = value;
			this._isValid = isValid;		

			if (this._setValueCallsSetValueToNestedTextBox)
			{
				SetValueToNestedTextBox();
			}
		}
		#endregion

		#region OnValueChanged
		/// <summary>
		/// Vyvolává událost ValueChanged. Více viz ValueChanged.
		/// </summary>
		protected virtual void OnValueChanged(EventArgs e)
		{
			if (_valueChanged != null)
			{
				_valueChanged(this, e);
			}
		}
		#endregion
	}
}
