using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Hangfire.Tags;
using Hangfire.Tags.SqlServer;

namespace Havit.Hangfire.Extensions.Tags;

// TODO Hangfire: Zapojit do uk�zkov� aplikace

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

		// TODO Hangfire: Zvalidovat, jestli chceme tuto podobu zadr�tovat napevno. Sp�e ne, ned�me tak mo�nost pou��t vlastn� n�zev v atributu (co� tam nyn� nen�).

		// Register the Hangfire Tag method attribute globally
		GlobalJobFilters.Filters.Add(new Filters.HangfireTagMethodAttribute());

		TagsOptions usedOptions = tagsOptions ?? new TagsOptions()
		{
			//TODO Hangfire: Nen� pro n�s vhodn�j�� default None? Pokud to tu (s ohledem na n�sleduj�c� koment��) z�stane.
			Clean = Clean.Punctuation
		};

		// TODO Hangfire: Pot�ebujeme dostat pry�, m�me projekty i bez SQL Serveru, nechceme zadr�tovat pot�ebu SQL Serveru
		configuration.UseTagsWithSql(usedOptions, storageOptions);
	}
}