using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Havit.Web.UI.ClientScripts;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Control pro výběr více hodnot pomocí checkboxů, které vizuálně vypadají jako položky DDL.
	/// Řešeno javascriptem postaveným nad ListBoxem (html element select).
	/// 
	/// Dokumentace a download:
	/// http://wenzhixin.net.cn/p/multiple-select/docs/#constructor
	/// https://github.com/wenzhixin/multiple-select
	/// </summary>
	public class CheckBoxDropDownList : ListBoxExt
	{
		private bool renderIsOpen = false;

		/// <summary>
		/// Indikuje, zda má být zobrazena volba pro rychlé zvolení všech hodnot.
		/// </summary>
		public bool ShowSelectAll
		{
			get
			{
				return (bool)(ViewState["ShowSelectAll"] ?? false);
			}
			set
			{
				ViewState["ShowSelectAll"] = value;
			}
		}

		/// <summary>
		/// Text pro výběr všech hodnot. Použije se jen v případě nastavení vlastnosti ShowSelectAll na true.
		/// Podporuje syntaxi $resources.
		/// </summary>		
		public string SelectAllText
		{
			get
			{
				return (string)(ViewState["SelectAllText"] ?? String.Empty);
			}
			set
			{
				ViewState["SelectAllText"] = value;
			}
		}

		/// <summary>
		/// Text zobrazený v případě, že jsou vybrány všechny hodnoty.
		/// Pokud není hodnota vlastnosti nastavena, zobrazí se vybrané hodnoty.
		/// Podporuje syntaxi $resources.
		/// </summary>		
		public string AllSelectedText
		{
			get
			{
				return (string)(ViewState["AllSelectedText"] ?? String.Empty);
			}
			set
			{
				ViewState["AllSelectedText"] = value;
			}
		}

		/// <summary>
		/// Text zobrazovaný, pokud není vybrána žádná hodnota.
		/// Podporuje syntaxi $resources.
		/// </summary>
		public string PlaceHolder
		{
			get
			{
				return (string)(ViewState["PlaceHolder"] ?? String.Empty);
			}
			set
			{
				ViewState["PlaceHolder"] = value;
			}
		}

		/// <summary>
		/// Pokud je uvedeno, řadí se položky za sebe (float) s tím, že každá má šířku dle hodnoty této vlastnosti.
		/// Pokud není hodnota nastavena, řadí se položky pod sebe.
		/// </summary>
		public Unit ItemWidth
		{
			get
			{
				return (Unit)(ViewState["ItemWidth"] ?? Unit.Empty);
			}
			set
			{
				ViewState["ItemWidth"] = value;				
			}
		}

		/// <summary>
		/// Pokud je true, bude po autopostbacku stále otevřen.
		/// </summary>
		public bool LeaveOpenInAutoPostBack
		{
			get
			{
				return (bool)(ViewState["LeaveOpenInAutoPostBack"] ?? false);
			}
			set
			{
				ViewState["LeaveOpenInAutoPostBack"] = value;
			}
		}

		/// <summary>
		/// Zobrazí inline filter položek checkbox listu.
		/// </summary>
		public bool ShowFilter
		{
			get
			{
				return (bool)(ViewState["ShowFilter"] ?? false);
			}
			set
			{
				ViewState["ShowFilter"] = value;
			}
		}

		/// <summary>
		/// Text, který se zobrazí, pokud při vyhledávání (viz ShowFilter) není nalezena žádná položka.
		/// </summary>
		public string NoMatchesFoundText
		{
			get
			{
				return (string)(ViewState["NoMatchesFoundText"] ?? String.Empty);
			}
			set
			{
				ViewState["NoMatchesFoundText"] = value;
			}
		}

		/// <summary>
		/// Klientský kód pro obsluhu kliknutí na javascriptový dropdown.
		/// </summary>
		public string OnClientDropDownClick
		{
			get
			{
				return (string)(ViewState["OnClientDropDownClick"] ?? String.Empty);
			}
			set
			{
				ViewState["OnClientDropDownClick"] = value;
			}
		}

		/// <summary>
		/// Klientský kód pro obsluhu opuštění javascriptového dropdownu.
		/// </summary>
		public string OnClientDropDownBlur
		{
			get
			{
				return (string)(ViewState["OnClientDropDownBlur"] ?? String.Empty);
			}
			set
			{
				ViewState["OnClientDropDownBlur"] = value;
			}
		}

		/// <summary>
		/// Klientský kód pro obsluhu otevření javascriptového dropdownu.
		/// </summary>
		public string OnClientDropDownOpen
		{
			get
			{
				return (string)(ViewState["OnClientDropDownOpen"] ?? String.Empty);
			}
			set
			{
				ViewState["OnClientDropDownOpen"] = value;
			}
		}

		/// <summary>
		/// Klientský kód pro obsluhu uzavření javascriptového dropdownu.
		/// </summary>
		public string OnClientDropDownClose
		{
			get
			{
				return (string)(ViewState["OnClientDropDownClose"] ?? String.Empty);
			}
			set
			{
				ViewState["OnClientDropDownClose"] = value;
			}
		}

		/// <summary>
		/// Indikuje, zda je povolen atribut "disabled". Vrací vždy true (na rozdíl od výchozí hodnoty .NET Frameworku, která vrací true jen při režimu kompatibility před 4.0.
		/// </summary>
		public override bool SupportsDisabledAttribute
		{
			get
			{				
				return true;
			}

		}

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public CheckBoxDropDownList()
		{
			SelectionMode = ListSelectionMode.Multiple;
		}

		/// <summary>
		/// Occurs when the selection from the list control changes between posts to the server.
		/// </summary>
		protected override void OnSelectedIndexChanged(EventArgs e)
		{
			base.OnSelectedIndexChanged(e);

			if (AutoPostBack && LeaveOpenInAutoPostBack)
			{
				renderIsOpen = true;
			}
		}

		/// <summary>
		/// Zajišťuje validaci vlastností controlu a registraci klientských skriptů.
		/// </summary>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			ValidateControlProperties();

			ClientScripts.HavitFrameworkClientScriptHelper.RegisterHavitFrameworkClientScript(this.Page);
			ScriptManager.ScriptResourceMapping.EnsureScriptRegistration(this.Page, HavitFrameworkClientScriptHelper.JQueryMultipleSelectResourceMappingName);
			ScriptManager.RegisterStartupScript(this.Page, typeof(CheckBoxDropDownList), "Startup", "havitDropDownCheckBoxListExtensions.init();", true);
		}

		/// <summary>
		/// Validuje hodnoty vlastností.
		/// </summary>
		private void ValidateControlProperties()
		{
			if ((ItemWidth != Unit.Empty) && (ItemWidth.Type != UnitType.Pixel))
			{
				throw new HttpException(String.Format("Hodnota vlastnost ItemWidth controlu '{0}' musí být nastavena v pixelech.", this.ID));
			}

			if (SelectionMode == ListSelectionMode.Single)
			{
				throw new HttpException(String.Format("Hodnota vlastnosti SelectionMode controlu '{0}' je nastavena na Single, což není podporováno.", this.ID));
			}
		}

		/// <summary>
		/// Zajišťuje vyrenderování html atributů pro klienský skript dropdowncheckboxlistu.
		/// </summary>
		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			Unit originalWidth = Width; // jsme v render, viewstate neřešíme
			Width = Unit.Empty;

			base.AddAttributesToRender(writer);

			writer.AddAttribute("data-dropdowncheckboxlist", "true");
			writer.AddAttribute("data-dropdowncheckboxlist-showselectall", ShowSelectAll.ToString().ToLower());

			if (!String.IsNullOrEmpty(SelectAllText))
			{
				writer.AddAttribute("data-dropdowncheckboxlist-selectalltext", HttpUtilityExt.GetResourceString(SelectAllText));
			}

			if (renderIsOpen)
			{
				writer.AddAttribute("data-dropdowncheckboxlist-isopen", "true");
			}

			if (!String.IsNullOrEmpty(AllSelectedText))
			{
				writer.AddAttribute("data-dropdowncheckboxlist-allselectedtext", HttpUtilityExt.GetResourceString(AllSelectedText));
			}

			if (!String.IsNullOrEmpty(PlaceHolder))
			{
				writer.AddAttribute("data-dropdowncheckboxlist-placeholder", HttpUtilityExt.GetResourceString(PlaceHolder));
			}

			if (originalWidth != Unit.Empty)
			{
				writer.AddAttribute("data-dropdowncheckboxlist-width", originalWidth.ToString());
			}

			if (ItemWidth != Unit.Empty)
			{
				writer.AddAttribute("data-dropdowncheckboxlist-itemwidth", ((int)ItemWidth.Value).ToString());
			}

			if (ShowFilter)
			{
				writer.AddAttribute("data-dropdowncheckboxlist-filter", "true");
			}

			if (!String.IsNullOrEmpty(NoMatchesFoundText))
			{
				writer.AddAttribute("data-dropdowncheckboxlist-nomatchesfound", NoMatchesFoundText);
			}

			if (!String.IsNullOrEmpty(OnClientDropDownBlur))
			{
				writer.AddAttribute("data-dropdowncheckboxlist-onblurscript", OnClientDropDownBlur);
			}

			if (!String.IsNullOrEmpty(OnClientDropDownClick))
			{
				writer.AddAttribute("data-dropdowncheckboxlist-onclickscript", OnClientDropDownClick);
			}

			if (!String.IsNullOrEmpty(OnClientDropDownOpen))
			{
				writer.AddAttribute("data-dropdowncheckboxlist-onopenscript", OnClientDropDownOpen);
			}

			if (!String.IsNullOrEmpty(OnClientDropDownClose))
			{
				writer.AddAttribute("data-dropdowncheckboxlist-onclosescript", OnClientDropDownClose);
			}

			if (SelectionMode == ListSelectionMode.Single)
			{
				writer.AddAttribute("data-dropdowncheckboxlist-single", "true");
			}

		}
	}
}
