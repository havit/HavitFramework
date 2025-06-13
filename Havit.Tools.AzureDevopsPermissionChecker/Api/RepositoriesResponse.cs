using System.Text.Json.Serialization;

namespace Havit.Tools.AzureDevopForcePushChecker.Api;

// API Response models
public class RepositoriesResponse
{
	[JsonPropertyName("value")]
	public List<RepositoryResult> Value { get; set; }
}
