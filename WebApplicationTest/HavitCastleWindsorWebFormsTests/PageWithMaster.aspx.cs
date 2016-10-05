using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Havit.CastleWindsor.WebForms;
using Havit.Diagnostics.Contracts;

namespace Havit.WebApplicationTest.HavitCastleWindsorWebFormsTests
{
	public partial class PageWithMaster : System.Web.UI.Page
	{
		[Inject]
		public IDisposableComponent DisposableComponent { get; set; }

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			Contract.Assert(DisposableComponent != null, "Page");

			Nested nesterMaster = (Nested)this.Master;
			Contract.Assert(nesterMaster.DisposableComponent != null, "Nester.master");

			Top topMaster = (Top)nesterMaster.Master;
			Contract.Assert(topMaster.DisposableComponent != null, "Top.Master");
		}
	}
}