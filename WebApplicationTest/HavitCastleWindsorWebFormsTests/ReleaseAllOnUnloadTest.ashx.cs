using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.Core.Internal;
using Havit.CastleWindsor.WebForms;

namespace Havit.WebApplicationTest.HavitCastleWindsorWebFormsTests
{
	/// <summary>
	/// Testovací handler pro release enumerace (pole) injected objetků.
	/// </summary>
	public class ReleaseAllOnUnloadTest : IHttpHandler
	{
		public readonly IDisposableComponent[] disposableComponents;

		public ReleaseAllOnUnloadTest(IDisposableComponent[] disposableComponents)
		{
			this.disposableComponents = disposableComponents;
		}

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			foreach (IDisposableComponent disposableComponent in disposableComponents)
			{
				context.Response.Write(disposableComponent.Hello());
				context.Response.Write("<br />");
			}
		}
	}
}