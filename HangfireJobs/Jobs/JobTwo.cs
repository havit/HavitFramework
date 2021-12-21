using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Havit.HangfireJobs.Jobs
{
    public class JobTwo : IJobTwo
    {
        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("Úloha se nezdařila :-).");            
            //for (int i = 0; i < 5; i++)
            //{
            //    Console.WriteLine(this.GetType().Name + ": " + i);
            //    await Task.Delay(1000, cancellationToken);
            //}
        }
    }
}