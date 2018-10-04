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
using System.Linq;

namespace Havit.WebApplicationTest.HavitWebTests
{
	public partial class AutoSuggestMenu_aspx : System.Web.UI.Page
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			SubjektLabel.Text = SubjektASM.SelectedValue;
			TimestampLabel.Text = Convert.ToString(DateTime.Now);

			MyTemplateField.Visible = false;
			if (!Page.IsPostBack)
			{
				MyGridView.DataSource = new int[] { 1, 2, 3 };
				MyGridView.DataBind();
			}
		}
	}
}
