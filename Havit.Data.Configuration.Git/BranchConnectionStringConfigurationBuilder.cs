using Havit.Data.Configuration.Git.Core;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace Havit.Data.Configuration.Git;

/// <summary>
/// BranchConfigurationBuilderBase implementation that modifies connection strings according to current Git branch (i.e. changes DB name).
/// </summary>
public class BranchConnectionStringConfigurationBuilder : BranchConfigurationBuilderBase
{
	/// <summary>
	/// Creates instance of <see cref="BranchConnectionStringConfigurationBuilder"/>.
	/// </summary>
	public BranchConnectionStringConfigurationBuilder()
		: this(new HeadFileGitRepositoryProvider(), new BranchConfigValueTransformer())
	{
	}

	/// <summary>
	/// Creates instance of <see cref="BranchXmlConfigurationBuilder"/> with custom <see cref="IGitRepositoryProvider"/> and/or <paramref name="branchConfigSectionValueTransformer"/> (usually for testing purposes).
	/// </summary>
	public BranchConnectionStringConfigurationBuilder(IGitRepositoryProvider gitRepositoryProvider, IBranchConfigValueTransformer branchConfigSectionValueTransformer)
		: base(gitRepositoryProvider, branchConfigSectionValueTransformer)
	{
	}

	/// <inheritdoc />
	public override ConfigurationSection ProcessConfigurationSection(ConfigurationSection configSection)
	{
		if (configSection is ConnectionStringsSection connectionStringsSection)
		{
			return ProcessConnectionStringsSection(connectionStringsSection);
		}
		else
		{
			throw new ConfigurationErrorsException("BranchConnectionStringConfigurationBuilder can be set only on ConnectionStrings section");
		}
	}

	private ConfigurationSection ProcessConnectionStringsSection(ConnectionStringsSection section)
	{
		var transformedSection = new ConnectionStringsSection();

		foreach (ConnectionStringSettings connString in section.ConnectionStrings)
		{
			var newConnString = TransformConnectionString(connString.ConnectionString);
			transformedSection.ConnectionStrings.Add(new ConnectionStringSettings(connString.Name, newConnString));
		}

		return transformedSection;
	}

	internal string TransformConnectionString(string connectionString)
	{
		return BranchConfigSectionValueTransformer.TransformConfigValue(connectionString, BranchName);
	}
}