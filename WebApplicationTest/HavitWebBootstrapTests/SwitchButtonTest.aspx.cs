using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Havit.Web.UI;

namespace WebApplicationTest.HavitWebBootstrapTests
{
	public partial class SwitchButtonTest : System.Web.UI.Page
	{
		#region PageStatePersister
		protected override PageStatePersister PageStatePersister
		{
			get
			{
				if (pageStatePersister == null)
				{
					pageStatePersister = new FilePageStatePersister(this, new FilePageStatePersister.PerUserNamingStrategy(@"\\TOPOL\Workspace\002.HFW\ViewState"));
				}
				return pageStatePersister;
			}
		}
		private PageStatePersister pageStatePersister;
		#endregion

		#region OnInit
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			FirstSwitchButton.ValueChanged += FirstSwitchButton_ValueChanged;
			SecondSwitchButton.ValueChanged += SecondSwitchButton_ValueChanged;
		}
		#endregion

		#region FirstSwitchButton_ValueChanged
		private void FirstSwitchButton_ValueChanged(object sender, EventArgs e)
		{
			FirstStateLabel.Text = FirstSwitchButton.Value ? FirstSwitchButton.YesText : FirstSwitchButton.NoText;
		}
		#endregion

		#region SecondSwitchButton_ValueChanged
		private void SecondSwitchButton_ValueChanged(object sender, EventArgs e)
		{
			SecondStateLabel.Text = SecondSwitchButton.Value ? SecondSwitchButton.YesText : SecondSwitchButton.NoText;
		}
		#endregion
	}
}