using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Havit.CastleWindsor.WebForms;

namespace Havit.WebApplicationTest.HavitCastleWindsorWebFormsTests
{
	public partial class Nested : System.Web.UI.MasterPage
	{
		public IDisposableComponent DisposableComponent { get; } 

		public Nested(IDisposableComponent disposableComponent)
		{
			this.DisposableComponent = disposableComponent;
		}
	}
}