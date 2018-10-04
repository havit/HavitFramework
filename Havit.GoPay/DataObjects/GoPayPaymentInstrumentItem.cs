using System.Collections.Generic;
using Havit.GoPay.Codebooks;
using Newtonsoft.Json;

namespace Havit.GoPay.DataObjects
{
	/// <summary>
	/// Platební nástroj
	/// </summary>
	public class GoPayPaymentInstrumentItem
	{
		/// <summary>
		/// Lokalizovaný název platebního nástroje. Klíčem slovníku je zkratka jazyka - např.: "cs"
		/// </summary>
		[JsonProperty("label")]
		public Dictionary<string, string> Label { get; set; }

		/// <summary>
		/// Obrázky/loga platebního nástroje
		/// </summary>
		[JsonProperty("image")]
		public GoPayPaymentMethodImage Image { get; set; }

		/// <summary>
		/// Měny
		/// </summary>
		[JsonProperty("currencies")]
		public List<GoPayCurrency> Currencies { get; set; }

		/// <summary>
		/// Skupina, do které platební metoda náleží
		/// </summary>
		[JsonProperty("group")]
		public string Group { get; internal set; }

		/// <summary>
		/// Povolené banky resp. jejich SWIFT kódy
		/// </summary>
		[JsonProperty("enabledSwifts")]
		public Dictionary<string, GoPayEnabledSwift> EnabledSwifts { get; set; } 
	}
}