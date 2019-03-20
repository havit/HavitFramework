using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Havit.Web;

namespace Havit.WebApplicationTest.HavitWebTests
{
	public partial class PageNavigator_A_Test : System.Web.UI.Page
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			BackButton.Click += BackButton_Click;
			ToBButton.Click += ToBButton_Click;
			BackButton.Enabled = PageNavigator.Current.CanNavigateBack();			
		}

		private void BackButton_Click(object sender, EventArgs e)
		{
			PageNavigator.Current.NavigateBack();
		}

		private void ToBButton_Click(object sender, EventArgs e)
		{
			PageNavigator.Current.NavigateFromRawUrlTo("PageNavigator_B_Test.aspx");
		}
	}
}