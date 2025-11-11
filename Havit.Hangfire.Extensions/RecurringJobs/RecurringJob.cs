using System.ComponentModel;
using System.Linq.Expressions;
using Hangfire;
using Hangfire.States;
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
			return _jobId ?? ((typeof(TJob).IsInterface && typeof(TJob).Name.StartsWith("I"))
				? typeof(TJob).Name.Substring(1)
				: typeof(TJob).Name);
		}
		set
		{
			_jobId = value;
		}
	}
	private string _jobId;

	/// <summary>
	/// Queue name.
	/// </summary>
	public string Queue { get; }

	/// <summary>
	/// Returns the jobs.
	/// </summary>
	public Expression<Func<TJob, Task>> MethodCall { get; }

	/// <summary>
	/// Cron expression.
	/// </summary>
	public string CronExpression { get; }

	/// <summary>
	/// Recurring jobs options.
	/// </summary>
	public RecurringJobOptions RecurringJobOptions { get; }

	/// <summary>
	/// Constructor (for backward compatibility).
	/// </summary>
	public RecurringJob(Expression<Func<TJob, Task>> methodCall, string cronExpression, TimeZoneInfo timeZone, string queue = EnqueuedState.DefaultQueue, MisfireHandlingMode misfireHandling = MisfireHandlingMode.Relaxed)
		: this(queue, methodCall, cronExpression, new RecurringJobOptions { TimeZone = timeZone ?? TimeZoneInfo.Utc, MisfireHandling = misfireHandling })
	{
		// NOOP
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public RecurringJob(string queue, Expression<Func<TJob, Task>> methodCall, string cronExpression, RecurringJobOptions recurringJobOptions)
	{
		Queue = queue;
		MethodCall = methodCall;
		CronExpression = cronExpression ?? Cron.Never();
		RecurringJobOptions = recurringJobOptions;
	}

	/// <inheritdoc />
	public void ScheduleAsRecurringJob(IRecurringJobManager recurringJobManager)
	{
		recurringJobManager.AddOrUpdate<TJob>(JobId, Queue, MethodCall, CronExpression, RecurringJobOptions);
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
