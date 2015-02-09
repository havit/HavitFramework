using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Havit.CastleWindsor.WebForms;

namespace WebApplicationTest.HavitCastleWindsorWebFormsTests
{
	public partial class ReleaseOnUnloadControlTest : System.Web.UI.UserControl
	{
		#region DisposableComponent
		[Inject]
		public IDisposableComponent DisposableComponent { get; set; }
		#endregion

		#region OnLoad
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			MyLabel.Text = DisposableComponent.Hello();
		}
		#endregion
	}
}