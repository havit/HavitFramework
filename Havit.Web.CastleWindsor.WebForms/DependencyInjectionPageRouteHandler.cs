using System;
using System.Web;
using System.Web.Compilation;
using System.Web.Routing;

namespace Havit.Web.CastleWindsor.WebForms
{
	/// <summary>
	/// PageRouteHandler pro normální aspx stránky s podporou Windsor Castle. Co jde přes routing, tak nejde přes PageHandlerFactory, tak proto.
	/// </summary>
	public class DependencyInjectionPageRouteHandler : PageRouteHandler
	{

		private readonly DependencyInjectionPageHandlerFactory _pageHandlerFactory = new DependencyInjectionPageHandlerFactory();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Web.Routing.PageRouteHandler"/> class.
		/// </summary>
		/// <param name="virtualPath">The virtual path of the physical file for this <see cref="P:System.Web.Routing.RouteData.Route"/> object. The file must be located in the current application. Therefore, the path must begin with a tilde (~).</param><exception cref="T:System.ArgumentException">The <paramref name="virtualPath"/> parameter is null or is an empty string or does not start with "~/".</exception>
		public DependencyInjectionPageRouteHandler(string virtualPath)
			: base(virtualPath)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Web.Routing.PageRouteHandler"/> class.
		/// </summary>
		/// <param name="virtualPath">The virtual path of the physical file of this <see cref="P:System.Web.Routing.RouteData.Route"/> object. The file must be located in the current application. Therefore, the path must begin with a tilde (~).</param><param name="checkPhysicalUrlAccess">If this property is set to false, authorization rules will be applied to the request URL and not to the URL of the physical page. If this property is set to true, authorization rules will be applied to both the request URL and to the URL of the physical page.</param><exception cref="T:System.ArgumentException">The <paramref name="virtualPath"/> parameter is null or is an empty string or does not start with "~/".</exception>
		public DependencyInjectionPageRouteHandler(string virtualPath, bool checkPhysicalUrlAccess)
			: base(virtualPath, checkPhysicalUrlAccess)
		{
		}

		/// <summary>
		/// Returns the object that processes the request.
		/// </summary>
		/// <returns>
		/// The object that processes the request.
		/// </returns>
		/// <param name="requestContext">An object that encapsulates information about the request.</param><exception cref="T:System.ArgumentNullException">The <paramref name="requestContext"/> parameter is null.</exception>
		/// <remarks>
		/// Okopírováno z bázové třídy, aby mi mohl stačit jen IHttpHandler.
		/// </remarks>
		public override IHttpHandler GetHttpHandler(RequestContext requestContext)
		{
			if (requestContext == null)
			{
				throw new ArgumentNullException("requestContext");
			}
			string virtualPath = this.GetSubstitutedVirtualPath(requestContext);
			int length = virtualPath.IndexOf('?');
			if (length != -1)
			{
				virtualPath = virtualPath.Substring(0, length);
			}

			IHttpHandler handler = (BuildManager.CreateInstanceFromVirtualPath(virtualPath, typeof(IHttpHandler)) as IHttpHandler);
			if (handler == null)
			{
				throw new ApplicationException("Chybný routing");
			}

			_pageHandlerFactory.SetUpDependencyInjections(handler);
			return handler;
		}

	}
}