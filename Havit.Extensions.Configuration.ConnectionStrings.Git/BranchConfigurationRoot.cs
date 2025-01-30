using Havit.Data.Configuration.Git.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Havit.Extensions.Configuration.ConnectionStrings.Git;

public class BranchConfigurationRoot : IConfigurationRoot
{
	private readonly IConfigurationRoot configurationRoot;
	private readonly IFileProvider fileProvider;
	private readonly IGitRepositoryProvider gitRepositoryProvider;

	public BranchConfigurationRoot(IConfigurationRoot configurationRoot,
		IFileProvider fileProvider,
		IGitRepositoryProvider gitRepositoryProvider)
	{
		this.configurationRoot = configurationRoot;
		this.fileProvider = fileProvider;
		this.gitRepositoryProvider = gitRepositoryProvider;
	}

	public IConfigurationSection GetSection(string key)
	{
		return new ExtensibleConfigurationSection(this, key);
	}

	public IEnumerable<IConfigurationSection> GetChildren()
	{
		return configurationRoot.GetChildren();
	}

	public IChangeToken GetReloadToken()
	{
		return configurationRoot.GetReloadToken();
	}

	public string this[string key]
	{
		get
		{
			string configValue = configurationRoot[key];
			if (configValue == null)
			{
				throw new ArgumentException(
					$"Specified config value ('{key}') not found, cannot transform it using current Git branch." +
					" Please make sure configuration is correctly set up.", nameof(key));
			}

			return TransformConfigValue(configValue);
		}
		set => configurationRoot[key] = value;
	}

	public void Reload()
	{
		configurationRoot.Reload();
	}

	public IEnumerable<IConfigurationProvider> Providers => configurationRoot.Providers;

	private string TransformConfigValue(string configValue)
	{
		var appSettingsDirectory = GetAppSettingsDirectory();
		string branchName = gitRepositoryProvider.GetBranch(appSettingsDirectory);
		return new BranchConfigValueTransformer().TransformConfigValue(configValue, branchName);
	}

	private string GetAppSettingsDirectory()
	{
		return new FileInfo(fileProvider.GetDirectoryContents("").First().PhysicalPath).DirectoryName;
	}
}