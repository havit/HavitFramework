namespace Havit.HangfireJobs.Jobs;

/// <summary>
/// Základní bezparametrický job, který lze spouštět z konzolovky a hangfire.
/// </summary>
public interface IRunnableJob
{
	Task ExecuteAsync(CancellationToken cancellationToken);
}
