using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication2
{
	public partial class Default : System.Web.UI.Page
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			try
			{
				Response.Write(DateTime.Now.ToString());
				Response.Write(" ");
				using var sqlConnection = new SqlConnection("server=tcp:kandadbserver.database.windows.net;database=VolejbalDb;Uid=AzureManagedIdentity;Authentication=Active Directory Interactive");
				sqlConnection.Open();
				Response.Write(new SqlCommand("SELECT COUNT(*) FROM Termin", sqlConnection).ExecuteScalar().ToString());
				Response.Write(" ");
				Response.Write("OK");
			}
			catch (Exception ex)
			{
				Response.Write(ex.ToString());
			}
			Response.Flush();
			Response.End();

		}
	}
}