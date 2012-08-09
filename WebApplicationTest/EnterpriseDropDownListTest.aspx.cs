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

namespace WebApplicationTest
{
	public partial class EnterpriseDropDownListTest : System.Web.UI.Page
	{
		protected Havit.Web.UI.WebControls.EnterpriseGridView Test1GV;
		protected Havit.Web.UI.WebControls.EnterpriseGridView Test2GV;
		protected Repeater TestRepeater;

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			Test1GV.DataBinding += new EventHandler(TestGV_DataBinding);
			Test2GV.DataBinding += new EventHandler(TestGV_DataBinding);
		}

		void TestGV_DataBinding(object sender, EventArgs e)
		{
			((Havit.Web.UI.WebControls.EnterpriseGridView)sender).DataSource = Subjekt.GetAll();
			
		}
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			Test1GV.DataBind();

			TestRepeater.DataSource = Subjekt.GetAll();
			TestRepeater.DataBind();
		}
}
}
