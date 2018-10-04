using System.Web.Routing;

namespace Havit.CastleWindsor.WebForms
{
	/// <summary>
	/// Extension metody pro routování na stránky s využitím Dependency Injection.
	/// </summary>
	public static class RouteTableExt
	{
		/// <summary>
		/// Vytvoří novou route pomocí DependencyInjectionPageRouteHandler
		/// </summary>
		public static void MapDependencyInjectionPageRoute(this RouteCollection routes, string routeName, string routeUrl, string physicalFile)
		{
			MapDependencyInjectionPageRoute(routes, routeName, routeUrl, physicalFile, true, null, null, null);
		}

		/// <summary>
		/// Vytvoří novou route pomocí DependencyInjectionPageRouteHandler
		/// </summary>
		public static void MapDependencyInjectionPageRoute(this RouteCollection routes, string routeName, string routeUrl, string physicalFile, bool checkPhysicalUrlAccess)
		{
			MapDependencyInjectionPageRoute(routes, routeName, routeUrl, physicalFile, checkPhysicalUrlAccess, null, null, null);
		}

		/// <summary>
		/// Vytvoří novou route pomocí DependencyInjectionPageRouteHandler
		/// </summary>
		public static void MapDependencyInjectionPageRoute(this RouteCollection routes, string routeName, string routeUrl, string physicalFile, bool checkPhysicalUrlAccess, RouteValueDictionary defaults)
		{
			MapDependencyInjectionPageRoute(routes, routeName, routeUrl, physicalFile, checkPhysicalUrlAccess, defaults, null, null);
		}

		/// <summary>
		/// Vytvoří novou route pomocí DependencyInjectionPageRouteHandler
		/// </summary>
		public static void MapDependencyInjectionPageRoute(this RouteCollection routes, string routeName, string routeUrl, string physicalFile, bool checkPhysicalUrlAccess, RouteValueDictionary defaults, RouteValueDictionary constraints)
		{
			MapDependencyInjectionPageRoute(routes, routeName, routeUrl, physicalFile, checkPhysicalUrlAccess, defaults, constraints, null);
		}

		/// <summary>
		/// Vytvoří novou route pomocí DependencyInjectionPageRouteHandler
		/// </summary>
		public static void MapDependencyInjectionPageRoute(this RouteCollection routes, string routeName, string routeUrl, string physicalFile, bool checkPhysicalUrlAccess, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens)
		{
			routes.Add(routeName, new Route(routeUrl, defaults, constraints, dataTokens, new DependencyInjectionPageRouteHandler(physicalFile, checkPhysicalUrlAccess)));
		}
	}
}