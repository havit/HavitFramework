using Havit.BusinessLayerTest;
using Havit.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.WebApplicationTest.HavitWebTests
{
	public partial class GridViewExt_Filtr : System.Web.UI.Page
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			MainGridView.DataBinding += MainGridView_DataBinding;
		}

		private void MainGridView_DataBinding(object sender, EventArgs e)
		{
			MainGridView.DataSource = Subjekt.GetAll().ToList();
		}
	}
}