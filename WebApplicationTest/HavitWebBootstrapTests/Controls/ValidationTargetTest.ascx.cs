﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Havit.Web.Bootstrap.UI.WebControls;

namespace Havit.WebApplicationTest.HavitWebBootstrapTests.Controls;

[ValidationProperty("Text")]
[ValidationDisplayTarget("MyTextBox")]
public partial class ValidationTargetTest : System.Web.UI.UserControl
{
	public string Text
	{
		get { return MyTextBox.Text; }
	}
}