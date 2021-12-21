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

namespace Havit.Hangfire.Extensions.RecurringJobs
{
	/// <summary>
	/// Methods to help with background jobs.
	/// </summary>
	internal class RecurringJobsSchedulerOnApplicationStartup : IHostedService
	{
        private readonly IRecurringJobsHelperService recurringJobsHelperService;
        private readonly RecurringJobsSchedulerOnApplicationStartupOptions options;

        /// <summary>
        /// Constructor.
        /// </summary>
        public RecurringJobsSchedulerOnApplicationStartup(IRecurringJobsHelperService recurringJobsHelperService, IOptions<RecurringJobsSchedulerOnApplicationStartupOptions> options)
        {
            this.recurringJobsHelperService = recurringJobsHelperService;
            this.options = options.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            recurringJobsHelperService.SetSchedule(options.RecurringJobs.ToArray());
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
