using Hangfire;

namespace Havit.Hangfire.Extensions.Tags;

/// <summary>
/// Options for configuring job tagging behavior.
/// </summary>
public class JobsTaggingOptions
{
	/// <summary>
	/// Customizes tag for job.
	/// Only non-recurring jobs are affected by this function.
	/// </summary>
	public Func<BackgroundJob, string> TagFunc { get; set; }
}
