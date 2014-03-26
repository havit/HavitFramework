﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplicationTest.HavitWebBootstrapTests
{
	public partial class CheckBoxListListTest : System.Web.UI.Page
	{
		#region OnInit
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			DynamicalyAddItemButton.Click += DynamicalyAddItemButton_Click;
		}
		#endregion

		#region DynamicalyAddItemButton_Click
		private void DynamicalyAddItemButton_Click(object sender, EventArgs e)
		{
			HorizontalCheckBoxList.Items.Add((HorizontalCheckBoxList.Items.Count + 1).ToString());
			VerticalCheckBoxList.Items.Add((VerticalCheckBoxList.Items.Count + 1).ToString());
		}
		#endregion
	}
}