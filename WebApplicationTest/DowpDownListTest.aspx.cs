using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;

namespace WebApplicationTest
{
	public partial class DowpDownListTest : System.Web.UI.Page
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			PostbackButton.Click += new EventHandler(PostbackButton_Click);
		}

		void PostbackButton_Click(object sender, EventArgs e)
		{
			HavitDDLExt.SelectedIndex = HavitDDLExt.Items.Count - 2;

			var data = CultureInfo.GetCultures(CultureTypes.AllCultures).ToList();
			data.RemoveRange(0, data.Count);
			HavitDDLExt.DataSource = data;
			HavitDDLExt.ClearSelection();
			HavitDDLExt.DataBind();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (!Page.IsPostBack)
			{
				HavitDDLExt.DataTextField = "EnglishName";
				HavitDDLExt.DataValueField = "EnglishName";

				var data = CultureInfo.GetCultures(CultureTypes.AllCultures).ToList();
				HavitDDLExt.DataSource = data;
				HavitDDLExt.DataBind();
			}
		}
	}
}