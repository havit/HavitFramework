using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Web;
using System.Web.Services;

namespace Havit.CastleWindsor.WebForms
{
	/// <summary>
	/// Bázová třída pro generické handlery, která si umí nainjektovat závislosty s [Inject]
	/// </summary>
	public abstract class InjectableWebServiceBase : WebService
	{
		public InjectableWebServiceBase()
		{
			DependencyInjectionWebFormsHelper.InitializeInstance(this);
		}		
		
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