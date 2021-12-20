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
			var backgroundJobHelperService = new BackgroundJobHelperService(new BackgroundJobClient(), JobStorage.Current);
			backgroundJobHelperService.DeleteEnqueuedJobs(queue);
		}

    }
}
