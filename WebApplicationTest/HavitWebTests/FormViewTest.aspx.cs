using Havit.BusinessLayerTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.WebApplicationTest.HavitWebTests
{
	public partial class FormViewTest : System.Web.UI.Page
	{
		#region OnInit
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			MyFormView.DataBinding += MyFormView_DataBinding;
			MyFormView.ItemUpdating += MyFormView_ItemUpdating;
		}
		#endregion

		#region MyFormView_ItemUpdating
		private void MyFormView_ItemUpdating(object sender, FormViewUpdateEventArgs e)
		{
			Uzivatel editedUzivatel = Uzivatel.GetObject(10);
			MyFormView.ExtractValues(editedUzivatel);
			editedUzivatel.Save();
		}
		#endregion

		#region MyFormView_DataBinding
		private void MyFormView_DataBinding(object sender, EventArgs e)
		{
			MyFormView.DataSource = Uzivatel.GetObject(10);
		}
		#endregion
	}
}