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
	public class ReleaseOnUnloadTest1 : IHttpHandler
	{
		private readonly IDisposableComponent disposableComponent;

		public ReleaseOnUnloadTest1(IDisposableComponent disposableComponent)
		{
			this.disposableComponent = disposableComponent;
		}

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			context.Response.Write(disposableComponent.Hello());
		}
	}
}