using Newtonsoft.Json;

namespace Havit.GoPay.DataObjects;

/// <summary>
/// Povolené platební metody
/// </summary>
public class GoPayAllowedPaymentMethods
{
	/// <summary>
	/// Platební metody povolené v administraci GoPay
	/// </summary>
	[JsonProperty("groups")]
	public Dictionary<string, GoPayPaymentMethod> PaymentMethods { get; set; }

	/// <summary>
	/// Povolené platební nástroje v administraci GoPay
	/// </summary>
	[JsonProperty("enabled_payment_instruments")]
	public Dictionary<string, GoPayPaymentInstrumentItem> EnabledPaymentInstruments { get; set; }
}
