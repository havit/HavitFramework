using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Havit.HangfireJobs.Jobs;

public class JobOne : IJobOne
{
	public Task ExecuteAsync(CancellationToken cancellationToken)
	{
		Console.WriteLine("Job one");
		return Task.CompletedTask;

	}
}