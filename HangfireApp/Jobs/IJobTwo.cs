using Havit.Hangfire.Extensions.Filters;
using Havit.Hangfire.Extensions.RecurringJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.HangfireApp.Jobs
{
    [DisableConcurrentExecutionInJobGroup("MainGroup")]
    public interface IJobTwo : IRunnableJob
    {
    }
}
