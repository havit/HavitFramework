using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.Configuration.Git.Core;
/// <summary>
/// Marker iterface for BranchConfigSectionValueTransformers 
/// </summary>
public interface IBranchConfigValueTransformer
{
	/// <summary>
	/// Applies <paramref name="branchName"/> to placeholders in <paramref name="originalValue"/>
	/// </summary>
	/// <param name="originalValue"></param>
	/// <param name="branchName"></param>
	/// <returns>originalValue if branchName is null or empty or originalValue with replaced placeholder(s) </returns>
	string TransformConfigValue(string originalValue, string branchName);
}
