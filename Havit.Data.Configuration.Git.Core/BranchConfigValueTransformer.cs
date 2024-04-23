using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.Configuration.Git.Core;
/// <summary>
/// Class for transforming config values (as strings) via placeholders and branch name
/// </summary>
public class BranchConfigValueTransformer : IBranchConfigValueTransformer
{
	/// <summary>
	/// Branch name placeholders
	/// </summary>
	private string[] branchNamePlaceholders = { "#BRANCH_NAME#", "#_BRANCH_NAME#", "{BRANCH_NAME}" };

	/// <summary>
	/// Applies <paramref name="branchName"/> to placeholders in <paramref name="originalValue"/>
	/// </summary>
	/// <param name="originalValue"></param>
	/// <param name="branchName"></param>
	/// <returns>originalValue if branchName is null or empty or originalValue with replaced placeholder(s) </returns>
	public virtual string TransformConfigValue(string originalValue, string branchName)
	{
		if (string.IsNullOrEmpty(branchName) || string.IsNullOrEmpty(originalValue))
		{
			// If there's no branch, there's nothing to replace
			// However currently HeadFileGitRepositoryProvider does not support matching detached HEAD to branch name
			// (scenario commonly occuring on build agent)
			return originalValue;
		}
		else
		{
			branchName = branchName?.Replace("/", "_");
			foreach (string branchNamePlaceholder in branchNamePlaceholders)
			{
				originalValue = originalValue.Replace(branchNamePlaceholder, branchName);
			}
			return originalValue;
		}
	}
}
