using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplicationTest
{
	public partial class WebFormExceptionTest : System.Web.UI.Page
	{
		#region OnInit
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			DoExceptionButton.Click += new EventHandler(DoExceptionButton_Click);

		}
		#endregion

		#region DoExceptionButton_Click
		private void DoExceptionButton_Click(object sender, EventArgs e)
		{
			try
			{
				Test1();
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Moje application exceptionn", ex);
			}
		}
		#endregion

		#region Test1
		private void Test1()
		{
			try
			{
				this.Test2();
			}
			catch (Exception e)
			{
				throw new InvalidCastException("Muj invalid cast exception.", e);
			}
		}
		#endregion

		#region Test2
		private void Test2()
		{
			throw new NullReferenceException("Moje null reference exception.");
		}
		#endregion
	}
}