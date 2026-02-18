using Hangfire;
using Hangfire.Dashboard;
using Havit.Diagnostics.Contracts;
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
		Contract.Assert<ArgumentNullException>(configuration != null);

		var tagJobsFilter = new TagJobFilter();

		if (jobsTaggingOptions?.TagFunc != null)
		{
			tagJobsFilter.TagFunc = jobsTaggingOptions.TagFunc;
		}

		configuration.UseFilter(tagJobsFilter);

		return configuration;
	}

	/// <summary>
	/// Configures and integrates Hangfire Tags functionality with SQL storage.
	/// </summary>
	/// <exception cref="ArgumentNullException">Thrown when the <paramref name="configuration"/> is null.</exception>
	public static IGlobalConfiguration UseTagsDashboardExtension(this IGlobalConfiguration configuration)
	{
		Contract.Assert<ArgumentNullException>(configuration != null);

		DashboardRoutes.Routes.AddRecurringJobsTags();

		return configuration;
	}
}