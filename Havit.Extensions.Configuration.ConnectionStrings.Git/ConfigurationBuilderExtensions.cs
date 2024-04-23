using Havit.Data.Configuration.Git.Core;
using Microsoft.Extensions.Configuration;

namespace Havit.Extensions.Configuration.ConnectionStrings.Git;

public static class ConfigurationBuilderExtensions
{
	/// <summary>
	/// Applies branchName to config section(s)
	/// </summary>
	/// <param name="configurationBuilder">configurationBuilder</param>
	/// <param name="configSectionNamesToApply">Config section names such as ConnectionStrings (default), AppSettings, ApplicationInsights or any other root level setting</param>
	public static IConfigurationRoot BuildGitBranchConfiguration(this IConfigurationBuilder configurationBuilder, params string[] configSectionNamesToApply)
	{
		return new BranchConfigurationRoot(configurationBuilder.Build(), configurationBuilder.GetFileProvider(), new HeadFileGitRepositoryProvider(), configSectionNamesToApply);
	}
}