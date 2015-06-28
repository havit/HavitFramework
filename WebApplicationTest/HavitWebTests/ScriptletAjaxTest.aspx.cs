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

namespace Havit.WebApplicationTest.HavitWebTests
{
	public partial class ScriptletAjaxTest1 : System.Web.UI.Page
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (!Page.IsPostBack)
			{
				TestRepeater.DataSource = System.Threading.Thread.CurrentThread.CurrentUICulture.DateTimeFormat.MonthNames;
				TestRepeater.DataBind();

				TestGridView.DataSource = System.Threading.Thread.CurrentThread.CurrentUICulture.DateTimeFormat.MonthNames;
				TestGridView.DataBind();

			}
		}
	}
}
