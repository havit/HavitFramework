using System;
using System.Web.UI;
using Havit.CastleWindsor.WebForms;

namespace Havit.WebApplicationTest.HavitCastleWindsorWebFormsTests
{
	public partial class DynamicallyLoadedControlTest : System.Web.UI.Page
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			Control loadedControl = this.LoadControl("./ReleaseOnUnloadControlTest.ascx");
			DependencyInjectionWebFormsHelper.InitializeControl(loadedControl);
			MyPH.Controls.Add(loadedControl);
		}
	}
}