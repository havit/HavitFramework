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
using Havit.BusinessLayerTest;
using Havit.Web.UI.WebControls;
using Havit.Web.UI;

namespace WebApplicationTest
{
	public partial class GridViewExtTest : System.Web.UI.Page
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			TestedGV.DataBinding += new EventHandler(TestedGV_DataBinding);
			TestedGV.RowCustomizingCommandButton += new GridViewRowCustomizingCommandButtonEventHandler(TestedGV_RowCustomizingCommandButton);
		}

		void TestedGV_RowCustomizingCommandButton(object sender, GridViewRowCustomizingCommandButtonEventArgs e)
		{
			if ((e.CommandName == CommandNames.Delete) && (e.RowIndex == 1))
			{
				e.Enabled = false;
			}
		}

		void TestedGV_DataBinding(object sender, EventArgs e)
		{
			TestedGV.DataSource = Subjekt.GetAll();
		}
	}
}
