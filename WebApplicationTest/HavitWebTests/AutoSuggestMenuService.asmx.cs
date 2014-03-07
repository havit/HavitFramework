using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using Havit.BusinessLayerTest;
using Havit.Web.UI.WebControls;
using Havit.Business.Query;

namespace WebApplicationTest.HavitWebTests
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[ScriptService]
	[System.ComponentModel.ToolboxItem(false)]
	public class AutoSuggestMenuService : System.Web.Services.WebService
	{
		[ScriptMethod]
		[WebMethod]
		public string GetSuggestions(string keyword, bool usePaging, int pageIndex, int pageSize, string context)
		{
			if (String.IsNullOrEmpty(keyword))
			{
				return AutoSuggestMenu.ConvertMenuItemsToJSON(new List<AutoSuggestMenuItem>(), 0);
			}

			List<AutoSuggestMenuItem> menuItems = new List<AutoSuggestMenuItem>();
			for (int i = 0; i < 15; i++)
			{
				menuItems.Add(new AutoSuggestMenuItem(keyword + i.ToString(), i.ToString()));
			}

			return AutoSuggestMenu.ConvertMenuItemsToJSON(menuItems.Skip(pageSize * pageIndex).Take(pageSize).ToList(), menuItems.Count);
		}

	}
}
