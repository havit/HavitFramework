using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplicationTest.HavitWebBootstrapTests
{
	public partial class Validators : System.Web.UI.Page
	{
		protected void CustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid = args.Value.StartsWith("a", StringComparison.CurrentCultureIgnoreCase);
		}

	}
}