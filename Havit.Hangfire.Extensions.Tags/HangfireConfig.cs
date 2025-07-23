using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Hangfire.Tags;
using Hangfire.Tags.SqlServer;

namespace Havit.Hangfire.Extensions.Tags;

/// <summary>
/// Provides configuration methods for integrating and using Hangfire Tags functionality with SQL storage.
/// </summary>
public static class HangfireConfig
{
	/// <summary>
	/// Configures and integrates Hangfire Tags functionality with SQL storage.
	/// </summary>
	/// <param name="configuration">The global Hangfire configuration to which the Tags functionality will be added.</param>
	/// <param name="storageOptions">Regired storage options</param>
	/// <param name="tagsOptions">Style of tag rendering</param>
	/// <exception cref="ArgumentNullException">Thrown when the <paramref name="configuration"/> is null.</exception>
	public static void UseTags(this IGlobalConfiguration configuration, SqlServerStorageOptions storageOptions, TagsOptions tagsOptions = null)
	{
		ArgumentNullException.ThrowIfNull(configuration);

		DashboardRoutes.Routes.AddRecurringJobsTags();

		// Register the Hangfire Tag method attribute globally
		GlobalJobFilters.Filters.Add(new Filters.HangfireTagMethodAttribute());

		TagsOptions usedOptions = tagsOptions ?? new TagsOptions()
		{
			Clean = Clean.Punctuation
		};

		configuration.UseTagsWithSql(usedOptions, storageOptions);
	}
}