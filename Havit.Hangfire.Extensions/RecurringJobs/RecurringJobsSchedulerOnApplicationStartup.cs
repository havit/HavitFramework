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

namespace Havit.Hangfire.Extensions.RecurringJobs
{
	/// <summary>
	/// Methods to help with background jobs.
	/// </summary>
	internal class RecurringJobsSchedulerOnApplicationStartup : IHostedService
	{
        private readonly IRecurringJobsHelperService recurringJobsHelperService;
        private readonly IRecurringJob[] recurringJobs;

        /// <summary>
        /// Constructor.
        /// </summary>
        public RecurringJobsSchedulerOnApplicationStartup(IRecurringJobsHelperService recurringJobsHelperService, IRecurringJob[] recurringJobs)
        {
            this.recurringJobsHelperService = recurringJobsHelperService;
            this.recurringJobs = recurringJobs;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            recurringJobsHelperService.SetSchedule(recurringJobs);
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
