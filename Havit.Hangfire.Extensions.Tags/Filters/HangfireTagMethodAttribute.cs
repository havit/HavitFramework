using Hangfire.Common;
using Hangfire.Server;
using Hangfire.Tags;

namespace Havit.Hangfire.Extensions.Tags.Filters;

// TODO Hangfire: Naming - tagujeme job, nikoliv metodu.

/// <summary>
/// Adds Tag to a Hangfire job based on the method name.
/// For recurring jobs, it uses the "RecurringJobId" parameter if available;
/// otherwise, it derives the job name from the method name by removing the "Async" suffix if present.
/// </summary>
internal class HangfireTagMethodAttribute : JobFilterAttribute, IServerFilter
{
	/// <inheritdoc />
	public void OnPerforming(PerformingContext filterContext)
	{
		// TODO Hangfire: Odpov�d� tato logika i Havit.Hangfire.Extensions? Pou��t, neduplikovat. Resp. ona pou�it� je t�m, �e jobName nastavujeme pouze tehdy, nem�me recurringjobid.
		// TODO Hangfire: Umo�nit pou��t custom n�zev tagu tak, aby bylo mo�n� atribut pou��t nejen glob�ln�?
		// TODO Hangfire: Nem�li bychom AddTags ud�lat u� d��ve, ne� dojde n OnPerforming? (proto�e taky na n�j doj�t nemus�, ale je to sou��st� historie job�)
		if (!filterContext.BackgroundJob.ParametersSnapshot.TryGetValue("RecurringJobId", out string jobName))
		{
			jobName = RemoveAsyncSuffix(filterContext.BackgroundJob.Job.Method.Name);
		}

		if (!string.IsNullOrEmpty(jobName))
		{
			filterContext.AddTags(jobName);
		}
	}

	/// <inheritdoc />
	public void OnPerformed(PerformedContext filterContext)
	{
		// NOOP
	}

	private static string RemoveAsyncSuffix(string methodName)
	{
		const string suffix = "Async";
		if (methodName.EndsWith(suffix, StringComparison.Ordinal))
		{
			return methodName.Substring(0, methodName.Length - suffix.Length);
		}
		return methodName;
	}
}