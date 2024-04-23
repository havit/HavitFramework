using Havit.Data.Configuration.Git.Core;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace Havit.Data.Configuration.Git;

/// <summary>
/// Configuration Builder implementation that modifies xml config sections according to current Git branch. Transforms entire xml nodes (key, values, attributes,..)
/// </summary>
public class BranchXmlConfigurationBuilder : BranchConfigurationBuilderBase
{
	/// <summary>
	/// Creates instance of <see cref="BranchConfigurationBuilderBase"/> with HeadFileGitRepositoryProvider and BranchConfigSectionValueTransformer
	/// </summary>
	public BranchXmlConfigurationBuilder()
		: this(new HeadFileGitRepositoryProvider(), new BranchConfigValueTransformer())
	{
	}

	/// <summary>
	/// Creates instance of <see cref="BranchConfigurationBuilderBase"/> with custom <see cref="IGitRepositoryProvider"/> and/or <paramref name="branchConfigSectionValueTransformer"/> (usually for testing purposes).
	/// </summary>
	public BranchXmlConfigurationBuilder(IGitRepositoryProvider gitRepositoryProvider, IBranchConfigValueTransformer branchConfigSectionValueTransformer)
		: base(gitRepositoryProvider, branchConfigSectionValueTransformer)
	{
	}

	/// <inheritdoc />
	public override XmlNode ProcessRawXml(XmlNode rawXml)
	{
		string rawXmlString = rawXml.OuterXml;

		if (string.IsNullOrEmpty(rawXmlString))
		{
			return rawXml;
		}
		rawXmlString = BranchConfigSectionValueTransformer.TransformConfigValue(rawXmlString, BranchName);
		XmlDocument doc = new XmlDocument();
		doc.PreserveWhitespace = true;
		doc.LoadXml(rawXmlString);
		return doc.DocumentElement;
	}
}