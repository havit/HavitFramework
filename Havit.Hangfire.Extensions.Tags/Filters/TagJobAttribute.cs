using Hangfire;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.Tags;
using Havit.Hangfire.Extensions.Helpers;
using Havit.Hangfire.Extensions.RecurringJobs.Services;

namespace Havit.Hangfire.Extensions.Tags.Filters;

/// <summary>
/// Adds Tag to a Hangfire job based on the method name.
/// For recurring jobs, it uses the "RecurringJobId" parameter if available;
/// otherwise, it derives the job name from the method name by configured function or JobNameHelper.TryGetSimpleName.
/// </summary>
internal class TagJobAttribute : JobFilterAttribute, IServerFilter
{
	/// <summary>
	/// Customizes tag for job.
	/// Only non-recurring jobs are affected by this function.
	/// </summary>
	public Func<BackgroundJob, string> TagFunc { get; set; }

	/// <inheritdoc />
	public void OnPerforming(PerformingContext performingContext)
	{
		string tag;

		// at first we need recurring job id for recurring job (while we are creating a html link with recurring job id)
		var recurringJobId = performingContext.GetJobParameter<string>("RecurringJobId");
		if (!String.IsNullOrEmpty(recurringJobId))
		{
			tag = recurringJobId;
		}
		// if there is a custom function to get the job name, use it
		else if (TagFunc != null)
		{
			tag = TagFunc(performingContext.BackgroundJob);
		}
		// if it is SequenceRecurringJobScheduler (but nonrecurring job), skip it as infrastructural job
		else if (performingContext.BackgroundJob.Job.Type == typeof(SequenceRecurringJobScheduler))
		{
			tag = null;
		}
		// otherwise, try to get the job name from RecurringJobId
		else
		{
			tag = JobNameHelper.TryGetSimpleName(performingContext.BackgroundJob.Job, out string simpleJobName)
					? simpleJobName
					: performingContext.BackgroundJob.Job.ToString();
		}

		if (!string.IsNullOrEmpty(tag))
		{
			// AddTags can be used only on PerformContext so we cannot register tag earlier.
			performingContext.AddTags(tag);
		}
	}

	/// <inheritdoc />
	public void OnPerformed(PerformedContext filterContext)
	{
		// NOOP
	}
}