using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Havit.BusinessLayerTest;
using System.Diagnostics;

namespace Havit.WebApplicationTest.HavitWebTests;

public partial class NumericBoxTest : System.Web.UI.Page
{
	protected override void OnInit(EventArgs e)
	{
		base.OnInit(e);
		ExtractButton.Click += ExtractButton_Click;
	}

	private void ExtractButton_Click(object sender, EventArgs e)
	{			
		DataClass data = new DataClass();
		MyFormView.ExtractValues(data);
		Debugger.Break();
	}

	protected override void OnLoad(EventArgs e)
	{
		if (!Page.IsPostBack)
		{
			MyFormView.DataSource = new DataClass();
			MyFormView.DataBind();
		}
	}

	protected override void OnPreRender(EventArgs e)
	{
		base.OnPreRender(e);
		if (Page.IsPostBack)
		{
			CallBackButton.Text = Page.IsValid.ToString();
		}
	}

	public class DataClass
	{
		public int IntegerValue { get; set; }
		public int? NullableIntegerValue { get; set; }
	}
}
