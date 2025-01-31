using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Havit.GoPay.DataObjects;

/// <summary>
/// Platební metoda
/// </summary>
public class GoPayPaymentMethod
{
	/// <summary>
	/// Lokalizovaný název platební metody - klíčem je zkratka jazyka např. "cs"
	/// </summary>
	[JsonProperty("label")]
	public Dictionary<string, string> Label { get; set; }
}
