using System.Text.Json.Serialization;

namespace Havit.Tools.AzureDevopForcePushChecker.Api;

public class GetUserInfoResponse
{
	[JsonPropertyName("id")]
	public string Id { get; set; }

	[JsonPropertyName("providerDisplayName")]
	public string DisplayName { get; set; }
}
