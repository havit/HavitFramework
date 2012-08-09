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

		#endregion

		#region Value properties

		#region Value
		[Themeable(false)]
		/// <summary>
		/// Vrací zadané datum. Není-li zadán žádný text, vrací null.
		/// Je-li zadán neplatný datum, vyhodí výjimku.
		/// </summary>
		public DateTime? Value
		{
			get
			{
				string text = valueTextBox.Text.Trim();
				if (String.IsNullOrEmpty(text))
				{
					return null;
				}

				return DateTime.Parse(text, Thread.CurrentThread.CurrentCulture.DateTimeFormat);
			}
			set
			{
				if (value == null || value == DateTime.MinValue)
				{
					valueTextBox.Text = String.Empty;
					return;
				}

				valueTextBox.Text = value.Value.ToShortDateString();
			}
		}
		#endregion.

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
				string text = valueTextBox.Text.Trim();

				if (text.Length == 0)
				{
					return true;
				}

				DateTime dt;
				return DateTime.TryParse(text, Thread.CurrentThread.CurrentCulture.DateTimeFormat, DateTimeStyles.None, out dt);
			}
		}
		#endregion

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
		///		<item>Změna formátu data: "1.1.2000" na "01.01.2001")</item> 
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

			seperatorLiteralControl = new LiteralControl("&nbsp");
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
			valueTextBox.TextChanged += new EventHandler(ValueTextBox_TextChanged);
			EnsureChildControls();
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

			valueTextBox.Enabled = IsEnabled;
			valueTextBox.AutoPostBack = AutoPostBack;

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
						valueTextBox.MaxLength = 10;
					}
					break;
				case DateTimeMode.DateTime:
					dateTimePickerDynarchCalendar.ShowsTime = true;
					if (valueTextBox.MaxLength == 0)
					{
						valueTextBox.MaxLength = 16;
					}
					break;
				default:
					throw new ApplicationException("Neznámý DateTimeMode.");
			}

			if (IsEnabled)
			{
				RegisterClientScript();
				valueTextBox.Attributes.Add("onKeyPress", "HavitDateTimeBox_KeyPress();");
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
		/// Zaregistruje klientské skripty pro omezení vstupu z klávesnice.
		/// </summary>
		private void RegisterClientScript()
		{
			string clientScriptBlockName = typeof(DateTimeBox).FullName + "_KeyPress";

			// ziskat ASCII kody oddelovacu datumu a casu
			System.Globalization.DateTimeFormatInfo dateTimeFormatInfo = CultureInfo.CurrentCulture.DateTimeFormat;
			string dateSeparatorCode = System.Text.Encoding.ASCII.GetBytes(dateTimeFormatInfo.DateSeparator)[0].ToString();
			#region JavaScript
			string javaScript = @"function HavitDateTimeBox_KeyPress() { event.returnValue = (event.keyCode >= 48 && event.keyCode <= 57) || (event.keyCode == " + dateSeparatorCode + @");	}";
			#endregion
			ScriptManager.RegisterClientScriptBlock(this.Page, typeof(DateTimeBox), "KeyPress", javaScript, true);
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
				return Value;
			}
			else
			{
				return DateTimeBox.InvalidValueMemento;
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
