using System.Text.Json.Serialization;

namespace Havit.Tools.AzureDevopForcePushChecker.Api;

public class AccessControlEntry
{
	[JsonPropertyName("allow")]
	public int Allow { get; set; }

	[JsonPropertyName("deny")]
	public int Deny { get; set; }

	[JsonPropertyName("extendedInfo")]
	public ExtendedInfoEntry ExtendedInfo { get; set; }
}
