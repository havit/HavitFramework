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
	public class ReleaseAllOnUnloadTest : InjectableGenericHandlerBase
	{
		#region DisposableComponent
		[Inject]
		public IDisposableComponent[] DisposableComponents { get; set; }
		#endregion

		#region DoProcessRequest
		protected override void DoProcessRequest(HttpContext context)
		{
			foreach (IDisposableComponent disposableComponent in DisposableComponents)
			{
				context.Response.Write(disposableComponent.Hello());
				context.Response.Write("<br />");
			}
		}
		#endregion
	}
}