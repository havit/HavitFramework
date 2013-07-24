using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Havit;
using Havit.Scopes;
using System.Threading.Tasks;
using System.Diagnostics;
using Havit.Business;

namespace WebApplicationTest
{
	public partial class IdentityMapScopeTest : System.Web.UI.Page
	{
		protected override void OnLoad(EventArgs e)
		{
			var files = System.IO.Directory.EnumerateFiles(@"C:\Dev\002.HFW-BLG\References").ToList();
			Response.Write("Starting on thread " + System.Threading.Thread.CurrentThread.ManagedThreadId + ", " + IdentityMapScope.Current.GetHashCode() + "<br/>");

			foreach (string file in files)
			{
				this.RegisterAsyncTask(new PageAsyncTask(async () =>
					{
						byte[] buffer = new byte[1000];
						using (var fs = new System.IO.FileStream(file, FileMode.Open, FileAccess.Read))
						{
							await fs.ReadAsync(buffer, 0, 1000);
							Response.Write("File finished on thread " + System.Threading.Thread.CurrentThread.ManagedThreadId + ", " + IdentityMapScope.Current.GetHashCode() + "<br/>");
						}
					}));
			}

			Response.Write("Finished on thread " + System.Threading.Thread.CurrentThread.ManagedThreadId + ", " + IdentityMapScope.Current.GetHashCode() + "<br/>");

			//foreach (string file in files)
			//{
			//	byte[] buffer = new byte[1000];
			//	using (var fs = new System.IO.FileStream(file, FileMode.Open, FileAccess.Read))
			//	{
			//		fs.ReadAsync(buffer, 0, 1000);
			//		Response.Write("File finished on thread " + System.Threading.Thread.CurrentThread.ManagedThreadId + (HttpContext.Current != null ? "(ok)" : "(PROBLEM)") + " " + MyScopeTest.Current + "<br/>");
			//	}
			//}

			//Task.WhenAll(files.Select(file =>
			//	{
			//		Task task = new Task(() =>
			//	{
			//		using (new MyScopeTest(scope))
			//		{
			//			byte[] buffer = new byte[1000];
			//			using (var fs = new System.IO.FileStream(file, FileMode.Open, FileAccess.Read))
			//			{
			//				fs.Read(buffer, 0, 1000);
			//				Response.Write("File finished on thread " + System.Threading.Thread.CurrentThread.ManagedThreadId + (HttpContext.Current != null ? "(ok)" : "(PROBLEM)") + " " + MyScopeTest.Current + "<br/>");
			//			}
			//		}
			//	});
			//		task.Start();
			//		return task;
			//	}).ToArray());

		}

		//base.OnLoad(e);
		////byte[] buffer = new byte[1000];

		//MyScopeTest scope = new MyScopeTest("A");

		//var files = System.IO.Directory.EnumerateFiles(@"C:\Dev\002.HFW-BLG\References").ToList();
		//foreach (string file in files)
		//{
		//	Response.Write("<br/>");				
		//	RegisterAsyncTask(new PageAsyncTask((sender, x, cb, extraData) => this.DoSomeJob(file), ar =>
		//		{
		//			Response.Write(file + " (endhandler): " + (HttpContext.Current == null));						
		//		}, null, null));
		//}

		//	{
		//		Response.Write(item + " (before): " + (HttpContext.Current == null));
		//		Response.Write("<br/>");

		//		using (var fs = new System.IO.FileStream(item, FileMode.Open, FileAccess.Read))
		//		{
		//			await fs.ReadAsync(buffer, 0, 1000);
		//		}

		//		Response.Write(item + " (after): " + (HttpContext.Current == null));
		//		Response.Write("<br/>");
		//		//Response. =Write(System.Threading.Thread.CurrentThread.ManagedThreadId + " - " + MyScopeTest.Current + "<br/>");
		//		//Response.Write(System.Threading.Thread.CurrentThread.ManagedThreadId + " - " + MyScopeTest.Current + "<br/>");
		//	});

	}

}