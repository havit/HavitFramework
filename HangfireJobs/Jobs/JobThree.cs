namespace Havit.HangfireJobs.Jobs;

public class JobThree : IJobThree
{
	public Task ExecuteAsync(CancellationToken cancellationToken)
	{
		Console.WriteLine("Job three");
		throw new ApplicationException("Job three failed.");
	}
}