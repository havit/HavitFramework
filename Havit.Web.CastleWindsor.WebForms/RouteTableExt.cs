using System.Web.Routing;

namespace Havit.Web.CastleWindsor.WebForms
{
	/// <summary>
	/// Extension metody pro routování
	/// </summary>
	public static class RouteTableExt
	{
		/// <summary>
		/// Vytvoří novou route na .aspx pomocí DependencyInjectionPageRouteHandler
		/// </summary>
		public static void MapDIPageRoute(this RouteCollection routes, string name, string urlPattern, string filePath, bool checkPhysicalAccess = true)
		{
			routes.Add(name, new Route(urlPattern, new DependencyInjectionPageRouteHandler(filePath, checkPhysicalAccess)));
		}
	}
}