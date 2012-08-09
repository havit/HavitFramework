using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using Havit.BusinessLayerTest;
using Havit.Web.UI.WebControls;
using Havit.Business.Query;

namespace WebApplicationTest
{
	/// <summary>
	/// Summary description for AutoSuggestMenuService
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[ScriptService]
	[System.ComponentModel.ToolboxItem(false)]
	// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
	// [System.Web.Script.Services.ScriptService]
	public class AutoSuggestMenuService : System.Web.Services.WebService
	{
		[ScriptMethod]
		[WebMethod()]
		public string GetSuggestions(string keyword, bool usePaging, int pageIndex, int pageSize, string context)
		{
			QueryParams qp = new QueryParams();
			qp.Conditions.Add(TextCondition.CreateWildcards(Subjekt.Properties.Nazev, keyword, WildCardsLikeExpressionMode.Contains));
			qp.TopRecords = pageSize;

			SubjektCollection subjekty = Subjekt.GetList(qp);

			List<AutoSuggestMenuItem> menuItems = new List<AutoSuggestMenuItem>(subjekty.Count);

			for (int i = 0; i < 15; i++)
			{
				foreach (Subjekt subjekt in subjekty)
				{
					menuItems.Add(new AutoSuggestMenuItem(subjekt.Nazev + i.ToString(), subjekt.ID.ToString()));
				}
			}

			return AutoSuggestMenu.ConvertMenuItemsToJSON(menuItems.Skip(pageSize * pageIndex).Take(pageSize).ToList(), menuItems.Count);
		}

	}
}
