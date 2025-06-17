using System.Text.Json.Serialization;

namespace Havit.Tools.AzureDevopForcePushChecker.Api;

public class RefItem
{
	[JsonPropertyName("name")]
	public string Name { get; set; }
}
