using Hangfire;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Hangfire.Extensions.BackgroundJobs
{
	/// <summary>
	/// Methods to help with background jobs.
	/// </summary>
	public class BackgroundJobHelper
	{
		/// <summary>
		/// Deletes all enqueued jobs in a queue.
		/// </summary>
		public static void DeleteEnqueuedJobs(string queue = "default")
		{
			// remove enqueued jobs
			List<string> toDelete = new List<string>();
			IMonitoringApi monitor = JobStorage.Current.GetMonitoringApi();
			if (monitor.Queues().Any())
			{
				QueueWithTopEnqueuedJobsDto queueWithTopEnqueuedJobs = monitor.Queues().Single(q => q.Name == queue); // get the single queue by name

				Enumerable.Range(0, (int)Math.Ceiling(queue.Length / 1000d)) // lets batch items by 1000
					.SelectMany(batchIndex => monitor.EnqueuedJobs(queueWithTopEnqueuedJobs.Name, 1000 * batchIndex, 1000)) // select jobs in a batch by batchIndex
					.Select(jobEntry => jobEntry.Key) // select JobId (key is a JobId)
					.ToList() // process all pages to memory
					.ForEach(jobId => BackgroundJob.Delete(jobId)); // remove the job
			}
		}
	}
}
