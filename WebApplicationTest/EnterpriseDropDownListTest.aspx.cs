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
		#region Protected fields (controls)
		protected Havit.Web.UI.WebControls.EnterpriseGridView Test1GV;
		protected Havit.Web.UI.WebControls.EnterpriseGridView Test2GV;
		protected Repeater TestRepeater;
		protected EnterpriseDropDownList AutoPostBackEDDL;
		protected Label AutoPostBackResultLabel;
		#endregion

		#region OnInit
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			AutoPostBackEDDL.SelectedIndexChanged += new EventHandler(AutoPostBackEDDL_SelectedIndexChanged);
			Test1GV.DataBinding += new EventHandler(TestGV_DataBinding);
			Test2GV.DataBinding += new EventHandler(TestGV_DataBinding);
		}
		#endregion

		#region OnLoad
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
		#endregion

		#region AutoPostBackEDDL_SelectedIndexChanged
		private void AutoPostBackEDDL_SelectedIndexChanged(object sender, EventArgs e)
		{
			AutoPostBackResultLabel.Text = ((Role)AutoPostBackEDDL.SelectedObject).Symbol;
		}
		#endregion

		#region TestGV_DataBinding
		private void TestGV_DataBinding(object sender, EventArgs e)
		{
			((Havit.Web.UI.WebControls.EnterpriseGridView)sender).DataSource = Subjekt.GetAll();

		}
		#endregion
		
}
}
