using Havit.Web.UI.WebControls;
using Havit.Web.UI.WebControls.ControlsValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

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

		#region MyRegion
		private void Test1ACTB_ValueChanged(object sender, EventArgs e)
		{
			Messenger.Default.AddMessage("Postback");
		}
 		#endregion

		void PersisterBtn_Click(object sender, EventArgs e)
		{
			ControlsValuesHolder holder = PersisterCVP.RetrieveValues();

			PersisterCVP.ApplyValues(holder);
		}
	}
}