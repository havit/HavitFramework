using Hangfire.Common;
using Havit.Hangfire.Extensions.RecurringJobs.Services;

namespace Havit.Hangfire.Extensions.Helpers;

/// <summary>
/// Helper methods to format job name.
/// </summary>
public static class JobNameHelper
{
	/// <summary>
	/// If the job type is a <see cref="SequenceRecurringJobScheduler" /> returns the name of the sequence (and the recurring job identifier which it is going to enqueue).
	/// If the job type is an interface returns the name of the type without the leading I.
	/// </summary>
	/// <example>For a job running interface IMyService returns MyService.</example>
	public static bool TryGetSimpleName(Job job, out string result)
	{
		Type type = job.Type;

		if (type == typeof(SequenceRecurringJobScheduler))
		{
			try
			{
				string sequenceName = (string)job.Args[0];
				string[] remainingRecurringJobIdsToEnqueue = (string[])job.Args[2];

				result = $"{sequenceName} (enqueueing {remainingRecurringJobIdsToEnqueue.First()})";
				return true;
			}
			catch
			{
				result = null;
				return false;
			}
		}

		if ((type != null) && type.IsInterface && (type.Name.StartsWith("I", System.StringComparison.Ordinal)))
		{
			result = type.Name.Substring(1);
			return true;
		}

		result = null;
		return false;
	}
}