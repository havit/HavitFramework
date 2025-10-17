using System.Reflection;
using Hangfire.Dashboard;

namespace Havit.Hangfire.Extensions.Tags;


/// <summary>
/// Provides extension methods for <see cref="RouteCollection"/>.
/// </summary>
internal static class RouteCollectionExtensions
{
	// ReSharper disable once InconsistentNaming
	private static readonly FieldInfo s_dispatchers = typeof(RouteCollection).GetTypeInfo().GetDeclaredField("_dispatchers");

	/// <summary>
	/// Returns a private list of registered routes.
	/// </summary>
	/// <param name="routes">Route collection</param>
	private static List<Tuple<string, IDashboardDispatcher>> GetDispatchers(this RouteCollection routes)
	{
		ArgumentNullException.ThrowIfNull(routes);

		if (s_dispatchers?.GetValue(routes) is null)
		{
			return [];
		}

		return (List<Tuple<string, IDashboardDispatcher>>)s_dispatchers.GetValue(routes)!;
	}

	/// <summary>
	/// Combines exising dispatcher for <paramref name="pathTemplate"/> with <paramref name="dispatcher"/>.
	/// If there's no dispatcher for the specified path, adds a new one.
	/// </summary>
	/// <param name="routes">Route collection</param>
	/// <param name="pathTemplate">Path template</param>
	/// <param name="dispatcher">Dispatcher to add or append for specified path</param>
	internal static void Append(this RouteCollection routes, string pathTemplate, IDashboardDispatcher dispatcher)
	{
		ArgumentNullException.ThrowIfNull(routes);
		ArgumentNullException.ThrowIfNull(pathTemplate);
		ArgumentNullException.ThrowIfNull(dispatcher);

		var list = routes.GetDispatchers();

		for (var i = 0; i < list.Count; i++)
		{
			var pair = list[i];
			if (pair.Item1 == pathTemplate)
			{
				if (pair.Item2 is not CompositeDispatcher composite)
				{
					// replace original dispatcher with a composite one
					composite = new CompositeDispatcher(pair.Item2);
					list[i] = new Tuple<string, IDashboardDispatcher>(pair.Item1, composite);
				}

				composite.AddDispatcher(dispatcher);
				return;
			}
		}

		routes.Add(pathTemplate, dispatcher);
	}

	public static void AddRecurringJobsTags(this RouteCollection routes)
	{
		var assembly = typeof(RouteCollectionExtensions).Assembly;
		const string jsContentType = "text/javascript";
		var jsDispatcher = new EmbeddedResourceDispatcher(jsContentType, assembly, "Havit.Hangfire.Extensions.Tags.CustomizeReccuringJobsList.js");
		routes.Append("/js[0-9]+", jsDispatcher);

		const string cssContentType = "text/css";
		var cssDispatcher = new EmbeddedResourceDispatcher(cssContentType, assembly, "Havit.Hangfire.Extensions.Tags.CustomizeTagList.css");
		routes.Append("/css[0-9]+", cssDispatcher);
	}
}