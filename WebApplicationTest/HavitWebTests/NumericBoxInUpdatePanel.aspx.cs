using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.WebApplicationTest.HavitWebTests
{
	public partial class NumericBoxInUpdatePanel : System.Web.UI.Page
	{
		#region OnInit
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			AsyncPostButton.Click += AsyncPostButton_Click;
		}
		#endregion

		#region OnLoad
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (!IsPostBack)
			{
				NumericBoxInUP.Visible = false;
			}
		}
		#endregion

		#region AsyncPostButton_Click
		private void AsyncPostButton_Click(object sender, EventArgs e)
		{
			NumericBoxInUP.Visible = true;
		}
		#endregion
	}
}