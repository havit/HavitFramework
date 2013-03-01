using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Threading;

namespace Havit.DsvCommerce.Web
{
	public partial class TestPage : Page
	{
		#region OnInit
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			ServerOpen1Button.Click += new EventHandler(ServerOpen1Button_Click);
			ServerClose1Button.Click += new EventHandler(ServerClose1Button_Click);

			ServerOpen2Button.Click += new EventHandler(ServerOpen2Button_Click);
			ServerClose2Button.Click += new EventHandler(ServerClose2Button_Click);
			CloseOpenButton.Click += new EventHandler(CloseOpenButton_Click);

			EnlargeButton.Click += new EventHandler(EnlargeButton_Click);
		}
		#endregion

		#region OnLoad
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.DataBind();
		}
		#endregion

		#region ServerOpen1Button_Click
		private void ServerOpen1Button_Click(object sender, EventArgs e)
		{
			MyBasicModalDialog.Show();
		}
		#endregion

		#region ServerClose1Button_Click
		private void ServerClose1Button_Click(object sender, EventArgs e)
		{
			MyBasicModalDialog.Hide();
		}
		#endregion

		#region ServerOpen2Button_Click
		private void ServerOpen2Button_Click(object sender, EventArgs e)
		{
			MyAjaxModalDialog.Show();
		}
		#endregion

		#region ServerClose2Button_Click
		private void ServerClose2Button_Click(object sender, EventArgs e)
		{
			MyAjaxModalDialog.Hide();
		}
		#endregion

		#region CloseOpenButton_Click
		private void CloseOpenButton_Click(object sender, EventArgs e)
		{
			MyAjaxModalDialog.Hide();
			MyAjaxModalDialog.Show();
		}
		#endregion

		#region EnlargeButton_Click
		private void EnlargeButton_Click(object sender, EventArgs e)
		{
			MyAjaxModalDialog.Width = new Unit(MyAjaxModalDialog.Width.Value + 10, UnitType.Pixel);
			MyAjaxModalDialog.Height = new Unit(MyAjaxModalDialog.Height.Value + 10, UnitType.Pixel);
		}
		#endregion

	}
}
