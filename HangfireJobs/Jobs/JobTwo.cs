namespace Havit.HangfireJobs.Jobs;

public class JobTwo : IJobTwo
{
	public Task ExecuteAsync(CancellationToken cancellationToken)
	{
		Console.WriteLine("Job two");
		return Task.CompletedTask;
	}
}