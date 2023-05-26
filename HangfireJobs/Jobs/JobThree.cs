using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Havit.HangfireJobs.Jobs
{
    public class JobThree : IJobThree
    {
        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
			Console.WriteLine("Job three");
			return Task.CompletedTask;
		}
    }
}