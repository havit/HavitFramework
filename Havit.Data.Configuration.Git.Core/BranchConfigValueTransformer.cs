using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.Configuration.Git.Core;
/// <summary>
/// Class for transforming config values (as strings) via placeholders and branch name
/// </summary>
public class BranchConfigValueTransformer
{
	private List<(string placeHolder, string branchReplacement)> branchNamePlaceHolderSettings = new List<(string placeHolder, string branchReplacement)>
	{
		("#BRANCH_NAME#", "{0}"),
		("#_BRANCH_NAME#", "_{0}"),
		("#-BRANCH_NAME#", "-{0}"),
		("{BRANCH_NAME}", "{0}"),
		("{_BRANCH_NAME}", "_{0}"),
		("{-BRANCH_NAME}", "-{0}")


	};
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
			foreach (var branchNamePlaceholderSetting in branchNamePlaceHolderSettings)
			{
				originalValue = originalValue.Replace(branchNamePlaceholderSetting.placeHolder, string.Format(branchNamePlaceholderSetting.branchReplacement, branchName));
			}
			return originalValue;
		}
	}
}
