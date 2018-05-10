using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using Havit.CastleWindsor.WebForms;

namespace Havit.WebApplicationTest.HavitCastleWindsorWebFormsTests
{
	/// <summary>
	/// Testovadlo pro korektní release závislostí ashx handleru
	/// </summary>
	[SuppressMessage("StyleCop.Analyzers", "SA1649", Justification = "Máme ASPX a ASHX stejně pojmenované, potřebujeme různě pojmenované třídy, proto soubor neodpovídá třídě.")]
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