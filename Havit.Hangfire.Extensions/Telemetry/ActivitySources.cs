using System.Diagnostics;

namespace Havit.Hangfire.Extensions.Telemetry;

/// <summary>
/// Obsahuje definice ActivitySource pro telemetrii Havit.Hangfire.Extensions.
/// </summary>
public class ActivitySources
{
	/// <summary>
	/// Název aktivity source v Havit.Hangfire.Extensions.
	/// </summary>
	public const string ActivitySourceName = "Havit.Hangfire.Extensions";

	internal static readonly ActivitySource HavitHangfireActivitySource = new ActivitySource(ActivitySourceName);
}
