using Havit.Data.Configuration.Git.Core;
using Microsoft.Extensions.Configuration;

namespace Havit.Extensions.Configuration.ConnectionStrings.Git;

public static class ConfigurationBuilderExtensions
{
	public static IConfigurationRoot BuildGitBranchConfiguration(this IConfigurationBuilder configurationBuilder)
	{
		return new BranchConnectionStringConfigurationRoot(configurationBuilder.Build(), configurationBuilder.GetFileProvider(), new HeadFileGitRepositoryProvider());
	}
}