using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Havit.Web;

namespace Havit.WebApplicationTest.HavitWebTests
{
	public partial class PageNavigator_B_Test : System.Web.UI.Page
	{
		#region OnInit
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			BackButton.Click += BackButton_Click;
			ToAButton.Click += ToAButton_Click;
			BackButton.Enabled = PageNavigator.Current.CanNavigateBack();

			Response.DisableUserCache();			
		}
		#endregion

		#region BackButton_Click
		private void BackButton_Click(object sender, EventArgs e)
		{
			PageNavigator.Current.NavigateBack();
		}
		#endregion

		#region ToBButton_Click
		private void ToAButton_Click(object sender, EventArgs e)
		{
			PageNavigator.Current.NavigateFromRawUrlTo("PageNavigator_A_Test.aspx");
		}
		#endregion
	}
}