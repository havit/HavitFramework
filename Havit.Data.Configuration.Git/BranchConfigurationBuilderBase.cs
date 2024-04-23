using Havit.Data.Configuration.Git.Core;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace Havit.Data.Configuration.Git;

/// <summary>
/// Configuration Builder implementation that modifies configuration according to current Git branch
/// </summary>
public abstract class BranchConfigurationBuilderBase : ConfigurationBuilder
{
	/// <inheritdoc />
	protected readonly IBranchConfigValueTransformer BranchConfigSectionValueTransformer;

	/// <inheritdoc />
	protected readonly string BranchName;

	/// <summary>
	/// Creates instance of <see cref="BranchConfigurationBuilderBase"/> with HeadFileGitRepositoryProvider and BranchConfigSectionValueTransformer
	/// </summary>
	public BranchConfigurationBuilderBase()
		: this(new HeadFileGitRepositoryProvider(), new BranchConfigValueTransformer())
	{
	}

	/// <summary>
	/// Creates instance of <see cref="BranchConfigurationBuilderBase"/> with custom <see cref="IGitRepositoryProvider"/> and/or <paramref name="branchConfigSectionValueTransformer"/> (usually for testing purposes).
	/// </summary>
	public BranchConfigurationBuilderBase(IGitRepositoryProvider gitRepositoryProvider, IBranchConfigValueTransformer branchConfigSectionValueTransformer)
	{
		BranchConfigSectionValueTransformer = branchConfigSectionValueTransformer;
		string configPath = AppDomain.CurrentDomain.BaseDirectory;
		BranchName = gitRepositoryProvider.GetBranch(configPath);
	}
}