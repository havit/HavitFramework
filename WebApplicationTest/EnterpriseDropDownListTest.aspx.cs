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

namespace WebApplicationTest
{
	public partial class EnterpriseDropDownListTest : System.Web.UI.Page
	{
		protected Havit.Web.UI.WebControls.EnterpriseGridView Test1GV;
		protected Havit.Web.UI.WebControls.EnterpriseGridView Test2GV;
		protected Repeater TestRepeater;
		protected EnterpriseDropDownList AutoPostBackEDDL;
		protected Label AutoPostBackResultLabel;

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			AutoPostBackEDDL.SelectedIndexChanged += new EventHandler(AutoPostBackEDDL_SelectedIndexChanged);
			Test1GV.DataBinding += new EventHandler(TestGV_DataBinding);
			Test2GV.DataBinding += new EventHandler(TestGV_DataBinding);
		}

		private void AutoPostBackEDDL_SelectedIndexChanged(object sender, EventArgs e)
		{
			AutoPostBackResultLabel.Text = ((Role)AutoPostBackEDDL.SelectedObject).Symbol;
		}

		void TestGV_DataBinding(object sender, EventArgs e)
		{
			((Havit.Web.UI.WebControls.EnterpriseGridView)sender).DataSource = Subjekt.GetAll();
			
		}
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (!Page.IsPostBack)
			{
				AutoPostBackEDDL.DataSource = Role.GetAll();
				AutoPostBackEDDL.DataBind();
			}
			Test1GV.DataBind();

			TestRepeater.DataSource = Subjekt.GetAll();
			TestRepeater.DataBind();
		}
}
}
