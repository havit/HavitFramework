using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Havit.HangfireJobs.Jobs
{
	/// <summary>
	/// Základní bezparametrický job, který lze spouštět z konzolovky a hangfire.
	/// </summary>
	public interface IRunnableJob
	{
		Task ExecuteAsync(CancellationToken cancellationToken);
	}
}
