using Havit.Data.Configuration.Git.Core;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace Havit.Data.Configuration.Git;

/// <summary>
/// BranchConfigurationBuilderBase implementation that modifies app settings values according to current Git branch
/// </summary>
public class BranchAppSettingConfigurationBuilder : BranchConfigurationBuilderBase
{
	/// <summary>
	/// Creates instance of <see cref="BranchAppSettingConfigurationBuilder"/>.
	/// </summary>
	public BranchAppSettingConfigurationBuilder()
		: this(new HeadFileGitRepositoryProvider(), new BranchConfigValueTransformer())
	{
	}

	/// <summary>
	/// Creates instance of <see cref="BranchAppSettingConfigurationBuilder"/> with custom <see cref="IGitRepositoryProvider"/> and/or <paramref name="branchConfigSectionValueTransformer"/> (usually for testing purposes).
	/// </summary>
	public BranchAppSettingConfigurationBuilder(IGitRepositoryProvider gitRepositoryProvider, IBranchConfigValueTransformer branchConfigSectionValueTransformer)
		: base(gitRepositoryProvider, branchConfigSectionValueTransformer)
	{
	}

	/// <inheritdoc />
	public override ConfigurationSection ProcessConfigurationSection(ConfigurationSection configSection)
	{
		if (configSection is AppSettingsSection appSettingSection)
		{
			return ProcessAppSettingSection(appSettingSection);
		}
		else
		{
			throw new ConfigurationErrorsException("BranchAppSettingConfigurationBuilder can be set only on AppSettings section");
		}
	}

	private ConfigurationSection ProcessAppSettingSection(AppSettingsSection appSettingSection)
	{
		var transformedSection = new AppSettingsSection();

		foreach (KeyValueConfigurationElement settingKeyValue in appSettingSection.Settings)
		{
			var newAppSettingValue = TransformAppSetting(settingKeyValue.Value);
			transformedSection.Settings.Add(new KeyValueConfigurationElement(settingKeyValue.Key, newAppSettingValue));
		}

		return transformedSection;
	}

	internal string TransformAppSetting(string appSettingValue)
	{
		return BranchConfigSectionValueTransformer.TransformConfigValue(appSettingValue, BranchName);
	}
}