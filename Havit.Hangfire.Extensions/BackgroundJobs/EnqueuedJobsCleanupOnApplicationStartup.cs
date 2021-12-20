using Hangfire;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Havit.Hangfire.Extensions.BackgroundJobs
{
	/// <summary>
	/// Methods to help with background jobs.
	/// </summary>
	internal class EnqueuedJobsCleanupOnApplicationStartup : IHostedService
	{
        private readonly string[] queues;
        private readonly IBackgroundJobHelperService backgroundJobHelperService;

        /// <summary>
        /// Constructor.
        /// </summary>
        public EnqueuedJobsCleanupOnApplicationStartup(IBackgroundJobHelperService backgroundJobHelperService, string[] queues)
        {
            this.backgroundJobHelperService = backgroundJobHelperService;
            this.queues = queues;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var queue in queues)
            {
                backgroundJobHelperService.DeleteEnqueuedJobs(queue);
            }
            return Task.CompletedTask;
        }

        public void StopApplication()
        {  
            // NOOP
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // NOOP
            return Task.CompletedTask;
        }
    }
}
