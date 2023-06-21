using Havit.BusinessLayerTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.WebApplicationTest.HavitWebTests;

public partial class FormViewTest : System.Web.UI.Page
{
	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);
		MyFormView.DataBinding += MyFormView_DataBinding;
		MyFormView.ItemUpdating += MyFormView_ItemUpdating;
	}

	private void MyFormView_ItemUpdating(object sender, FormViewUpdateEventArgs e)
	{
		Uzivatel editedUzivatel = Uzivatel.GetObject(10);
		MyFormView.ExtractValues(editedUzivatel);
		editedUzivatel.Save();
	}

	private void MyFormView_DataBinding(object sender, EventArgs e)
	{
		MyFormView.DataSource = Uzivatel.GetObject(10);
	}
}