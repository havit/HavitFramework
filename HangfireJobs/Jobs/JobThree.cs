namespace Havit.HangfireJobs.Jobs;

public class JobThree : IJobThree
{
	public Task ExecuteAsync(CancellationToken cancellationToken)
	{
		Console.WriteLine("Job three");
		return Task.CompletedTask;
	}
}