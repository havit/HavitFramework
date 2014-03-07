using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Havit.BusinessLayerTest;

namespace WebApplicationTest.HavitWebTests
{
	public partial class GridViewExtTest_InnerGVControl : System.Web.UI.UserControl
	{
		#region OnInit
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			InnerGV.DataBinding += new EventHandler(InnerGV_DataBinding);
			InnerGV.GetInsertRowDataItem += InnerGV_GetInsertRowDataItem;
			InnerGV.RowInserting += new Havit.Web.UI.WebControls.GridViewInsertEventHandler(InnerGV_RowInserting);
			InnerGV.RowEditing += new GridViewEditEventHandler(InnerGV_RowEditing);
			InnerGV.RowDeleting += new GridViewDeleteEventHandler(InnerGV_RowDeleting);
		}
		#endregion

		#region InnerGV_RowDeleting
		private void InnerGV_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{
			Trace.Write("InnerGV_RowDeleting");
		}
		#endregion

		#region InnerGV_RowEditing
		private void InnerGV_RowEditing(object sender, GridViewEditEventArgs e)
		{
			Trace.Write("InnerGV_RowEditing");
		}
		#endregion

		#region InnerGV_RowInserting
		private void InnerGV_RowInserting(object sender, Havit.Web.UI.WebControls.GridViewInsertEventArgs e)
		{
			Trace.Write("InnerGV_RowInserting");
		}
		#endregion

		#region InnerGV_DataBinding
		private void InnerGV_DataBinding(object sender, EventArgs e)
		{
			InnerGV.DataSource = Subjekt.GetAll().Take(5).ToList();
		}
		#endregion

		#region InnerGV_GetInsertRowDataItem
		protected object InnerGV_GetInsertRowDataItem()
		{
			return Subjekt.CreateObject();
		}
		#endregion

	}
}