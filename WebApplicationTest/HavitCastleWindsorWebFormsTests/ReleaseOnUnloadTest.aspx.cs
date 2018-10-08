using System;
using Havit.CastleWindsor.WebForms;
using Havit.WebApplicationTest.HavitCastleWindsorWebFormsTests;

namespace Havit.WebApplicationTest.HavitCastleWindsorWebFormsTests
{
	public partial class ReleaseOnUnloadTest : System.Web.UI.Page
	{
		private readonly IDisposableComponent disposableComponent;

		public ReleaseOnUnloadTest(IDisposableComponent disposableComponent)
		{
			this.disposableComponent = disposableComponent;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			MyLabel.Text = this.disposableComponent.Hello();
		}
	}
}