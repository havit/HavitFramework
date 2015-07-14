using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Web;
using System.Web.Services;

namespace Havit.CastleWindsor.WebForms
{
	/// <summary>
	/// Bázová třída pro ASMX web services, která si umí nainjektovat závislosty s [Inject]
	/// </summary>
	public abstract class InjectableWebServiceBase : WebService
	{
		/// <summary>
		/// Konstruktor. Zajišťuje vyhodnocení závislostí.
		/// </summary>
		public InjectableWebServiceBase()
		{
			DependencyInjectionWebFormsHelper.InitializeInstance(this);
		}

		/// <summary>
		/// Uvolní získané závislosti.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (disposing)
			{
				DependencyInjectionWebFormsHelper.ReleaseDependencies(this);				
			}
		}
	}
}