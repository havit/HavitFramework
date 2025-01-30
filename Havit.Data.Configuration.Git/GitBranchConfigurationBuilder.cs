using Havit.Data.Configuration.Git.Core;
using System.Configuration;
using System.Xml;

namespace Havit.Data.Configuration.Git;

/// <summary>
/// Configuration Builder implementation that modifies xml config sections according to current Git branch. Transforms entire xml nodes (key, values, attributes,..)
/// </summary>
public class GitBranchConfigurationBuilder : ConfigurationBuilder
{
	private readonly BranchConfigValueTransformer branchConfigSectionValueTransformer;
	private readonly string branchName;

	/// <summary>
	/// Creates instance of <see cref="GitBranchConfigurationBuilder"/> with HeadFileGitRepositoryProvider and BranchConfigSectionValueTransformer
	/// </summary>
	public GitBranchConfigurationBuilder()
		: this(new HeadFileGitRepositoryProvider(), new BranchConfigValueTransformer())
	{
	}

	/// <summary>
	/// Creates instance of <see cref="GitBranchConfigurationBuilder"/> with custom <see cref="IGitRepositoryProvider"/> and fixed <paramref name="branchConfigSectionValueTransformer"/> (usually for testing purposes).
	/// </summary>
	public GitBranchConfigurationBuilder(IGitRepositoryProvider gitRepositoryProvider, BranchConfigValueTransformer branchConfigSectionValueTransformer)
	{
		this.branchConfigSectionValueTransformer = branchConfigSectionValueTransformer;
		string configPath = AppDomain.CurrentDomain.BaseDirectory;
		branchName = gitRepositoryProvider.GetBranch(configPath);
	}

	/// <inheritdoc />
	public override XmlNode ProcessRawXml(XmlNode rawXml)
	{
		string rawXmlString = rawXml.OuterXml;

		if (string.IsNullOrEmpty(rawXmlString))
		{
			return rawXml;
		}
		rawXmlString = branchConfigSectionValueTransformer.TransformConfigValue(rawXmlString, branchName);
		XmlDocument doc = new XmlDocument();
		doc.PreserveWhitespace = true;
		doc.LoadXml(rawXmlString);
		return doc.DocumentElement;
	}
}