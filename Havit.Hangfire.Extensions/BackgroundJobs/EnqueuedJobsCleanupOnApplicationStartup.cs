using Hangfire;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
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
        private readonly EnqueuedJobsCleanupOnApplicationStartupOptions options;
        private readonly IBackgroundJobHelperService backgroundJobHelperService;

        /// <summary>
        /// Constructor.
        /// </summary>
        public EnqueuedJobsCleanupOnApplicationStartup(IBackgroundJobHelperService backgroundJobHelperService, IOptions<EnqueuedJobsCleanupOnApplicationStartupOptions> options)
        {
            this.backgroundJobHelperService = backgroundJobHelperService;
            this.options = options.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var queue in options.Queues)
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
