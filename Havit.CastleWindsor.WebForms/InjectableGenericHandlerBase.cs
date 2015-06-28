using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Web;

namespace Havit.CastleWindsor.WebForms
{
	/// <summary>
	/// Bázová třída pro generické handlery, která si umí nainjektovat závislosty s [Inject]
	/// </summary>
	public abstract class InjectableGenericHandlerBase : IHttpHandler
	{
		private readonly ConcurrentDictionary<Type, PropertyInfo[]> _injectablePropertyCache = new ConcurrentDictionary<Type, PropertyInfo[]>();

		/// <summary>
		/// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"/> instance.
		/// </summary>
		/// <returns>
		/// true if the <see cref="T:System.Web.IHttpHandler"/> instance is reusable; otherwise, false.
		/// </returns>
		public virtual bool IsReusable
		{
			get { return false; }
		}

		/// <summary>
		/// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
		/// </summary>
		/// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests. </param>
		public void ProcessRequest(HttpContext context)
		{
			ResolveDependencies();
			DoProcessRequest(context);
			ReleaseDependencies();
		}

		/// <summary>
		/// Zpracuje HTTP požadadek (ekvivalent IHttpHandler.ProcessRequest) po resolve všech závislostí
		/// </summary>
		/// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
		protected abstract void DoProcessRequest(HttpContext context);

		/// <summary>
		/// Resolvne a injektuje všechny závislosti
		/// </summary>
		protected virtual void ResolveDependencies()
		{
			DependencyInjectionWebFormsHelper.InitializeInstance(this, _injectablePropertyCache);
		}

		/// <summary>
		/// Uvolní veškeré závislé komponenty aktivované pomocí WindsorCastlu
		/// </summary>
		protected virtual void ReleaseDependencies()
		{
			DependencyInjectionWebFormsHelper.ReleaseDependencies(this, _injectablePropertyCache);
		}
	}
}