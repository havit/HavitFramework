using System.Text.Json.Serialization;

namespace Havit.Tools.AzureDevopForcePushChecker.Api;

public class RepositoryResult
{
	[JsonPropertyName("id")]
	public string Id { get; set; }

	[JsonPropertyName("name")]
	public string Name { get; set; }

	[JsonPropertyName("isDisabled")]
	public bool IsDisabled { get; set; }
}
