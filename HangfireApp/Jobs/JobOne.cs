using Havit.Hangfire.Extensions.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Havit.HangfireApp.Jobs
{
    public class JobOne : IJobOne
    {
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine(this.GetType().Name + ": " + i);
                await Task.Delay(1000);
            }
        }
    }
}
