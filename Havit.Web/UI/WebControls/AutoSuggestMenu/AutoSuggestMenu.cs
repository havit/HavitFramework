#pragma warning disable 1591
using System;
using System.Text;
using System.Collections;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.Generic;

using Havit.Web.UI.ClientScripts;

[assembly: WebResource("Havit.Web.UI.WebControls.AutoSuggestMenu.AutoSuggestMenu.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.AutoSuggestMenu.AutoSuggestMenuItem.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.AutoSuggestMenu.XUtils.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.AutoSuggestMenu.Events.js", "text/javascript")]
[assembly: WebResource("Havit.Web.UI.WebControls.AutoSuggestMenu.AutoSuggestMenu.css", "text/css")]
[assembly: WebResource("Havit.Web.UI.WebControls.AutoSuggestMenu.Blank.html", "text/html")]

namespace Havit.Web.UI.WebControls
{
     /// <summary>Represents a control for extending TextBox with suggestions. Not compatible with UpdatePanel.</summary>
    [ValidationProperty("SelectedValue")]
    [DefaultProperty("SelectedValue")]
    [ToolboxData("<{0}:AutoSuggestMenu runat=server></{0}:AutoSuggestMenu>")]
	public class AutoSuggestMenu : WebControl, INamingContainer
	{
		private string _targetControlID;
		private AutoSuggestMenuMode _mode = AutoSuggestMenuMode.Classic;
		private string _messageOnClearText;
		private bool _autoPostBack = false;

        private int _minSuggestChars;
		private int _maxSuggestChars;
		private int _keyPressDelay;	
		private bool _usePaging;
        private int _pageSize;
		private string _context;

        private Unit _maxHeight;
		private string _menuItemCssClass;
		private string _selMenuItemCssClass;
        private string _navigationLinkCssClass;

        private bool _updateTextBoxOnUpDown;
		private bool _useIFrame;
		//private string _resourcesDir;

        private bool _usePageMethods;

        private string _onGetSuggestions;
        private string _onClientTextBoxUpdate;

	    //Internal
        private HiddenField _hdnSelectedValue;

        /// <summary>
		/// Indikuje, zda má dojít k automatické registraci CSS v OnPreRenderu.
		/// Výchozí hodnota je true.
		/// </summary>
		public bool AutoRegisterStyleSheets
		{
			get
			{
				return (bool)(ViewState["AutoRegisterStyleSheets"] ?? true);
			}
			set
			{
				ViewState["AutoRegisterStyleSheets"] = value;
			}
		}

        public AutoSuggestMenuMode Mode
		{
			get { return _mode; }
			set { _mode = value; }
		}

		/// <summary>
		/// Zpráva zobrazená na klientské straně (javascriptovým alertem) pokud dojde k vyčištění uživatelem zadané hodnoty do textboxu při nespárování.
		/// Zpráva může obsahovat značku #TEXT#, která je nahrazena skutečně vyčištěnou hodnotou textboxu.
		/// </summary>
		public string MessageOnClearText
		{
			get { return _messageOnClearText ?? String.Empty; }
			set { _messageOnClearText = value; }
		}

		public bool AutoPostBack
		{
			get { return _autoPostBack; }
			set { _autoPostBack = value; }
		}
	
        public string TargetControlID
        {
            get { return _targetControlID; }
            set { _targetControlID = value; }
        }

        public int MinSuggestChars
        {
            get { return _minSuggestChars; }
            set { _minSuggestChars = value; }
        }

		public int MaxSuggestChars
		{
			get	{ return _maxSuggestChars; }
			set	{ _maxSuggestChars = value; }
		}

		public int KeyPressDelay
		{
			get	{ return _keyPressDelay; }
			set	{ _keyPressDelay = value; }
		}

		public bool UsePaging
		{
			get	{ return _usePaging; }
            set { _usePaging = value; }
		}

        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

		/// <summary>
		/// Kontext, v jakém je AutoSuggestMenu použito.
		/// Předává se webové službě, která vrací položky pro nápovědu, aby bylo možno se přizpůsobit kontextu (omezit výběr, atp.).
		/// </summary>
		public new string Context
		{
			get { return _context; }
			set { _context = value; }
		}

        public Unit MaxHeight
        {
            get { return _maxHeight; }
            set { _maxHeight = value; }
        }

		public string MenuItemCssClass
		{
            get { return _menuItemCssClass; }
            set { _menuItemCssClass = value; }
		}

		public string SelMenuItemCssClass
		{
            get { return _selMenuItemCssClass; }
            set { _selMenuItemCssClass = value; }
		}

