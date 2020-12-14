using Hangfire;
using Hangfire.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Hangfire.Extensions.RecurringJobs
{
	/// <summary>
	/// Contains methods to schedule recurring jobs.
	/// </summary>
	public static class RecurringJobsHelper
	{
		/// <summary>
		/// Schedules the recurring jobs and clears any additional already scheudled recurring jobs.
		/// </summary>
		public static void SetSchedule(params IRecurringJob[] recurringJobsToSchedule)
		{
			// schedule recurring jobs
			foreach (IRecurringJob job in recurringJobsToSchedule)
			{
				job.Schedule();
			}

			// Clear previous plan
			using (var connection = JobStorage.Current.GetConnection())
			{
				string[] jobsToRemove = connection.GetRecurringJobs().Select(item => item.Id).Except(recurringJobsToSchedule.Select(item => item.JobId)).ToArray();
				foreach (string jobId in jobsToRemove)
				{
					RecurringJob.RemoveIfExists(jobId);
				}
			}
		}
	}
}
