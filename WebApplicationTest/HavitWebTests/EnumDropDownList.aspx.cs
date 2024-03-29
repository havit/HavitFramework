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

namespace Havit.WebApplicationTest.HavitWebTests;

public partial class EnumDropDownList_aspx : System.Web.UI.Page
{
	protected GridView MainGridView;

	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);

		TestDDL.ItemDataBound += new EventHandler<Havit.Web.UI.WebControls.ListControlItemDataBoundEventArgs>(TestDDL_ItemDataBound);
		TestBt.Click += new EventHandler(TestBt_Click);
	}

	private void TestBt_Click(object sender, EventArgs e)
	{
		if (TestDDL.SelectedEnumValue == null)
		{
			TestLb.Text = "null";
		}
		else
		{
			TestLb.Text = ((TestEnum)TestDDL.SelectedEnumValue).ToString();
		}

		TestDDL.SelectedEnumValue = TestEnum.EnumHodnota2;
	}

	private void TestDDL_ItemDataBound(object sender, Havit.Web.UI.WebControls.ListControlItemDataBoundEventArgs e)
	{
		TestEnum item = (TestEnum)e.DataItem;
		// e.Item.Text = item.ToString("d");
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		MainGridView.DataSource = new string[] { "a", "b" };
		MainGridView.DataBind();
	}
}

public enum TestEnum
{
	EnumHodnota1 = 1,
	EnumHodnota2 = 2,
	EnumHodnota3 = 3
}
