using Havit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.WebApplicationTest.SystemWebTests
{
	public partial class SmtpTraceListenerTest : System.Web.UI.Page
	{
		#region OnInit
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			ExceptionCodeButton.Click += ExceptionCodeButton_Click;
			ExceptionBackgroundThreadButton.Click += ExceptionBackgroundThreadButton_Click;
			ExceptionTaskButton.Click += ExceptionTaskButton_Click;
		}
		#endregion

		#region ExceptionCodeButton_Click
		private void ExceptionCodeButton_Click(object sender, EventArgs e)
		{
			DoTrace(ExceptionCodeButton.Text);
		}
		#endregion

		#region ExceptionBackgroundThreadButton_Click
		private void ExceptionBackgroundThreadButton_Click(object sender, EventArgs e)
		{
			new Thread(() => { DoTrace(ExceptionBackgroundThreadButton.Text); }).Start();
		}
		#endregion

		#region ExceptionTaskButton_Click
		private void ExceptionTaskButton_Click(object sender, EventArgs e)
		{
			Task.Factory.StartNew(() => { DoTrace(ExceptionTaskButton.Text); });			
		}
		#endregion

		#region DoTrace
		private static void DoTrace(string message)
		{
			try
			{
				throw new ApplicationException(message);
			}
			catch (Exception exception)
			{
				ExceptionTracer.Default.TraceException(exception);
			}
		}
		#endregion
	}
}