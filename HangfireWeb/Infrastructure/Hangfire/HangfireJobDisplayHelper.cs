using Hangfire.Common;
using Hangfire.Dashboard;
using Havit.HangfireJobs.Jobs;

namespace Havit.HangfireApp.Infrastructure.Hangfire
{
    internal static class HangfireJobDisplayHelper
    {
        public static string GetJobDisplayName(DashboardContext dashboardContext, Job job)
        {
            if ((job.Type != null) && job.Type.IsInterface && (job.Type.Name.StartsWith("I", System.StringComparison.Ordinal)))
            {
                return job.Type.Name.Substring(1);
            }
            return job.ToString();                
        }
    }
}
