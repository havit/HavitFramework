namespace Havit.HangfireJobs.Jobs;

public class JobOne : IJobOne
{
	public Task ExecuteAsync(CancellationToken cancellationToken)
	{
		Console.WriteLine("Job one");
		return Task.CompletedTask;

	}
}