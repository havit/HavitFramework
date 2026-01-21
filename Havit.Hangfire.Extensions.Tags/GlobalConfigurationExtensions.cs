using Hangfire;
using Hangfire.Dashboard;
using Havit.Hangfire.Extensions.Tags.Dashboard;
using Havit.Hangfire.Extensions.Tags.Filters;

namespace Havit.Hangfire.Extensions.Tags;

/// <summary>
/// Provides configuration methods for integrating and using Hangfire Tags functionality with SQL storage.
/// </summary>
public static class GlobalConfigurationExtensions
{
	/// <summary>
	/// Configures job tagging functionality for Hangfire.
	/// </summary>
	public static IGlobalConfiguration UseJobsTagging(this IGlobalConfiguration configuration, JobsTaggingOptions jobsTaggingOptions = null)
	{
		ArgumentNullException.ThrowIfNull(configuration);

		var hangfireTagJobsAttribute = new TagJobAttribute();

		if (jobsTaggingOptions?.TagFunc != null)
		{
			hangfireTagJobsAttribute.TagFunc = jobsTaggingOptions.TagFunc;
		}

		configuration.UseFilter(hangfireTagJobsAttribute);

		return configuration;
	}

	/// <summary>
	/// Configures and integrates Hangfire Tags functionality with SQL storage.
	/// </summary>
	/// <exception cref="ArgumentNullException">Thrown when the <paramref name="configuration"/> is null.</exception>
	public static IGlobalConfiguration UseTagsDashboardExtension(this IGlobalConfiguration configuration)
	{
		ArgumentNullException.ThrowIfNull(configuration);

		DashboardRoutes.Routes.AddRecurringJobsTags();

		return configuration;
	}
}