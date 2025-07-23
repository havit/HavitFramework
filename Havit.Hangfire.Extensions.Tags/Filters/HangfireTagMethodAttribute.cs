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
		// TODO Hangfire: Odpovídá tato logika i Havit.Hangfire.Extensions? Použít, neduplikovat. Resp. ona použitá je tím, že jobName nastavujeme pouze tehdy, nemáme recurringjobid.
		// TODO Hangfire: Umožnit použít custom název tagu tak, aby bylo možné atribut použít nejen globálnì?
		// TODO Hangfire: Nemìli bychom AddTags udìlat už døíve, než dojde n OnPerforming? (protože taky na nìj dojít nemusí, ale je to souèástí historie jobù)
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