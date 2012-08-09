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

namespace WebApplicationTest
{
	public partial class DateTimeBoxTest : System.Web.UI.Page
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			AutoPostBackDateTimeBox.Value = DateTime.Now;
			AutoPostBackDateTimeBox.ValueChanged += new EventHandler(AutoPostBackDateTimeBox_ValueChanged);
			AutoPostBackDateTimeBoxValidator.ServerValidate += new ServerValidateEventHandler(AutoPostBackDateTimeBoxValidator_ServerValidate);
			AutoPostBackOnInitValueLabel.Text = AutoPostBackDateTimeBox.Value.ToString();
		}	

		private void AutoPostBackDateTimeBox_ValueChanged(object sender, EventArgs e)
		{
			ChangedLabel.Text = "yes";
			if (Page.IsValid)
			{
			}
		}

		private void AutoPostBackDateTimeBoxValidator_ServerValidate(object source, ServerValidateEventArgs args)
		{
			AutoPostBackServerValidatorValueLabel.Text = AutoPostBackDateTimeBox.Value.ToString();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			AutoPostBackOnLoadValueLabel.Text = AutoPostBackDateTimeBox.Value.ToString();
			ChangedLabel.Text = "no";
		}

		protected override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			AutoPostBackOnLoadCompleteValueLabel.Text = AutoPostBackDateTimeBox.Value.ToString();
		}
	}
}
