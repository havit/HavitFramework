using Microsoft.Extensions.Configuration;

namespace Havit.Business.Configuration.NetCore
{
	public static class ConfigurationBuilderExtensions
	{
		public static IConfigurationRoot BuildGitBranchConfiguration(this IConfigurationBuilder configurationBuilder)
		{
			return new BranchConnectionStringConfigurationRoot(configurationBuilder.Build(), configurationBuilder.GetFileProvider(), new WebGitRepositoryProvider());
		}
	}
}