namespace Havit.HangfireJobs.Jobs;

public class JobOne : IJobOne
{
	public async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		Console.WriteLine("Job one");

		// To check correlations (OpenTelemetry, Application Insights)
		_ = await new HttpClient().GetAsync("https://www.havit.cz", cancellationToken);

	}
}