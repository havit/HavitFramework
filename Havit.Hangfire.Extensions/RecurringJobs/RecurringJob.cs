using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Hangfire.Extensions.RecurringJobs;

/// <summary>
/// Recurring job to schedule.
/// </summary>
public class RecurringJob<TJob> : IRecurringJob
{
	/// <summary>
	/// Job identifier. Takes TJob class name (for interfaces it trims starting I).
	/// </summary>
	public string JobId
	{
		get
		{
			return (typeof(TJob).IsInterface && typeof(TJob).Name.StartsWith("I"))
				? typeof(TJob).Name.Substring(1)
				: typeof(TJob).Name;
		}
	}

	/// <summary>
	/// Returns the jobs.
	/// </summary>
	public Expression<Func<TJob, Task>> MethodCall { get; }

	/// <summary>
	/// Cron expression.
	/// </summary>
	public string CronExpression { get; }

	/// <summary>
	/// Time zone info.
	/// </summary>
	public TimeZoneInfo TimeZone { get; }

	/// <summary>
	/// Queue name.
	/// </summary>
	public string Queue { get;  }

	/// <summary>
	/// Constructor.
	/// </summary>
	public RecurringJob(Expression<Func<TJob, Task>> methodCall, string cronExpression, TimeZoneInfo timeZone, string queue = "default")
	{
		MethodCall = methodCall;
		CronExpression = cronExpression ?? Cron.Never();
		TimeZone = timeZone;
		Queue = queue;
	}

	/// <inheritdoc />
	public void ScheduleAsRecurringJob(IRecurringJobManager recurringJobManager)
	{
		recurringJobManager.AddOrUpdate<TJob>(JobId, MethodCall, CronExpression, TimeZone, Queue);
	}

	/// <inheritdoc />
	public async Task RunAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
	{
		var job = serviceProvider.GetRequiredService<TJob>();
		SetCancellationTokenVisitor setCancellationTokenVisitor = new SetCancellationTokenVisitor(cancellationToken);
		var methodCall = (Expression<Func<TJob, Task>>)setCancellationTokenVisitor.Visit(MethodCall);
		await methodCall.Compile().Invoke(job);
	}
}
