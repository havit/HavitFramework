using Hangfire.Dashboard;
using Havit.Diagnostics.Contracts;

namespace Havit.Hangfire.Extensions.Tags.Dashboard;

/// <summary>
/// Dispatcher that combines output from several other dispatchers.
/// Used internally by <see cref="RouteCollectionExtensions.Append"/>.
/// </summary>
internal class CompositeDispatcher(params IDashboardDispatcher[] dispatchers) : IDashboardDispatcher
{
	private readonly List<IDashboardDispatcher> _dispatchers = [.. dispatchers];

	public void AddDispatcher(IDashboardDispatcher dispatcher)
	{
		Contract.Assert<ArgumentNullException>(dispatcher != null);

		_dispatchers.Add(dispatcher);
	}

	public async Task Dispatch(DashboardContext context)
	{
		Contract.Assert<ArgumentNullException>(context != null);

		if (_dispatchers.Count == 0)
		{
			throw new InvalidOperationException("CompositeDispatcher should contain at least one dispatcher.");
		}

		foreach (var dispatcher in _dispatchers)
		{
			await dispatcher.Dispatch(context);
		}
	}
}