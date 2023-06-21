using Havit.Web.UI.WebControls.ControlsValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Havit.WebApplicationTest.HavitWebBootstrapTests;

public partial class CollapsiblePanelTestPage1 : System.Web.UI.Page
{
	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);

		SaveButton.Click += new EventHandler(SaveButton_Click);			
		LoadButton.Click += new EventHandler(LoadButton_Click);

		FirstSwitchButton.ValueChanged += FirstSwitchButton_ValueChanged;
		SecondSwitchButton.ValueChanged += SecondSwitchButton_ValueChanged;
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);

		if (!Page.IsPostBack)
		{
			List<string> items = new List<string>() { "a1", "a2", "a3" };
			TestUpdatePanelRepeater3.DataSource = items;
			TestUpdatePanelRepeater3.DataBind();

			HtmlGenericControl collapsiblePanel3H1 = (HtmlGenericControl)CollapsiblePanel3.FindControl("CollapsiblePanel3H1");
			collapsiblePanel3H1.InnerHtml = String.Format("{0} <span class=\"badge\">{1}</span>", "Header", 5);
		}
	}

	protected void GotoBtn_Click(object sender, EventArgs e)
	{
		this.Response.Redirect("~/HavitWebBootstrapTests/CollapsiblePanelTestPage2.aspx");
	}

	protected void PostbackBtn_Click(object sender, EventArgs e)
	{
		// NOOP
	}

	private void SaveButton_Click(object sender, EventArgs e)
	{
		ControlsValuesHolder data = MainControlsValuesPersister.RetrieveValues();
		Session["CollapsiblePanelTestPage1.aspx"] = data;
	}

	private void LoadButton_Click(object sender, EventArgs e)
	{			
		ControlsValuesHolder data = (ControlsValuesHolder)Session["CollapsiblePanelTestPage1.aspx"];
		if (data != null)
		{
			MainControlsValuesPersister.ApplyValues(data);
		}
	}

	private void FirstSwitchButton_ValueChanged(object sender, EventArgs e)
	{
		FirstStateLabel.Text = FirstSwitchButton.Value ? FirstSwitchButton.YesText : FirstSwitchButton.NoText;
	}

	private void SecondSwitchButton_ValueChanged(object sender, EventArgs e)
	{
		SecondStateLabel.Text = SecondSwitchButton.Value ? SecondSwitchButton.YesText : SecondSwitchButton.NoText;
	}

	protected void CollapsiblePanel3_CollapsedStateChanged(object sender, EventArgs e)
	{
		// NOOP
	}
}