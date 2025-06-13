using System.Text.Json.Serialization;

namespace Havit.Tools.AzureDevopForcePushChecker.Api;

public class AccessControlListResponse
{
	[JsonPropertyName("value")]
	public List<AccessControlList> Value { get; set; }
}
