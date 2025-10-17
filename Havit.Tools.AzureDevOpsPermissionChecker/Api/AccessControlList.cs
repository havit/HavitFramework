using System.Text.Json.Serialization;

namespace Havit.Tools.AzureDevopForcePushChecker.Api;

public class AccessControlList
{
	[JsonPropertyName("acesDictionary")]
	public Dictionary<string, AccessControlEntry> AcesDictionary { get; set; }
}
