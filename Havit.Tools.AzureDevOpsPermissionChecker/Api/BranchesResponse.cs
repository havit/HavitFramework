using System.Text.Json.Serialization;

namespace Havit.Tools.AzureDevopForcePushChecker.Api;

public class BranchesResponse
{
	[JsonPropertyName("value")]
	public List<RefItem> Value { get; set; }
}
