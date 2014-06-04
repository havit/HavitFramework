using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Havit.Web.UI.WebControls;

namespace WebApplicationTest.HavitWebBootstrapTests
{
	using Havit.BusinessLayerTest;

	public partial class GridViewExtTest : System.Web.UI.Page
	{
		#region OnInit
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			MainGV.DataBinding += MainGV_DataBinding;
			MainGV.GetInsertRowDataItem += MainGV_GetInsertRowDataItem;
		}
		#endregion

		#region OnLoad
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!Page.IsPostBack)
			{
				BindValues();
			}
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			MainGV.SetRequiresDatabinding();
		}
		#endregion

		#region MainGV_DataBinding
		private void MainGV_DataBinding(object sender, EventArgs e)
		{
			MainGV.DataSource = Subjekt.GetAll().Take(5).ToList();
		}
		#endregion

		#region MainGV_GetInsertRowDataItem
		private object MainGV_GetInsertRowDataItem()
		{
			return Subjekt.CreateObject();
		}
		#endregion

	}
}