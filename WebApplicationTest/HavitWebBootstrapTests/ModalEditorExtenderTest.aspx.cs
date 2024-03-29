﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Havit.Web.UI.WebControls;

namespace Havit.WebApplicationTest.HavitWebBootstrapTests;

using Havit.BusinessLayerTest;

public partial class GridViewExtTest : System.Web.UI.Page
{
	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);
		MainGV.DataBinding += MainGV_DataBinding;
		MainGV.GetInsertRowDataItem += MainGV_GetInsertRowDataItem;
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		if (!Page.IsPostBack)
		{
			BindValues();
		}
	}

	private void BindValues()
	{
		MainGV.SetRequiresDatabinding();
	}

	private void MainGV_DataBinding(object sender, EventArgs e)
	{
            if (mainGVDataBindingCalled)
            {
                throw new ApplicationException("Necheme v jednom requestu více databindingů (testujeme VirtualItemsCount a vliv na modaleditorextender)!");
            }
            SubjektCollection subjekty = Subjekt.GetAll().OrderBy(item => item.Nazev).ToCollection();

            MainGV.VirtualItemCount = subjekty.Count;
            MainGV.DataSource = subjekty.Skip(MainGV.PageIndex * MainGV.PageSize).Take(MainGV.PageSize);
            mainGVDataBindingCalled = true;

        }
        private bool mainGVDataBindingCalled = false;

        private object MainGV_GetInsertRowDataItem()
	{
		return Subjekt.CreateObject();
	}
}