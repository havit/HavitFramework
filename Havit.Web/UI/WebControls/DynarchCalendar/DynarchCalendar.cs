using System;
using System.Text;
using System.Web;
using System.Web.UI;

[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar_stripped.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-setup_stripped.js", "text/javascript")]

[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-cs-utf8.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.DynarchCalendar.calendar-cs-win.js", "text/javascript")]

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Wrapper pro Dynarch DHTML/JavaScript Calendar
	/// http://www.dynarch.com/projects/calendar/
	/// </summary>
	/// <remarks>
	/// Properties odpovídají pøevážnì vlastnostem nastavovacího objektu používaného v jscript:Calendar.setup()
	/// </remarks>
	/// <example>
	/// PopUp kalendáø:
	/// <code>
	/// &lt;asp:TextBox ID="OdDateTB" Text="3.3.2004" Runat="server" /&gt;
	/// &lt;asp:Button ID="OdPickerBtn" Text="..." Runat="server" /&gt;
	/// &lt;havit:DynarchCalendar
	/// 	InputField = "OdDateTB"
	/// 	Button = "OdPickerBtn"
	/// 	ShowOthers = "true"
	/// 	Runat = "server" /&gt;
	/// </code>
	/// Flat kalendáø:
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
		#region static Properties
		/// <summary>
		/// Cesta k hlavnímu skriptu calendar[_stripped].js
		/// Prázdná hodnota zpùsobí použití skriptu pøes WebResource.axd.
		/// </summary>
		public static string MainScriptUrl
		{
			get	{ return mainScriptUrl; }
			set { mainScriptUrl = value; }
		}
		private static string mainScriptUrl = String.Empty;

		/// <summary>
		/// Cesta k vybranému jazykovému skriptu calendar-{lang}.js
		/// Prázdná hodnota zpùsobí použití scriptu pro èeštinu pøes WebResource.axd, vybírá se skript podle kódování.
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
		/// Prázdná hodnota zpùsobí použití setup skriptu pøes WebResource.axd.
		/// </summary>
		public static string SetupScriptUrl
		{
			get	{ return setupScriptUrl; }
			set { setupScriptUrl = value; }
		}
		private static string setupScriptUrl = String.Empty;
		#endregion

		#region Originální vlastnosti kalendáøe odpovídající JScrípt
		/// <summary>
		/// The ID of your input field.
		/// </summary>
		/// <remarks>
		/// Pokud je zadáno ID controlu, dohledá se ClientID od controlu,
		/// jinak je použito pøímo.
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
				return String.Empty;
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
		/// jinak je použito pøímo.
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
		/// jinak je použito pøímo.
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
		/// Default hodnota "click" je urèena v calendar-setup.js a zde je reprezentována
		/// prázdným øetìzcem (nerenderovat nastavení tohoto parametru).
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
		/// Default hodnota je urèena v calendar-setup[_stripped].js a zde je reprezentována
		/// prázdným øetìzcem (nerenderovat nastavení tohoto parametru).
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
				return String.Empty;
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
		/// Default hodnota je urèena v calendar-setup[_stripped].js a zde je reprezentována
		/// prázdným øetìzcem (nerenderovat nastavení tohoto parametru).
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
		/// Default hodnota je urèena v calendar-{lang}.js a zde je reprezentována
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
		/// Resolvuje se pøi renderování - pokud ID patøí controlu, nahradí se jeho ClientID.
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
		/// You should provide this function if the calendar is flat.
		/// It will be called when the date in the calendar is changed with a reference to the calendar object.
		/// </summary>
		/// <remarks>
		/// See reference section 2.2 for an example of how to setup a flat calendar. 
		/// </remarks>
		public string FlatCallback
		{
			get
			{
				string tmp = (string)ViewState["FlatCallback"];
				if (tmp != null)
				{
					return tmp;
				}
				return String.Empty;
			}
			set
			{
				ViewState["FlatCallback"] = value;
			}
		}

		/// <summary>
		/// If you provide a function handler here then you have to manage the "click-on-date" event by yourself.
		/// Look in the calendar-setup.js and take as an example the onSelect handler that you can see there. 
		/// </summary>
		public string OnSelectFunction
		{
			get
			{
				string tmp = (string)ViewState["OnSelectFunction"];
				if (tmp != null)
				{
					return tmp;
				}
				return String.Empty;
			}
			set
			{
				ViewState["OnSelectFunction"] = value;
			}
		}

		/// <summary>
		/// This handler will be called when the calendar needs to close.
		/// You don't need to provide one, but if you do it's your responsibility to hide/destroy the calendar.
		/// You're on your own. Check the calendar-setup.js file for an example.
		/// </summary>
		public string OnCloseFunction
		{
			get
			{
				string tmp = (string)ViewState["OnCloseFunction"];
				if (tmp != null)
				{
					return tmp;
				}
				return String.Empty;
			}
			set
			{
				ViewState["OnCloseFunction"] = value;
			}
		}

		/// <summary>
		/// If you supply a function handler here, it will be called right after the target field
		/// is updated with a new date. You can use this to chain 2 calendars,
		/// for instance to setup a default date in the second just
		/// after a date was selected in the first. 
		/// </summary>
		public string OnUpdateFunction
		{
			get
			{
				string tmp = (string)ViewState["OnUpdateFunction"];
				if (tmp != null)
				{
					return tmp;
				}
				return String.Empty;
			}
			set
			{
				ViewState["OnUpdateFunction"] = value;
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
					throw new ArgumentOutOfRangeException("TimeFormat", value, "Hodnota musí být 12 nebo 24.");
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
		/// Default: false.
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
				return false;
			}
			set
			{
				ViewState["ShowOthers"] = value;
			}
		}
		#endregion

		#region Pøidané vlastnosti
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

		#region ValidateControlProperties
		/// <summary>
		/// Zkontroluje, zda jsou vlastnosti controlu správnì nastaveny a mohou být použity.
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
				throw new InvalidOperationException("Alespoò jedna z vlastností InputField, DisplayArea, Button nebo Flat musí být nastavena.");
			}
		}
		#endregion

		#region RegisterClientScript
		/// <summary>
		/// Zaregistruje klientské skripty kalendáøe.
		/// </summary>
		protected virtual void RegisterClientScript()
		{
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
				Page.ClientScript.RegisterClientScriptResource(typeof(DynarchCalendar), "Havit.Web.UI.WebControls.DynarchCalendar.calendar_stripped.js");
			}
			else
			{
				if (!this.Page.ClientScript.IsClientScriptBlockRegistered("DynarchCalendar.MainScript"))
				{
					StringBuilder sb = new StringBuilder();
					sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>",
						this.ResolveUrl(DynarchCalendar.MainScriptUrl));
					this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "DynarchCalendar.MainScript", sb.ToString());
				}
			}
		}
		#endregion

		#region RegisterLanguageScript
		/// <summary>
		/// Zaregistruje klientský skript odpovídající jazykové mutace kalendáøe,
		/// tj calendar-en.js, calendar-de.js, atp.
		/// </summary>
		protected virtual void RegisterLanguageScript()
		{
			if (String.IsNullOrEmpty(LanguageScriptUrl))
			{
				if (HttpContext.Current.Response.ContentEncoding == Encoding.UTF8)
				{
					Page.ClientScript.RegisterClientScriptResource(typeof(DynarchCalendar), "Havit.Web.UI.WebControls.DynarchCalendar.calendar-cs-utf8.js");
				}
				else
				{
					Page.ClientScript.RegisterClientScriptResource(typeof(DynarchCalendar), "Havit.Web.UI.WebControls.DynarchCalendar.calendar-cs-win.js");
				}
			}
			else
			{
				if (!this.Page.ClientScript.IsClientScriptBlockRegistered("DynarchCalendar.LanguageScript"))
				{
					StringBuilder sb = new StringBuilder();
					sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>",
						this.ResolveUrl(DynarchCalendar.LanguageScriptUrl));
					this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "DynarchCalendar.LanguageScript", sb.ToString());
				}
			}
		}
		#endregion

		#region RegisterSetupScript
		/// <summary>
		/// Zaregistruje klientský setup-skript pro funkènost Calendar.setup, tj calendar-setup.js.		
		/// </summary>
		protected virtual void RegisterSetupScript()
		{
			if (String.IsNullOrEmpty(SetupScriptUrl))
			{
				// pokud není URL skriptu zadáno, použijeme soubor z resources
				Page.ClientScript.RegisterClientScriptResource(typeof(DynarchCalendar), "Havit.Web.UI.WebControls.DynarchCalendar.calendar-setup_stripped.js");
			}
			else
			{
				if (!this.Page.ClientScript.IsClientScriptBlockRegistered("DynarchCalendar.SetupScript"))
				{
					StringBuilder sb = new StringBuilder();
					sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>",
						this.ResolveUrl(DynarchCalendar.SetupScriptUrl));
					this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "DynarchCalendar.SetupScript", sb.ToString());
				}
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
		/// Zkontroluje platnost properties a zaregistruje klientské skripty kalendáøe.
		/// </summary>
		/// <param name="e">EventArgs</param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if (this.Enabled)
			{
				this.ValidateControlProperties();
				this.RegisterClientScript();
			}
		}
		#endregion

		#region Render
		/// <summary>
		/// Vyrenderuje script nastavení kalendáøe pøes Calendar.setup(...).
		/// </summary>
		/// <param name="writer">The HtmlTextWriter object that receives the server control content.</param>
		protected override void Render(HtmlTextWriter writer)
		{
			if (this.Enabled)
			{
				writer.WriteLine("<script type=\"text/javascript\">");
				writer.Indent++;
				writer.WriteLine("Calendar.setup({");
				writer.Indent++;

				bool firstLine = true;
				if (this.InputField.Length > 0)
				{
					if (!firstLine) writer.WriteLine(",");
					writer.Write("inputField : \"{0}\"", this.ResolveID(this.InputField));
					firstLine = false;
				}

				if (this.DisplayArea.Length > 0)
				{
					if (!firstLine) writer.WriteLine(",");
					writer.Write("displayArea : \"{0}\"", this.ResolveID(this.DisplayArea));
					firstLine = false;
				}

				if (this.Button.Length > 0)
				{
					if (!firstLine) writer.WriteLine(",");
					writer.Write("button : \"{0}\"", this.ResolveID(this.Button));
					firstLine = false;
				}

				if (this.EventName.Length > 0)
				{
					if (!firstLine) writer.WriteLine(",");
					writer.Write("eventName : \"{0}\"", this.EventName);
					firstLine = false;
				}

				if (this.InputFieldDateFormat.Length > 0)
				{
					if (!firstLine) writer.WriteLine(",");
					writer.Write("ifFormat : \"{0}\"", this.InputFieldDateFormat);
					firstLine = false;
				}

				if (this.DisplayAreaDateFormat.Length > 0)
				{
					if (!firstLine) writer.WriteLine(",");
					writer.Write("daFormat : \"{0}\"", this.DisplayAreaDateFormat);
					firstLine = false;
				}

				if (!this.SingleClick)
				{
					if (!firstLine) writer.WriteLine(",");
					writer.Write("singleClick : false");
					firstLine = false;
				}

				if (!String.IsNullOrEmpty(this.DateStatusFunction))
				{
					if (!firstLine) writer.WriteLine(",");
					writer.Write("dateStatusFunc : {0}", this.DateStatusFunction);
					firstLine = false;
				}

				if (this.FirstDay > 0)
				{
					if (!firstLine) writer.WriteLine(",");
					writer.Write("firstDay : {0}", this.FirstDay);
					firstLine = false;
				}

				if (!this.WeekNumbers)
				{
					if (!firstLine) writer.WriteLine(",");
					writer.Write("weekNumbers : false");
					firstLine = false;
				}

				if (this.Align.Length > 0)
				{
					if (!firstLine) writer.WriteLine(",");
					writer.Write("align : \"{0}\"", this.Align);
					firstLine = false;
				}

				if (this.Range.Length > 0)
				{
					if (!firstLine) writer.WriteLine(",");
					writer.Write("range : \"{0}\"", this.Range);
					firstLine = false;
				}

				if (this.Flat.Length > 0)
				{
					if (!firstLine) writer.WriteLine(",");
					writer.Write("flat : \"{0}\"", this.ResolveID(this.Flat));
					firstLine = false;
				}

				if (this.FlatCallback.Length > 0)
				{
					if (!firstLine) writer.WriteLine(",");
					writer.Write("flatCallback : \"{0}\"", this.FlatCallback);
					firstLine = false;
				}

				if (this.OnSelectFunction.Length > 0)
				{
					if (!firstLine) writer.WriteLine(",");
					writer.Write("onSelect : \"{0}\"", this.OnSelectFunction);
					firstLine = false;
				}

				if (this.OnCloseFunction.Length > 0)
				{
					if (!firstLine) writer.WriteLine(",");
					writer.Write("onClose : \"{0}\"", this.OnCloseFunction);
					firstLine = false;
				}

				if (this.OnUpdateFunction.Length > 0)
				{
					if (!firstLine) writer.WriteLine(",");
					writer.Write("onUpdate : \"{0}\"", this.OnUpdateFunction);
					firstLine = false;
				}

				if (this.Date != DateTime.Today)
				{
					if (!firstLine) writer.WriteLine(",");
					writer.Write("date : new Date({0},{1},{2})", this.Date.Year, this.Date.Month, this.Date.Day);
					firstLine = false;
				}

				if (this.ShowsTime)
				{
					if (!firstLine) writer.WriteLine(",");
					writer.Write("showsTime : true");
					firstLine = false;
				}

				if (this.TimeFormat == 12)
				{
					if (!firstLine) writer.WriteLine(",");
					writer.Write("timeFormat : \"12\"");
					firstLine = false;
				}

				if (!this.Electric)
				{
					if (!firstLine) writer.WriteLine(",");
					writer.Write("electric : false");
					firstLine = false;
				}

				if (this.Position.Length > 0)
				{
					if (!firstLine) writer.WriteLine(",");
					writer.Write("position : \"{0}\"", this.Position);
					firstLine = false;
				}

				if (this.CacheCalendar)
				{
					if (!firstLine) writer.WriteLine(",");
					writer.Write("cache : true");
					firstLine = false;
				}

				if (this.ShowOthers)
				{
					if (!firstLine) writer.WriteLine(",");
					writer.Write("showOthers : true");
					firstLine = false;
				}

				writer.WriteLine();
				writer.Indent--;
				writer.WriteLine("});");
				writer.Indent--;
				writer.WriteLine("</script>");
			}
		}
		#endregion

		#region ResolveID
		/// <summary>
		/// Pokud ID patøí controlu, pak vrátí jeho ClientID, jinak vrátí zpìt pùvodní ID.
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
	}
}
