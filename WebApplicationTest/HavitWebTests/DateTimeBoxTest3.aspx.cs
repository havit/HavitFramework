using Havit.Web.UI.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.WebApplicationTest.HavitWebTests
{
	public partial class DateTimeBoxTest3 : System.Web.UI.Page
	{
		protected DateTimeBox dateTimeBox;

		protected override void OnLoad(EventArgs e)
		{
			dateTimeBox = new DateTimeBox();
			dateTimeBox.ID = "MyTextBox";
			ContentPanel.Controls.Add(dateTimeBox);
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			ValueLabel.Text = dateTimeBox.Value?.ToString() ?? "null";
		}
	}
}