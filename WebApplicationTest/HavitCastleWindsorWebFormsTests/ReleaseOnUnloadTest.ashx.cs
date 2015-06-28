using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Havit.CastleWindsor.WebForms;

namespace Havit.WebApplicationTest.HavitCastleWindsorWebFormsTests
{
	/// <summary>
	/// Testovadlo pro korektní release závislostí ashx handleru
	/// </summary>
	public class ReleaseOnUnloadTest1 : InjectableGenericHandlerBase
	{
		#region DisposableComponent
		[Inject]
		public IDisposableComponent DisposableComponent { get; set; }
		#endregion

		#region DoProcessRequest
		protected override void DoProcessRequest(HttpContext context)
		{
			context.Response.Write(DisposableComponent.Hello());
		}
		#endregion
	}
}