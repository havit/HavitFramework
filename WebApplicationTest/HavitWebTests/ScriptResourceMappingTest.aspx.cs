using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Havit.Web.UI;

namespace Havit.WebApplicationTest.HavitWebTests;

public partial class ScriptResourceMappingTest : System.Web.UI.Page
{
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		ScriptManager.ScriptResourceMapping.EnsureScriptRegistration(this.Page, "jquery");
	}

}