        public string NavigationLinkCssClass
        {
            get { return _navigationLinkCssClass; }
            set { _navigationLinkCssClass = value; }
        }

        public bool UpdateTextBoxOnUpDown
        {
            get { return _updateTextBoxOnUpDown; }
            set { _updateTextBoxOnUpDown = value; }
        }

		public bool UseIFrame
		{
			get	{ return _useIFrame; }
			set	{ _useIFrame = value; }
		}

		//public string ResourcesDir
		//{
		//    get	{return _resourcesDir;}
		//    set	{_resourcesDir=value;}
		//}

        public bool UsePageMethods
        {
            get { return _usePageMethods; }
            set { _usePageMethods = value; }
        }

        public string OnGetSuggestions
        {
            get { return _onGetSuggestions; }
            set { _onGetSuggestions = value; }
        }

        public string OnClientTextBoxUpdate
        {
            get { return _onClientTextBoxUpdate; }
            set { _onClientTextBoxUpdate = value; }
        }

        public string SelectedValue
        {
            get
            {
                EnsureChildControls();
                return _hdnSelectedValue.Value;
            }

            set
            {
                EnsureChildControls();
                _hdnSelectedValue.Value = value;
            }
        }

        /// <summary>
		/// Cesta k webové službě, která má být přilinkována.
		/// </summary>
		public string ServicePath
		{
			get
			{
				return _servicePath;
			}
			set
			{
				_servicePath = value;
			}
		}
		private string _servicePath;

		/// <summary>Initializes new instance of AutoSuggestMenu/// </summary>
		/// <remarks>Wire the events so the control can participate in them</remarks>
		public AutoSuggestMenu()
		{
			//Set Defaults
			_minSuggestChars = 1;
			_maxSuggestChars = 999;

			//_resourcesDir = "~/asm_includes";

			//Number of milliseconds to wait before returning Suggestions div
			//Makes control more efficient
			_keyPressDelay = 300;

			_usePaging = true;
			_pageSize = 10;

			this.CssClass = "asmMenu";

			_menuItemCssClass = "asmMenuItem";
			_selMenuItemCssClass = "asmSelMenuItem";
			_navigationLinkCssClass = "asmNavigationLink";

			_updateTextBoxOnUpDown = true;
			_useIFrame = true;

			_usePageMethods = true;
			_onGetSuggestions = "GetSuggestions";

			this.Attributes["autocomplete"] = "off";
		}

		/// <summary>This member overrides <see cref="Control.CreateChildControls"/></summary>
		protected override void CreateChildControls()
		{
			_hdnSelectedValue = new HiddenField();
			_hdnSelectedValue.ID = "hdnSelectedValue";

			Controls.Add(_hdnSelectedValue);
		}

		/// <summary>This member overrides <see cref="Control.LoadViewState"/></summary>
		/// <remarks>Loads date from view-state saved in previous page into date control properties</remarks>
		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);

			_targetControlID = (string)ViewState["TargetControlID"];
			_mode = (AutoSuggestMenuMode)ViewState["Mode"];
			_autoPostBack = (bool)ViewState["AutoPostBack"];
			_minSuggestChars = (int)ViewState["MinSuggestChars"];
			_maxSuggestChars = (int)ViewState["MaxSuggestChars"];

			_keyPressDelay = (int)ViewState["KeyPressDelay"];
			_usePaging = (bool)ViewState["UsePaging"];
			_pageSize = (int)ViewState["PageSize"];
			_context = (string)ViewState["Context"];

			_maxHeight = (Unit)ViewState["MaxHeight"];

			_menuItemCssClass = (string)ViewState["MenuItemCssClass"];
			_selMenuItemCssClass = (string)ViewState["SelMenuItemCssClass"];
			_navigationLinkCssClass = (string)ViewState["NavigationLinkCssClass"];

			_updateTextBoxOnUpDown = (bool)ViewState["UpdateTextBoxOnUpDown"];
			_useIFrame = (bool)ViewState["UseIFrame"];
			//_resourcesDir			    = (string)ViewState["ResourcesDir"];

			_usePageMethods = (bool)ViewState["UsePageMethods"];
			_onGetSuggestions = (string)ViewState["OnGetSuggestions"];
			_onClientTextBoxUpdate = (string)ViewState["OnClientTextBoxUpdate"];

			_messageOnClearText = (string)ViewState["MessageOnClearText"];

