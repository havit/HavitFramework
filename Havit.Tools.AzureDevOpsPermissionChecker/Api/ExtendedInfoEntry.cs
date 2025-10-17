using System.Text.Json.Serialization;

namespace Havit.Tools.AzureDevopForcePushChecker.Api;

public class ExtendedInfoEntry
{
	[JsonPropertyName("effectiveAllow")]
	public int EffectiveAllow { get; set; }

	[JsonPropertyName("effectiveDeny")]
	public int EffectiveDeny { get; set; }
}
