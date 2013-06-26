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
using System.Web.Services;
using System.Collections.Generic;
using Havit.Web.UI.WebControls;
using Havit.BusinessLayerTest;
using Havit.Business.Query;
using System.Linq;

namespace WebApplicationTest
{
	public partial class TextBoxTest_aspx : System.Web.UI.Page
	{
		#region OnInit
		protected override void OnInit(EventArgs e)
		{
			TextBox1.TextChanged += TextBox1_TextChanged;
			TextBox2.TextChanged += TextBox2_TextChanged;
		}
		#endregion

		#region TextBox1_TextChanged
		private void TextBox1_TextChanged(object sender, EventArgs e)
		{
			Label1.Text = TextBox1.Text;
		}
		#endregion

		#region TextBox2_TextChanged
		private void TextBox2_TextChanged(object sender, EventArgs e)
		{
			Label2.Text = TextBox2.Text;
		}
		#endregion

	}
}
