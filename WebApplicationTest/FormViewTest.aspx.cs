using Havit.BusinessLayerTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplicationTest
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
			Subjekt editedSubjekt = Subjekt.GetObject(8);
			MyFormView.ExtractValues(editedSubjekt);
			editedSubjekt.Save();
		}
		#endregion

		#region MyFormView_DataBinding
		private void MyFormView_DataBinding(object sender, EventArgs e)
		{
			MyFormView.DataSource = Subjekt.GetObject(8);
		}
		#endregion

	}
}