using Hangfire;
using Hangfire.Common;
using Hangfire.Server;

namespace Havit.Hangfire.Extensions.Filters;

/// <summary>
/// Legacy.
/// </summary>
[Obsolete("Use OpenTelemetryAttribute instead.", error: true)]
public class ApplicationInsightAttribute : JobFilterAttribute, IServerFilter
{
	/// <summary>
	/// Gets the custom name of the job.
	/// </summary>
	public Func<BackgroundJob, string> JobNameFunc { get; set; }

	/// <inheritdoc />
	public void OnPerforming(PerformingContext context)
	{
		throw new NotSupportedException();
	}

	/// <inheritdoc />
	public void OnPerformed(PerformedContext context)
	{
		throw new NotSupportedException();
	}

}
