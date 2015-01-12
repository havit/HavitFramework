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
	public class DropDownCheckBoxList : ListBoxExt
	{
		#region ShowSelectAll
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
		#endregion

		#region AllSelectedText
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
		#endregion

		#region PlaceHolder
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
		#endregion

		#region ItemWidth
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
		#endregion

		#region SupportsDisabledAttribute
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
		#endregion

		#region Constructor
		/// <summary>
		/// Konstruktor.
		/// </summary>
		public DropDownCheckBoxList()
		{
			SelectionMode = ListSelectionMode.Multiple;
		}
		#endregion

		#region OnPreRender
		/// <summary>
		/// Zajišťuje validaci vlastností controlu a registraci klientských skriptů.
		/// </summary>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			ValidateControlProperties();

			ClientScripts.HavitFrameworkClientScriptHelper.RegisterHavitFrameworkClientScript(this.Page);
			ScriptManager.ScriptResourceMapping.EnsureScriptRegistration(this.Page, "jquery.multipleselect");
			ScriptManager.RegisterStartupScript(this.Page, typeof(DropDownCheckBoxList), "Startup", "havitDropDownCheckBoxListExtensions.init();", true);
			ScriptManager.RegisterOnSubmitStatement(this.Page, typeof(DropDownCheckBoxList), "OnSubmit", "havitDropDownCheckBoxListExtensions.beforeSubmit();");
		}
		#endregion

		#region ValidateControlProperties
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
		#endregion

		#region AddAttributesToRender
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

			if (SelectionMode == ListSelectionMode.Single)
			{
				writer.AddAttribute("data-dropdowncheckboxlist-single", "true");
			}

		}
		#endregion
	}
}
