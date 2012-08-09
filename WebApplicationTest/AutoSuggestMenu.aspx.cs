using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Services;
using System.Collections.Generic;
using Havit.Web.UI.WebControls;
using Havit.BusinessLayerTest;
using Havit.Business.Query;

namespace WebApplicationTest
{
	public partial class AutoSuggestMenu_aspx : System.Web.UI.Page
	{
		[WebMethod()]
		public static string GetSuggestions(string keyword, bool usePaging, int pageIndex, int pageSize)
		{
			QueryParams qp = new QueryParams();
			qp.Conditions.Add(TextCondition.CreateWildcards(Subjekt.Properties.Nazev, keyword));
			qp.TopRecords = pageSize;

			SubjektCollection subjekty = Subjekt.GetList(qp);
			
			List<AutoSuggestMenuItem> menuItems = new List<AutoSuggestMenuItem>(subjekty.Count);
			foreach (Subjekt subjekt in subjekty)
			{
				menuItems.Add(new AutoSuggestMenuItem(subjekt.Nazev, subjekt.ID.ToString()));
			}

			return AutoSuggestMenu.ConvertMenuItemsToJSON(menuItems, menuItems.Count);
		}
	}
}
