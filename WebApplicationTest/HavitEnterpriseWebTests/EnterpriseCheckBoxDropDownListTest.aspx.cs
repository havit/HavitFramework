using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Havit.Web.UI.WebControls;
using Havit.BusinessLayerTest;

namespace Havit.WebApplicationTest.HavitEnterpriseWebTests;

public partial class EnterpriseDropDownCheckBoxListTest : System.Web.UI.Page
{
	protected EnterpriseCheckBoxDropDownList E1;

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);

		if (!Page.IsPostBack)
		{
			E1.DataSource = Role.GetAll().FindAll(delegate(Role role) { return role.ID < 2; });
			E1.DataBind();
			E1.SelectedObjects = Role.GetAll();

			// JK: Zrušeno, metoda přejmenována a viditelnost změněna na internal.
			// Vybere roli 1 a 3. Role 5 neexistuje.
			//int[] abc = { 1, 3, 5 };
			//E2.SelectExistingItems(abc);

		}
	}
}
