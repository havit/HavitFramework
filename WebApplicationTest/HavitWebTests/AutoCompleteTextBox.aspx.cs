using Havit.Web.UI.WebControls;
using Havit.Web.UI.WebControls.ControlsValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace WebApplicationTest.HavitWebTests
{
	public partial class AutoCompleteTextBox : System.Web.UI.Page
	{
		#region OnInit
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			Test1ACTB.SelectedValueChanged += Test1ACTB_ValueChanged;
			PersisterBtn.Click += PersisterBtn_Click;
		}
		#endregion

		#region Test1ACTB_ValueChanged
		private void Test1ACTB_ValueChanged(object sender, EventArgs e)
		{
			Random r = new Random();
			Test1ACTB.Context = r.Next(10).ToString("n2");

			Messenger.Default.AddMessage("Postback");
		}
		#endregion

		#region PersisterBtn_Click
		private void PersisterBtn_Click(object sender, EventArgs e)
		{
			ControlsValuesHolder holder = PersisterCVP.RetrieveValues();
			XmlDocument data = holder.ToXmlDocument();

			PersisterOutputTB.Text = data.OuterXml;

			ControlsValuesHolder holder2 = ControlsValuesHolder.FromXmlDocument(data);
			PersisterCVP.ApplyValues(holder2);
		}
		#endregion
	}
}