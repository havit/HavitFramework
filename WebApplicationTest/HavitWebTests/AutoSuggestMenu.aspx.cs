﻿using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Services;
using System.Collections.Generic;
using Havit.Web.UI.WebControls;
using Havit.BusinessLayerTest;
using Havit.Business.Query;
using System.Linq;

namespace Havit.WebApplicationTest.HavitWebTests;

public partial class AutoSuggestMenu_aspx : System.Web.UI.Page
{
	public int Counter
	{
		get => (int)(ViewState["Counter"] ?? 0);
		set => ViewState["Counter"] = value;
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		Counter += 1;

		TimestampLabel.Text = Convert.ToString(DateTime.Now);
		CounterLabel.Text = Counter.ToString();
		
		SubjektLabel.Text = SubjektASM.SelectedValue;
		
		AsyncLabel.Text = Page.IsPostBack
			? (ScriptManager.GetCurrent(this).IsInAsyncPostBack ? "Async PostBack" : "Classic PostBack")
			: "GET";

		if (!Page.IsPostBack)
		{
			MyRepeater.DataSource = new int[] { 1, 2, 3 };
			MyRepeater.DataBind();

			MyGridView.DataSource = new int[] { 1, 2, 3 };
			MyGridView.DataBind();
		}
	}
}
