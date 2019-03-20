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
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			ExceptionCodeButton.Click += ExceptionCodeButton_Click;
			ExceptionBackgroundThreadButton.Click += ExceptionBackgroundThreadButton_Click;
			ExceptionTaskButton.Click += ExceptionTaskButton_Click;
		}

		private void ExceptionCodeButton_Click(object sender, EventArgs e)
		{
			DoTrace(ExceptionCodeButton.Text);
		}

		private void ExceptionBackgroundThreadButton_Click(object sender, EventArgs e)
		{
			new Thread(() => { DoTrace(ExceptionBackgroundThreadButton.Text); }).Start();
		}

		private void ExceptionTaskButton_Click(object sender, EventArgs e)
		{
			Task.Factory.StartNew(() => { DoTrace(ExceptionTaskButton.Text); });			
		}

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
	}
}