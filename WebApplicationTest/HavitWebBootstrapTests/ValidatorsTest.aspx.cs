﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.WebApplicationTest.HavitWebBootstrapTests;

public partial class ValidatorsTest : System.Web.UI.Page
{
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		Form.DefaultButton = SectionBButton.UniqueID;
	}

	protected void CustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
	{
		args.IsValid = args.Value.Equals("bla", StringComparison.CurrentCultureIgnoreCase);
	}
}