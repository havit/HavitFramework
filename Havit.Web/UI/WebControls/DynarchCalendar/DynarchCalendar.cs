using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Threading;
using System.Diagnostics;

using Havit.Web.UI.ClientScripts;

[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar_stripped.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-setup_stripped.js", "text/javascript")]

#region WebResources - Languages
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-cs-utf8.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-cs-win.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-af.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-al.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-bg.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-br.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-ca.js", "text/javascript")]
//assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-cs.js", "text/javascript")
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-da.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-de.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-du.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-el.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-en.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-es.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-fi.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-fr.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-he.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-hr.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-hu.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-it.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-jp.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-ko.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-lt.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-lv.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-nl.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-no.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-pl.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-pt.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-ro.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-ru.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-si.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-sl.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-sk.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-sp.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-sv.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-tr.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-zh.js", "text/javascript")]
#endregion

#region WebResources - Styles
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-blue.css", "text/css")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-blue2.css", "text/css")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-brown.css", "text/css")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-green.css", "text/css")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-system.css", "text/css")]
#endregion

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Wrapper pro Dynarch DHTML/JavaScript Calendar
	/// http://www.dynarch.com/projects/calendar/
	/// </summary>
	/// <remarks>
	/// Properties odpovídají převážně vlastnostem nastavovacího objektu používaného v jscript:Calendar.setup()
	/// </remarks>
	/// <example>
	/// PopUp kalendář:
	/// <code>
	/// &lt;asp:TextBox ID="OdDateTB" Text="3.3.2004" Runat="server" /&gt;
	/// &lt;asp:Button ID="OdPickerBtn" Text="..." Runat="server" /&gt;
	/// &lt;havit:DynarchCalendar
	/// 	InputField = "OdDateTB"
	/// 	Button = "OdPickerBtn"
	/// 	ShowOthers = "true"
	/// 	Runat = "server" /&gt;
	/// </code>
	/// Flat kalendář:
	/// <code>
	/// &lt;span id="cal"&gt;&lt;/span&gt;
	/// &lt;havit:DynarchCalendar
	/// 	Flat = "cal"
	/// 	Date = "03/03/2004"
	/// 	Runat = "server" /&gt;
	/// </code>
	/// </example>
	public class DynarchCalendar : System.Web.UI.Control
	{		
		#region Static Properties
		/// <summary>
		/// Cesta k hlavnímu skriptu calendar[_stripped].js
		/// Prázdná hodnota způsobí použití skriptu přes WebResource.axd.
		/// </summary>
		public static string MainScriptUrl
		{
			get	{ return mainScriptUrl; }
			set { mainScriptUrl = value; }
		}
		private static string mainScriptUrl = String.Empty;

		/// <summary>
		/// Cesta k vybranému jazykovému skriptu calendar-{lang}.js
		/// Prázdná hodnota způsobí použití scriptu pro češtinu přes WebResource.axd, vybírá se skript podle kódování.
		/// </summary>
		public static string LanguageScriptUrl
		{
			get
			{
				return languageScriptUrl;
			}
			set
			{
				languageScriptUrl = value;
			}
		}
		private static string languageScriptUrl = String.Empty;

		/// <summary>
		/// Cesta k setup scriptu calendar-setup[_stripped].js
		/// Prázdná hodnota způsobí použití setup skriptu přes WebResource.axd.
		/// </summary>
		public static string SetupScriptUrl
		{
			get	{ return setupScriptUrl; }
			set { setupScriptUrl = value; }
		}
		private static string setupScriptUrl = String.Empty;

		/// <summary>
		/// Určuje, jaký kaskádový styl bude automaticky připojen do stránky.
		/// </summary>
		public static DynarchCalendarSkin Skin
		{
			get { return skin; }
			set { skin = value; }
		}
		private static DynarchCalendarSkin skin = DynarchCalendarSkin.System;

		private static readonly System.Collections.Generic.Dictionary<string, string> supportedLanguages;
		private static readonly object supportedLanguagesLock = new object();

		#endregion

		#region Originální vlastnosti kalendáře odpovídající JavaScript
		/// <summary>
		/// The ID of your input field.
		/// </summary>
		/// <remarks>
		/// Pokud je zadáno ID controlu, dohledá se ClientID od controlu,
		/// jinak je použito přímo.
		/// </remarks>
		public string InputField
		{
			get
			{
				string tmp = (string)ViewState["InputField"];
				if (tmp != null)
				{
					return tmp;
				}
				return string.Empty;
			}
			set
			{
				ViewState["InputField"] = value;
			}
		}

		/// <summary>
		/// This is the ID of a &gt;span&lt;, &gt;div&lt;, or any other element that you would like
		/// to use to display the current date. This is generally useful only if the input field is hidden,
		/// as an area to display the date.
		/// </summary>
		/// <remarks>
		/// Pokud je zadáno ID controlu, dohledá se ClientID od controlu,
		/// jinak je použito přímo.
		/// </remarks>
		public string DisplayArea
		{
			get
			{
				string tmp = (string)ViewState["DisplayArea"];
				if (tmp != null)
				{
					return tmp;
				}
				return String.Empty;
			}
			set
			{
				ViewState["DisplayArea"] = value;
			}
		}

		/// <summary>
		/// The ID of the calendar "trigger". This is an element (ordinarily a button or an image)
		/// that will dispatch a certain event (usually "click") to the function that creates
		/// and displays the calendar. 
		/// </summary>
		/// <remarks>
		/// Pokud je zadáno ID controlu, dohledá se ClientID od controlu,
		/// jinak je použito přímo.
		/// </remarks>
		public string Button
		{
			get
			{
				string tmp = (string)ViewState["Button"];
				if (tmp != null)
				{
					return tmp;
				}
				return String.Empty;
			}
			set
			{
				ViewState["Button"] = value;
			}
		}

		/// <summary>
		/// The name of the event that will trigger the calendar.
		/// The name should be without the "on" prefix, such as "click" instead of "onclick".
		/// Virtually all users will want to let this have the default value ("click").
		/// Anyway, it could be useful if, say, you want the calendar to appear when
		/// the input field is focused and have no trigger button
		/// (in this case use "focus" as the event name). 
		/// </summary>
		/// <remarks>
		/// Default hodnota "click" je určena v calendar-setup.js a zde je reprezentována
		/// prázdným řetězcem (nerenderovat nastavení tohoto parametru).
		/// </remarks>
		public string EventName
		{
			get
			{
				string tmp = (string)ViewState["EventName"];
				if (tmp != null)
				{
					return tmp;
				}
				return String.Empty;
			}
			set
			{
				ViewState["EventName"] = value;
			}
		}

		/// <summary>
		/// The format string that will be used to enter the date in the input field.
		/// This format will be honored even if the input field is hidden. 
		/// Default: "%d.%m.%Y" nastaven ve calendar-setup.js.
		/// </summary>
		/// <remarks>
		/// Default hodnota je určena v calendar-setup[_stripped].js a zde je reprezentována
		/// prázdným řetězcem (nerenderovat nastavení tohoto parametru).
		/// </remarks>
		public string InputFieldDateFormat
		{
			get
			{
				string tmp = (string)ViewState["InputFieldDateFormat"];
				if (tmp != null)
				{
					return tmp;
				}

				string pattern = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;

				if (ShowsTime)
				{
					pattern += " ";
					pattern += Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortTimePattern;
				}

				return TransformDatePatternToClientScript(pattern, Thread.CurrentThread.CurrentCulture.DateTimeFormat.DateSeparator, Thread.CurrentThread.CurrentCulture.DateTimeFormat.TimeSeparator);
			}
			set
			{
				ViewState["InputFieldDateFormat"] = value;
			}
		}

		/// <summary>
		/// Format of the date displayed in the displayArea (if specified).
		/// Default: "%d.%m.%Y" nastaven ve calendar-setup.js.
		/// </summary>
		/// <remarks>
		/// Default hodnota je určena v calendar-setup[_stripped].js a zde je reprezentována
		/// prázdným řetězcem (nerenderovat nastavení tohoto parametru).
		/// </remarks>
		public string DisplayAreaDateFormat
		{
			get
			{
				string tmp = (string)ViewState["DisplayAreaDateFormat"];
				if (tmp != null)
				{
					return tmp;
				}
				return String.Empty;
			}
			set
			{
				ViewState["DisplayAreaDateFormat"] = value;
			}
		}

		/// <summary>
		/// Wether the calendar is in "single-click mode" or "double-click mode".
		/// If true (the default) the calendar will be created in single-click mode. 
		/// </summary>
		/// <remarks>
		/// Renderuje se pouze false nastavení, true je default.
		/// </remarks>
		public bool SingleClick
		{
			get
			{
				object tmp = ViewState["SingleClick"];
				if (tmp != null)
				{
					return (bool)tmp;
				}
				return true;
			}
			set
			{
				ViewState["SingleClick"] = value;
			}
		}

		/// <summary>
		/// A function that receives a JS Date object and returns a boolean or a string.
		/// This function allows one to set a certain CSS class to some date, therefore making it look different.
		/// If it returns true then the date will be disabled. If it returns false nothing special
		/// happens with the given date. If it returns a string then that will be taken
		/// as a CSS class and appended to the date element. If this string is "disabled"
		/// then the date is also disabled (therefore is like returning true).
		/// </summary>
		/// <remarks>
		/// For more information please also refer to section 5.3.8 of calendar reference.
		/// </remarks>
		public string DateStatusFunction
		{
			get
			{
				string tmp = (string)ViewState["DateStatusFunction"];
				if (tmp != null)
				{
					return tmp;
				}
				return String.Empty;
			}
			set
			{
				ViewState["DateStatusFunction"] = value;
			}
		}		

		/// <summary>
		/// Specifies which day is to be displayed as the first day of week.
		/// Possible values are 0 to 6; 0 means Sunday, 1 means Monday, ..., 6 means Saturday.
		/// The end user can easily change this too, by clicking on the day name in the calendar header. 
		/// Default hodnotu nastavuje language script calendar-{lang}.js, zde reprezentováno jako -1 !!!
		/// </summary>
		/// <remarks>
		/// Default hodnota je určena v calendar-{lang}.js a zde je reprezentována
		/// hodnotou -1 (nerenderovat nastavení tohoto parametru).
		/// </remarks>
		public int FirstDay
		{
			get
			{
				object tmp = ViewState["FirstDay"];
				if (tmp != null)
				{
					return (int)tmp;
				}
				return -1;
			}
			set
			{
				ViewState["FirstDay"] = value;
			}
		}

		/// <summary>
		/// If "true" then the calendar will display week numbers. 
		/// </summary>
		/// <remarks>
		/// Renderuje se pouze false nastavení, true je default.
		/// </remarks>
		public bool WeekNumbers
		{
			get
			{
				object tmp = ViewState["WeekNumbers"];
				if (tmp != null)
				{
					return (bool)tmp;
				}
				return true;
			}
			set
			{
				ViewState["WeekNumbers"] = value;
			}
		}

		/// <summary>
		/// Alignment of the calendar, relative to the reference element.
		/// The reference element is dynamically chosen like this:
		/// if a displayArea is specified then it will be the reference element.
		/// Otherwise, the input field is the reference element.
		/// </summary>
		/// <remarks>
		/// VERTICAL - The first character in "align" can take one of the following values of VERTICAL alignment:
		/// T -- completely above the reference element (bottom margin of the calendar aligned to the top margin of the element).
		/// t -- above the element but may overlap it (bottom margin of the calendar aligned to the bottom margin of the element).
		/// c -- the calendar displays vertically centered to the reference element. It might overlap it (that depends on the horizontal alignment).
		/// b -- below the element but may overlap it (top margin of the calendar aligned to the top margin of the element).
		/// B -- completely below the element (top margin of the calendar aligned to the bottom margin of the element).
		/// HORIZONTAL - The second character in "align" can take one of the following values:
		/// L -- completely to the left of the reference element (right margin of the calendar aligned to the left margin of the element).
		/// l -- to the left of the element but may overlap it (left margin of the calendar aligned to the left margin of the element).
		/// c -- horizontally centered to the element. Might overlap it, depending on the vertical alignment.
		/// r -- to the right of the element but may overlap it (right margin of the calendar aligned to the right margin of the element).
		/// R -- completely to the right of the element (left margin of the calendar aligned to the right margin of the element).
		/// </remarks>
		public string Align
		{
			get
			{
				string tmp = (string)ViewState["Align"];
				if (tmp != null)
				{
					return tmp;
				}
				return String.Empty;
			}
			set
			{
				ViewState["Align"] = value;
			}
		}

		/// <summary>
		/// [startYear, endYear]
		/// The first element is the minimum year that is available,
		/// and the second element is the maximum year that the calendar will allow. 
		/// </summary>
		public string Range
		{
			get
			{
				string tmp = (string)ViewState["Range"];
				if (tmp != null)
				{
					return tmp;
				}
				return String.Empty;
			}
			set
			{
				ViewState["Range"] = value;
			}
		}

		/// <summary>
		/// If you want a flat calendar, pass the ID of the parent object in this property.
		/// </summary>
		/// <remarks>
		/// Resolvuje se při renderování - pokud ID patří controlu, nahradí se jeho ClientID.
		/// </remarks>
		public string Flat
		{
			get
			{
				string tmp = (string)ViewState["Flat"];
				if (tmp != null)
				{
					return tmp;
				}
				return String.Empty;
			}
			set
			{
				ViewState["Flat"] = value;
			}
		}

		/// <summary>
		/// This allows you to setup an initial date where the calendar will be positioned to.
		/// If absent then the calendar will open to the today date. 
		/// !!! POZOR: Pokud je navázáno na InputField, je brána hodnota odtamtud !!!
		/// </summary>
		public DateTime Date
		{
			get
			{
				object tmp = ViewState["Date"];
				if (tmp != null)
				{
					return (DateTime)tmp;
				}
				return DateTime.Today;
			}
			set
			{
				ViewState["Date"] = value;
			}
		}

		/// <summary>
		/// If this is set to true then the calendar will also allow time selection.
		/// Default: false.
		/// </summary>
		public bool ShowsTime
		{
			get
			{
				object tmp = ViewState["ShowsTime"];
				if (tmp != null)
				{
					return (bool)tmp;
				}
				return false;
			}
			set
			{
				ViewState["ShowsTime"] = value;
			}
		}

		/// <summary>
		/// Set this to 12 or 24 to configure the way that the calendar will display time. 
		/// Default: 24
		/// </summary>
		public int TimeFormat
		{
			get
			{
				object tmp = ViewState["TimeFormat"];
				if (tmp != null)
				{
					return (int)tmp;
				}
				return 24;
			}
			set
			{
				if ((value == 12) || (value == 24))
				{
					ViewState["TimeFormat"] = value;
				}
				else
				{
					throw new InvalidOperationException("Hodnota musí být 12 nebo 24.");
				}
			}
		}

		/// <summary>
		/// Set this to "false" if you want the calendar to update the field only when closed
		/// (by default it updates the field at each date change, even if the calendar is not closed) 
		/// </summary>
		public bool Electric
		{
			get
			{
				object tmp = ViewState["Electric"];
				if (tmp != null)
				{
					return (bool)tmp;
				}
				return true;
			}
			set
			{
				ViewState["Electric"] = value;
			}
		}

		/// <summary>
		/// Specifies the [x, y] position, relative to page's top-left corner, where the calendar will be displayed.
		/// If not passed then the position will be computed based on the "align" parameter.
		/// </summary>
		public string Position
		{
			get
			{
				string tmp = (string)ViewState["Position"];
				if (tmp != null)
				{
					return tmp;
				}
				return String.Empty;
			}
			set
			{
				ViewState["Position"] = value;
			}
		}

		/// <summary>
		/// Set this to "true" if you want to cache the calendar object.
		/// This means that a single calendar object will be used for all fields that require a popup calendar 
		/// Default: false.
		/// </summary>
		public bool CacheCalendar
		{
			get
			{
				object tmp = ViewState["CacheCalendar"];
				if (tmp != null)
				{
					return (bool)tmp;
				}
				return false;
			}
			set
			{
				ViewState["CacheCalendar"] = value;
			}
		}

		/// <summary>
		/// If set to "true" then days belonging to months overlapping with the currently displayed month
		/// will also be displayed in the calendar (but in a "faded-out" color)
		/// Default: true. (Na rozdíl od výchozího DC).
		/// </summary>
		public bool ShowOthers
		{
			get
			{
				object tmp = ViewState["ShowOthers"];
				if (tmp != null)
				{
					return (bool)tmp;
				}
				return true;
			}
			set
			{
				ViewState["ShowOthers"] = value;
			}
		}
		#endregion

		#region Přidané vlastnosti
		/// <summary>
		/// Enables/disables whole calendar.
		/// </summary>
		public bool Enabled
		{
			get
			{
				object tmp = ViewState["Enabled"];
				if (tmp != null)
				{
					return (bool)tmp;
				}
				return true;
			}
			set
			{
				ViewState["Enabled"] = value;
			}
		}

		#endregion		

		#region Constructor (static)
		static DynarchCalendar()
		{
			// inicializujeme kolekci podporovaných jazyků
			// protože jde o statický konstruktor, nemusíme řešit žádné zámky
			supportedLanguages = new System.Collections.Generic.Dictionary<string, string>();
			supportedLanguages.Add("af", null);
			supportedLanguages.Add("ak", null);
			supportedLanguages.Add("bg", null);
			supportedLanguages.Add("br", null);
			supportedLanguages.Add("ca", null);
			// cs má výjimky v kódu kvůli Win1250 + Utf8
			supportedLanguages.Add("da", null);
			supportedLanguages.Add("de", null);
			supportedLanguages.Add("du", null);
			supportedLanguages.Add("el", null);
			supportedLanguages.Add("en", null);
			supportedLanguages.Add("es", null);
			supportedLanguages.Add("fi", null);
			supportedLanguages.Add("fr", null);
			supportedLanguages.Add("he", null);
			supportedLanguages.Add("hr", null);
			supportedLanguages.Add("hu", null);
			supportedLanguages.Add("it", null);
			supportedLanguages.Add("jp", null);
			supportedLanguages.Add("ko", null);
			supportedLanguages.Add("lt", null);
			supportedLanguages.Add("lv", null);
			supportedLanguages.Add("nl", null);
			supportedLanguages.Add("no", null);
			supportedLanguages.Add("pl", null);
			supportedLanguages.Add("pt", null);
			supportedLanguages.Add("ro", null);
			supportedLanguages.Add("ru", null);
			supportedLanguages.Add("si", null);
			supportedLanguages.Add("sl", null);
			supportedLanguages.Add("sk", null);
			supportedLanguages.Add("sp", null);
			supportedLanguages.Add("sv", null);
			supportedLanguages.Add("tr", null);
			supportedLanguages.Add("zh", null);
		}
		#endregion

		#region ValidateControlProperties
		/// <summary>
		/// Zkontroluje, zda jsou vlastnosti controlu správně nastaveny a mohou být použity.
		/// </summary>
		/// <remarks>
		/// Zejména testuje, jestli jsou nastaveny všechny povinné vlastnosti.
		/// </remarks>
		protected virtual void ValidateControlProperties()
		{
			if ((this.InputField.Length == 0)
				&& (this.DisplayArea.Length == 0)
				&& (this.Button.Length == 0)
				&& (this.Flat.Length == 0))
			{
				throw new InvalidOperationException("Alespoň jedna z vlastností InputField, DisplayArea, Button nebo Flat musí být nastavena.");
			}
		}
		#endregion

		#region RegisterClientScript
		/// <summary>
		/// Zaregistruje klientské skripty kalendáře.
		/// </summary>
		protected virtual void RegisterClientScript()
		{
			ScriptManager.ScriptResourceMapping.EnsureScriptRegistration(this.Page, "jquery");
			this.RegisterMainScript();
			this.RegisterLanguageScript();
			this.RegisterSetupScript();
		}
		#endregion

		#region RegisterMainScript
		/// <summary>
		/// Zaregistruje hlavní klientský skript calendar.js.
		/// </summary>
		protected virtual void RegisterMainScript()
		{
			if (String.IsNullOrEmpty(DynarchCalendar.MainScriptUrl))
			{
				ScriptManager.RegisterClientScriptResource(this, typeof(DynarchCalendar), "Havit.Web.UI.WebControls.DynarchCalendar.calendar_stripped.js");
			}
			else			
			{
				ScriptManager.RegisterClientScriptInclude(this, typeof(DynarchCalendar), "DynarchCalendar.MainScriptUrl", this.ResolveUrl(MainScriptUrl));
			}
		}
		#endregion

		#region RegisterLanguageScript
		/// <summary>
		/// Zaregistruje klientský skript odpovídající jazykové mutace kalendáře,
		/// tj calendar-en.js, calendar-de.js, atp.
		/// </summary>
		protected virtual void RegisterLanguageScript()
		{
			if (String.IsNullOrEmpty(LanguageScriptUrl))
			{
				// ověříme response encoding
				// povoleno utf-8 a win-1250 pro češtinu
				if ((HttpContext.Current.Response.ContentEncoding != Encoding.UTF8) &&
					!((Thread.CurrentThread.CurrentUICulture.Name.Substring(0, 2) == "cs") && (HttpContext.Current.Response.ContentEncoding == Encoding.GetEncoding(1250))))
				{
					throw new ApplicationException("Response encoding must be UTF8 (or Windows-1250 for czech). Otherwise DynarchCalendar's javascripts won't work.");
				}
				
				if ((Thread.CurrentThread.CurrentUICulture.Name.Substring(0, 2) == "cs"))
				{
					if (HttpContext.Current.Response.ContentEncoding == Encoding.UTF8)
					{
						// čeština UTF-8
						ScriptManager.RegisterClientScriptResource(this, typeof(DynarchCalendar), "Havit.Web.UI.WebControls.DynarchCalendar.calendar-cs-utf8.js");
					}
					else
					{						
						// čeština Win-1250
						
						// POZOR! Pokud použiji ScriptManager na soubor uložený ve Windows-1250, nezobrazí se čeština správně.
						// Ovšem pokud použiji soubor uložený v UTF-8 při ResponseEncoding Windows-1250, čeština je správně!

						Page.ClientScript.RegisterClientScriptResource(typeof(DynarchCalendar), "Havit.Web.UI.WebControls.DynarchCalendar.calendar-cs-win.js");
						//ScriptManager.RegisterClientScriptResource(this, typeof(DynarchCalendar), "Havit.Web.UI.WebControls.DynarchCalendar.calendar-cs-win.js");
					}
				}
				else
				{
					string languageCode = Thread.CurrentThread.CurrentUICulture.Name.Substring(0, 2);
					bool isLanguageSupported;
					lock (supportedLanguagesLock)
					{
						isLanguageSupported = supportedLanguages.ContainsKey(languageCode);
					}

					if (isLanguageSupported)
					{
						// ostatni, utf-8
						ScriptManager.RegisterClientScriptResource(this, typeof(DynarchCalendar), "Havit.Web.UI.WebControls.DynarchCalendar.calendar-" + languageCode + ".js");
					}
					else
					{
						// nepodporovaný jazyk, použijeme angličtinu
						ScriptManager.RegisterClientScriptResource(this, typeof(DynarchCalendar), "Havit.Web.UI.WebControls.DynarchCalendar.calendar-en.js");
					}
				}
			}
			else
			{
				ScriptManager.RegisterClientScriptInclude(this, typeof(DynarchCalendar), "DynarchCalendar.LanguageScript", this.ResolveUrl(DynarchCalendar.LanguageScriptUrl));
			}
		}
		#endregion

		#region RegisterSetupScript
		/// <summary>
		/// Zaregistruje klientský setup-skript pro funkčnost Calendar.setup, tj calendar-setup.js.		
		/// </summary>
		protected virtual void RegisterSetupScript()
		{
			if (String.IsNullOrEmpty(SetupScriptUrl))
			{
				// pokud není URL skriptu zadáno, použijeme soubor z resources
				ScriptManager.RegisterClientScriptResource(this, typeof(DynarchCalendar), "Havit.Web.UI.WebControls.DynarchCalendar.calendar-setup_stripped.js");
			}
			else
			{
				ScriptManager.RegisterClientScriptInclude(this, typeof(DynarchCalendar), "DynarchCalendar.SetupScript", this.ResolveUrl(DynarchCalendar.SetupScriptUrl));
			}
		}
		#endregion

		#region CreateControlCollection (override)
		/// <summary>
		/// Creates a new ControlCollection object to hold the child controls
		/// (both literal and server) of the server control.
		/// </summary>
		/// <returns>new EmptyControlCollection()</returns>
		protected override System.Web.UI.ControlCollection CreateControlCollection()
		{
			return new EmptyControlCollection(this);
		}
		#endregion

		#region OnPreRender
		/// <summary>
		/// Raises the PreRender event.
		/// Zkontroluje platnost properties a zaregistruje klientské skripty kalendáře.
		/// </summary>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if (this.Enabled)
			{
				this.ValidateControlProperties();
				this.RegisterCss();
				this.RegisterClientScript();
			}
		}
		#endregion

		#region Render
		/// <summary>
		/// Emituje script pro inicializace komponenty DynarchCalendar.
		/// </summary>
		protected override void Render(HtmlTextWriter writer)
		{
			// Pokud je na GridView schován sloupec, tak schování probíhá až po OnPreRender. Proto musíme mít registraci skriptu až zde - pokud není DynarchCalendar zobrazen, neemitujeme skripty.
			// Ale nemůžeme sem dát vše, protože na registraci pomocí RegisterClientScriptResource je pozdě (možno vyzkoušet RegisterNamedResource).
			if (this.Enabled)
			{
				this.RegisterCalendarSetupScript();
			}
			base.Render(writer);
		}
		#endregion

		#region RegisterCss
		/// <summary>
		/// Zaregistruje css pro zobrazení kalendáře.
		/// </summary>
		protected void RegisterCss()
		{
			RegisterCalendarSkinStylesheets(this.Page);
		}
		#endregion

		#region RegisterCalendarSetupScript
		/// <summary>
		/// Emituje script nastavení kalendáře přes Calendar.setup(...).
		/// </summary>
		protected void RegisterCalendarSetupScript()
		{
			StringBuilder sb = new StringBuilder();		

			//writer.WriteLine("<script type=\"text/javascript\">");
			//writer.Indent++;
			sb.AppendLine("Calendar.setup({");
			//writer.Indent++;

			bool firstLine = true;
			if (this.InputField.Length > 0)
			{
				sb.AppendFormat("inputField : \"{0}\"", this.ResolveID(this.InputField));
				firstLine = false;
			}

			if (this.DisplayArea.Length > 0)
			{
				if (!firstLine)
				{
					sb.AppendLine(",");
				}
				sb.AppendFormat("displayArea : \"{0}\"", this.ResolveID(this.DisplayArea));
				firstLine = false;
			}

			if (this.Button.Length > 0)
			{
				if (!firstLine)
				{
					sb.AppendLine(",");
				}
				sb.AppendFormat("button : \"{0}\"", this.ResolveID(this.Button));
				firstLine = false;
			}

			if (this.EventName.Length > 0)
			{
				if (!firstLine)
				{
					sb.AppendLine(",");
				}
				sb.AppendFormat("eventName : \"{0}\"", this.EventName);
				firstLine = false;
			}

			if (this.InputFieldDateFormat.Length > 0)
			{
				if (!firstLine)
				{
					sb.AppendLine(",");
				}
				sb.AppendFormat("ifFormat : \"{0}\"", this.InputFieldDateFormat);
				firstLine = false;
			}

			if (this.DisplayAreaDateFormat.Length > 0)
			{
				if (!firstLine)
				{
					sb.AppendLine(",");
				}
				sb.AppendFormat("daFormat : \"{0}\"", this.DisplayAreaDateFormat);
				firstLine = false;
			}

			if (!this.SingleClick)
			{
				if (!firstLine)
				{
					sb.AppendLine(",");
				}
				sb.Append("singleClick : false");
				firstLine = false;
			}

			if (!String.IsNullOrEmpty(this.DateStatusFunction))
			{
				if (!firstLine)
				{
					sb.AppendLine(",");
				}
				sb.AppendFormat("dateStatusFunc : {0}", this.DateStatusFunction);
				firstLine = false;
			}

			if (this.FirstDay >= 0)
			{
				if (!firstLine)
				{
					sb.AppendLine(",");
				}
				sb.AppendFormat("firstDay : {0}", this.FirstDay);
				firstLine = false;
			}

			if (!this.WeekNumbers)
			{
				if (!firstLine)
				{
					sb.AppendLine(",");
				}
				sb.Append("weekNumbers : false");
				firstLine = false;
			}

			if (this.Align.Length > 0)
			{
				if (!firstLine)
				{
					sb.AppendLine(",");
				}
				sb.AppendFormat("align : \"{0}\"", this.Align);
				firstLine = false;
			}

			if (this.Range.Length > 0)
			{
				if (!firstLine)
				{
					sb.AppendLine(",");
				}
				sb.AppendFormat("range : \"{0}\"", this.Range);
				firstLine = false;
			}

			if (this.Flat.Length > 0)
			{
				if (!firstLine)
				{
					sb.AppendLine(",");
				}
				sb.AppendFormat("flat : \"{0}\"", this.ResolveID(this.Flat));
				firstLine = false;
			}

			if (this.Date != DateTime.Today)
			{
				if (!firstLine)
				{
					sb.AppendLine(",");
				}
				sb.AppendFormat("date : new Date({0},{1},{2})", this.Date.Year, this.Date.Month, this.Date.Day);
				firstLine = false;
			}

			if (this.ShowsTime)
			{
				if (!firstLine)
				{
					sb.AppendLine(",");
				}
				sb.Append("showsTime : true");
				firstLine = false;
			}

			if (this.TimeFormat == 12)
			{
				if (!firstLine)
				{
					sb.AppendLine(",");
				}
				sb.Append("timeFormat : \"12\"");
				firstLine = false;
			}

			if (!this.Electric)
			{
				if (!firstLine)
				{
					sb.AppendLine(",");
				}
				sb.Append("electric : false");
				firstLine = false;
			}

			if (this.Position.Length > 0)
			{
				if (!firstLine)
				{
					sb.AppendLine(",");
				}
				sb.AppendFormat("position : \"{0}\"", this.Position);
				firstLine = false;
			}

			if (this.CacheCalendar)
			{
				if (!firstLine)
				{
					sb.AppendLine(",");
				}
				sb.Append("cache : true");
				firstLine = false;
			}

			if (this.ShowOthers)
			{
				if (!firstLine)
				{
					sb.AppendLine(",");
				}
				sb.Append("showOthers : true");
				firstLine = false;
			}

			sb.AppendLine();
			//writer.Indent--;
			sb.AppendLine("});");
			//writer.Indent--;
			//writer.WriteLine("</script>");

			ScriptManager.RegisterStartupScript(this, typeof(DynarchCalendar), this.ClientID + "-Calendar.setup", sb.ToString(), true);
		}
		#endregion

		#region ResolveID
		/// <summary>
		/// Pokud ID patří controlu, pak vrátí jeho ClientID, jinak vrátí zpět původní ID.
		/// </summary>
		/// <param name="id">ID k resolvování</param>
		/// <returns>cílové ID</returns>
		protected virtual string ResolveID(string id)
		{
			Control ctrl = this.NamingContainer.FindControl(id);
			if (ctrl != null)
			{
				return ctrl.ClientID;
			}
			return id;
		}
		#endregion

		#region TransformDatePatternToClientScript
		/// <summary>
		/// Transformuje .NETový Date pattern do formátu používáného DynarchCalendarem.
		/// </summary>
		/// <param name="pattern">Date pattern.</param>
		/// <param name="dateSeparator">Řetězec, který má být použit jako oddělovač dne, měsíce a roku.</param>
		/// <param name="timeSeparator">Řetězec, který má být použit jako oddělovač hodin a minut.</param>
		/// <returns>DateFormat používaný DynarchCalendarem.</returns>
		public static string TransformDatePatternToClientScript(string pattern, string dateSeparator, string timeSeparator)
		{
			// dle dokumentace http://msdn.microsoft.com/en-us/library/8kb3ddd4(v=vs.100).aspx
			// je možné, aby v patternu byly literály - hodnoty, které bodou ve výstupu, což je uděláno uvozením do apostrofů nebo uvozovek
			// pro transformaci do formátu pro DynarchCalendar potřebujeme
			// 1) odstranit apostrof nebo uvozovky (zjednodušené řešení: rozsekáme pattern dle apostrofů a uvozovek do sekcí)
			// 2) nenahrazovat text v apostrofech či uvozovkách (zjednodušení řešeno: náhradou jen sudých sekcí)
			//
			// Zjednodušené řešení nepostihuje (zatím jsme na to nenarazili):
			// 1) nepodporuje uvozovky a apostrofy v patternu současně
			// 2) neřeší transformaci textu (aby nedošlo náhodou k záměně) z textu v uvozovkách DynarchCalendarem

			if (pattern.Contains("\'") && pattern.Contains("\""))
			{
				throw new NotSupportedException(String.Format("Uvozovky a apostrofy nejsou ve podporovány ({0}).", pattern));
			}

			string[] sections = pattern.Split('\'', '\"');
			for (var i = 0; i < sections.Length; i = i + 2)
			{
				string section = sections[i];
				section = section.Replace("%", "");

				// date
				section = section.Replace("dddd", "%A");
				section = section.Replace("ddd", "%a");
				section = section.Replace("dd", "#0#"); // %d
				section = section.Replace("d", "#0#");

				section = section.Replace("MMMM", "%B");
				section = section.Replace("MMM", "%b");
				section = section.Replace("MM", "#1#");
				section = section.Replace("M", "#1#");

				section = section.Replace("yyyy", "%Y");
				section = section.Replace("yy", "%y");
				section = section.Replace("y", "%y");

				// ignore gg, z, zz, zzz
				section = section.Replace("gg", "");
				section = section.Replace("z", "");

				// date: ve výsledku mohli přibýt A, a, B, b, Y, y

				// time
//			section = section.Replace("%H", "%k");
				section = section.Replace("%H", "#2#");
				section = section.Replace("HH", "#2#"); // %H
//			section = section.Replace("H", "%k");
				section = section.Replace("H", "#2#");
				section = section.Replace("hh", "%I");
				section = section.Replace("h", "%l");

				section = section.Replace("mm", "%M");
				section = section.Replace("%m", "%M");
				section = section.Replace("m", "%M");

				section = section.Replace("ss", "%S");
				section = section.Replace("%s", "%S");
				section = section.Replace("s", "%S");

				section = section.Replace("tt", "%P");
				section = section.Replace("%t", "%P");
				section = section.Replace("t", "%P");

				// time: ve výsledku mohli přibýt I, k, l, M, S, P

				// dočasné náhrady
				section = section.Replace("#0#", "%d");
				section = section.Replace("#1#", "%m");
				section = section.Replace("#2#", "%H");

				// Replace date separator
				section = section.Replace("/", dateSeparator);
				section = section.Replace(":", timeSeparator);

				sections[i] = section;
			}

			return String.Join("", sections);
		}
		#endregion

		#region RegisterCssForCalendarSkin (static)
		/// <summary>
		/// Zaregistruje css pro zobrazení kalendáře.
		/// Statická metoda je určena k řešení situace, kdy se kalendář ve stránce zobrazuje až v asynchronním postbacku.
		/// Touto metodou je potřeba zaregistrovat do stránky styly, které calendář používá (pokud není nastanven Skin na DynarchCalendarSkin.None).
		/// Pokud nejsou skripty zaregistrovány, zobrazuje se DynarchCalendar většinou jako rozsypaný/rozpadlý - tj. chybnně zobrazený.
		/// </summary>
		public static void RegisterCalendarSkinStylesheets(Page page)
		{
			if ((page.Header != null) && (Skin != DynarchCalendarSkin.None))
			{
				bool registered = (bool)(HttpContext.Current.Items["Havit.Web.UI.WebControls.DynarchCalendar.RegisterCss_registered"] ?? false);

				if (!registered)
				{
					HtmlLink htmlLink = new HtmlLink();
					string resourceName = "Havit.Web.UI.WebControls.DynarchCalendar.calendar-" + Skin.ToString().ToLower() + ".css";
					htmlLink.Href = page.ClientScript.GetWebResourceUrl(typeof(DynarchCalendar), resourceName);
					htmlLink.Attributes.Add("rel", "stylesheet");
					htmlLink.Attributes.Add("type", "text/css");
					//							<link rel="stylesheet" type="text/css" href="~/templates/styles/calendar-system.css" />
					page.Header.Controls.Add(htmlLink);
					HttpContext.Current.Items["Havit.Web.UI.WebControls.DynarchCalendar.RegisterCss_registered"] = true;
				}
			}
		}
		#endregion		
	}
}
