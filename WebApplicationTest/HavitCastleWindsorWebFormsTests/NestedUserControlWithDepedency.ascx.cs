using System;
using Havit.CastleWindsor.WebForms;
using Havit.WebApplicationTest.HavitCastleWindsorWebFormsTests;

namespace Havit.WebApplicationTest.HavitCastleWindsorWebFormsTests
{
	public partial class NestedUserControlWithDepedency : System.Web.UI.UserControl
	{
		private readonly IDisposableComponent disposableComponent;

		public NestedUserControlWithDepedency(IDisposableComponent disposableComponent)
		{
			this.disposableComponent = disposableComponent;
		}

		#region OnLoad
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			MyLabel.Text = disposableComponent.Hello();
		}
		#endregion
	}
}