			_servicePath = (string)ViewState["ServicePath"];
		}

		/// <summary>This member overrides <see cref="Control.SaveViewState"/></summary>
		/// <remarks>Stores date control properties in view-state for future page</remarks>
		protected override object SaveViewState()
		{
			ViewState["TargetControlID"] = _targetControlID;
			ViewState["Mode"] = _mode;
			ViewState["AutoPostBack"] = _autoPostBack;
			ViewState["MinSuggestChars"] = _minSuggestChars;
			ViewState["MaxSuggestChars"] = _maxSuggestChars;

			ViewState["KeyPressDelay"] = _keyPressDelay;
			ViewState["UsePaging"] = _usePaging;
			ViewState["PageSize"] = _pageSize;
			ViewState["Context"] = _context;

			ViewState["MaxHeight"] = _maxHeight;

			ViewState["MenuItemCssClass"] = _menuItemCssClass;
			ViewState["SelMenuItemCssClass"] = _selMenuItemCssClass;
			ViewState["NavigationLinkCssClass"] = _navigationLinkCssClass;

			ViewState["UpdateTextBoxOnUpDown"] = _updateTextBoxOnUpDown;

			ViewState["UseIFrame"] = _useIFrame;
			//ViewState["ResourcesDir"]		    = _resourcesDir;

			ViewState["UsePageMethods"] = _usePageMethods;
			ViewState["OnGetSuggestions"] = _onGetSuggestions;
			ViewState["OnClientTextBoxUpdate"] = _onClientTextBoxUpdate;

			ViewState["ServicePath"] = _servicePath;
			ViewState["MessageOnClearText"] = _messageOnClearText;

			return base.SaveViewState();
		}

		//internal string GetAbsoluteResourcesDir()
		//{
		//    string resourcesDir = _resourcesDir;
		//    if (resourcesDir.Substring(0, 1) == "~")
		//    {
		//        //Remove ~
		//        resourcesDir = resourcesDir.Substring(1, resourcesDir.Length - 1);

		//        if (HttpRuntime.AppDomainAppVirtualPath != "/")
		//            resourcesDir = HttpRuntime.AppDomainAppVirtualPath + resourcesDir;
		//    }

		//    return resourcesDir;
		//}

		protected void WriteJSIncludes()
		{
			ScriptManager.ScriptResourceMapping.EnsureScriptRegistration(this.Page, "jquery");

			//if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("AutoSuggestMenu_XUtils"))
			//{
			//string resourcesDir = GetAbsoluteResourcesDir();

			//RegisterClientScriptInclude("AutoSuggestMenu_XUtils", resourcesDir + "/XUtils.js");

			ScriptManager.RegisterClientScriptResource(this, typeof(AutoSuggestMenu), "Havit.Web.UI.WebControls.AutoSuggestMenu.XUtils.js");

			// RegisterClientScriptInclude("AutoSuggestMenu_Events", resourcesDir + "/Events.js");
			ScriptManager.RegisterClientScriptResource(this, typeof(AutoSuggestMenu), "Havit.Web.UI.WebControls.AutoSuggestMenu.Events.js");

			// RegisterClientScriptInclude("AutoSuggestMenu_AutoSuggestMenu", resourcesDir + "/AutoSuggestMenu.js");
			ScriptManager.RegisterClientScriptResource(this, typeof(AutoSuggestMenu), "Havit.Web.UI.WebControls.AutoSuggestMenu.AutoSuggestMenu.js");

			//RegisterClientScriptInclude("AutoSuggestMenu_AutoSuggestMenuItem", resourcesDir + "/AutoSuggestMenuItem.js");
			ScriptManager.RegisterClientScriptResource(this, typeof(AutoSuggestMenu), "Havit.Web.UI.WebControls.AutoSuggestMenu.AutoSuggestMenuItem.js");
			//} 
		}

		protected void WriteMenu()
		{
			HtmlTextWriter writer = new HtmlTextWriter(new System.IO.StringWriter());

			//Javascript to associate
			string funcName = "writeMenu_" + this.ClientID;
			writer.WriteLine("function  " + funcName + "()");
			writer.WriteLine("{");

			writer.WriteLine("var menu=new AutoSuggestMenu();");

			writer.WriteLine("menu.id=\"" + this.ClientID + "\";");

			string textBoxID = this.NamingContainer.FindControl(_targetControlID).ClientID;
			writer.WriteLine("menu.textBoxID=\"" + textBoxID + "\";");
			writer.WriteLine("menu.hiddenSelectedValueID=\"" + _hdnSelectedValue.ClientID + "\";");

			writer.WriteLine("menu.minSuggestChars=" + _minSuggestChars + ";");
			writer.WriteLine("menu.maxSuggestChars=" + _maxSuggestChars + ";");
			writer.WriteLine("menu.keyPressDelay=" + _keyPressDelay + ";");
			writer.WriteLine("menu.usePaging=" + _usePaging.ToString().ToLower() + ";");
			writer.WriteLine("menu.pageSize=\"" + _pageSize + "\";");
			if (_context != null)
			{
				writer.WriteLine("menu.context=\"" + _context.Replace("\"", "\\\"") + "\";");
			}

			if (!_maxHeight.IsEmpty)
			{
				writer.WriteLine("menu.maxHeight=\"" + _maxHeight.ToString() + "\";");
			}

			writer.WriteLine("menu.cssClass=\"" + this.CssClass + "\";");

			string jsMessageOnClearText = (HttpUtilityExt.GetResourceString((_messageOnClearText ?? String.Empty)) ?? String.Empty).Replace("\"", "\\\"");
			writer.WriteLine("menu.messageOnClearText=\"" + jsMessageOnClearText + "\";");

			writer.WriteLine("menu.menuItemCssClass=\"" + _menuItemCssClass + "\";");
			writer.WriteLine("menu.selMenuItemCssClass=\"" + _selMenuItemCssClass + "\";");
			writer.WriteLine("menu.navigationLinkCssClass=\"" + _navigationLinkCssClass + "\";");

			writer.WriteLine("menu.updateTextBoxOnUpDown=" + _updateTextBoxOnUpDown.ToString().ToLower() + ";");
			writer.WriteLine("menu.useIFrame=" + _useIFrame.ToString().ToLower() + ";");

			//writer.WriteLine("menu.resourcesDir=\"" + GetAbsoluteResourcesDir() + "\";");
			string resourceName = "Havit.Web.UI.WebControls.AutoSuggestMenu.Blank.html";
			writer.WriteLine("menu.blankPage=\"" + Page.ClientScript.GetWebResourceUrl(typeof(AutoSuggestMenu), resourceName) + "\";");  // RH, místo resourcesDir

			if (Mode == AutoSuggestMenuMode.ClearTextOnNoSelection)
			{ 
				writer.WriteLine("menu.clearTextOnNoSelection=true;");
			}

			if (AutoPostBack)
			{
				
				writer.WriteLine(String.Format("menu.autoPostBackScript=\"{0}\";", Page.ClientScript.GetPostBackEventReference(this, "")));
				// Pro kompatibilitu s UpdatePanelem je pravděpodobně nutno odpojit se od události kliknutí na HTML element, avšak to následně způsobí dvojí postback.
				//writer.WriteLine(String.Format("menu.autoPostBackScript=\" window.setTimeout(function() {{ {0} }}, 1);\"", Page.ClientScript.GetPostBackEventReference(this, "")));
			}

			string func = _onGetSuggestions;
			if (_usePageMethods)
			{
				func = "PageMethods." + func;
			}

			writer.WriteLine("menu.onGetMenuItems=\"" + func + "\";");

			if (!String.IsNullOrEmpty(_onClientTextBoxUpdate))
			{
				writer.WriteLine("menu.onTextBoxUpdate=\"" + _onClientTextBoxUpdate + "\";");
			}

			writer.WriteLine("menu.render();");
			writer.WriteLine("}");

			writer.WriteLine("");

			//in IE 6.0 adding div to Body before </body> tag, but nested inside the <form> will throw "Operation Aborted error".
			//Since RegisterClientStartupScript adds it before </form>.  Need to call the method as soon as Body is loaded. 

			if (IsInPartialRendering())
			{
				//Just call the method since the Body is already renderer
				writer.WriteLine(funcName + "();");
			}
			else
			{
				writer.WriteLine("XUtils.addEventListener(window, \"load\", " + funcName + ");");
			}

			RegisterClientStartupScript("AutoSuggestMenu_" + this.UniqueID, writer.InnerWriter.ToString());
		}

		protected void CheckRequiredProperties()
		{
			if (_targetControlID == null)
			{
				throw new Exception("TargetControlID property is required.");
			}

			if (this.NamingContainer.FindControl(_targetControlID) == null)
			{
				throw new Exception("Target control with ID '" + _targetControlID + "' doesn't exist");
			}

			if (String.IsNullOrEmpty(_onGetSuggestions))
			{
				throw new Exception("onGetSuggestions property is required.");
			}

			//Make sure resources directory exists
			//string dir=GetAbsoluteResourcesDir();
			//dir=this.Page.Request.MapPath(dir);

			//if (!System.IO.Directory.Exists(dir))
			//    throw new Exception("ResourcesDir '" + _resourcesDir + "' doesn't exist");
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnPreRender(EventArgs e)
		{
			CheckRequiredProperties();

			base.OnPreRender(e);

			RegisterServicePath();
			WriteJSIncludes();
			if (AutoRegisterStyleSheets)
			{
				RegisterStylesheets(this.Page);
			}
		}

		private void RegisterServicePath()
		{
			if (!String.IsNullOrEmpty(this.ServicePath))
			{
				ScriptManager.GetCurrent(this.Page).Services.Add(new ServiceReference(this.ServicePath));
			}
		}

		/// <summary>Renders AutoSuggestMenu to the output HTML parameter specified.</summary>
		/// <param name="output"> The HTML writer to write out to</param>
		protected override void Render(HtmlTextWriter output)
		{
			// Pokud je na GridView schován sloupec, tak schování probíhá až po OnPreRender. Proto musíme mít registraci skriptu až zde - pokud není DynarchCalendar zobrazen, neemitujeme skripty.
			// Ale nemůžeme sem dát vše, protože na registraci pomocí RegisterClientScriptResource je pozdě (možno vyzkoušet RegisterNamedResource).
			WriteMenu();

			//output.WriteLine("<link href=\"" + GetAbsoluteResourcesDir() + "/AutoSuggestMenu.css\" type=\"text/css\" rel=\"stylesheet\">");

			// Pro kompatibilitu s update panelem je nutno renderovat komponentu, nejen její obsah (hidden field).
			//base.Render(output);

			_hdnSelectedValue.RenderControl(output);
		}

		/// <summary>
		/// Vytvoří do head odkaz na CSS menu.
		/// </summary>
		public static void RegisterStylesheets(Page page)
		{
			bool registered = (bool)(HttpContext.Current.Items["Havit.Web.UI.WebControls.AutoSuggestMenu.RegisterCss_registered"] ?? false);

			if (!registered)
			{
				HtmlLink htmlLink = new HtmlLink();
				string resourceName = "Havit.Web.UI.WebControls.AutoSuggestMenu.AutoSuggestMenu.css";
				htmlLink.Href = page.ClientScript.GetWebResourceUrl(typeof(AutoSuggestMenu), resourceName);
				htmlLink.Attributes.Add("rel", "stylesheet");
				htmlLink.Attributes.Add("type", "text/css");
				page.Header.Controls.Add(htmlLink);
				HttpContext.Current.Items["Havit.Web.UI.WebControls.AutoSuggestMenu.RegisterCss_registered"] = true;
			}
		}

		internal static void WriteMenuItemsToJSON(List<AutoSuggestMenuItem> menuItems, int totalResults, XJsonWriter writer)
		{
			List<string> jsonMenuItems = new List<string>();

			string jsonMenuItem;

			foreach (AutoSuggestMenuItem menuItem in menuItems)
			{
				jsonMenuItem = menuItem.GetJSON();
				jsonMenuItems.Add(jsonMenuItem);
			}

			writer.WriteList("menuItems", jsonMenuItems, false);

			//Add total results
			writer.WriteNameValue("totalResults", totalResults, true);
		}

		/// <summary>
		///     Used to send data back to browser.
		/// </summary>
		/// <returns>
		/// {"MenuItems": 
		///   [
		///       {"label": "Option1", "value": "1"},
		///       {"label": "Option2", "value": "2"},
		///       {"label": "Option3", "value": "3"},
		///   ]
		/// }
		/// </returns>
		///
		public static string ConvertMenuItemsToJSON(List<AutoSuggestMenuItem> menuItems, int totalResults)
		{
			XJsonWriter writer = new XJsonWriter();
			WriteMenuItemsToJSON(menuItems, totalResults, writer);
			return writer.ToString();
		}

		//==========================================
        //Utility methods for registering javascript
        //==========================================

        //Methods for handling Client Script in both scenarious - when using partial postback and when not using it
		private bool IsInPartialRendering()
		{
			ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
			if (scriptManager != null)
			{
				return scriptManager.IsInAsyncPostBack;
			}

			return false;
		}

		private void RegisterClientScriptInclude(string key, string url)
		{
			if (IsInPartialRendering())
			{
				ScriptManager.RegisterClientScriptInclude(this, this.GetType(), key, url);
			}
			else
			{
				this.Page.ClientScript.RegisterClientScriptInclude(key, url);
			}
		}

		private void RegisterClientStartupScript(string key, string script)
		{
			if (IsInPartialRendering())
			{
				ScriptManager.RegisterStartupScript(this, this.GetType(), key, script, true);
			}
			else
			{
				this.Page.ClientScript.RegisterStartupScript(this.GetType(), key, script, true);
			}
		}
	}
}
#pragma warning restore 1591