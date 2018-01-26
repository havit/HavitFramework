using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.GoPay.Codebooks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Havit.GoPay.DataObjects
{
	/// <summary>
	/// Plátce platby
	/// </summary>
	public class GoPayPayer
	{
		/// <summary>
		/// Povolené platební nástroje
		/// </summary>
		[JsonProperty("allowed_payment_instruments", ItemConverterType = typeof(StringEnumConverter), NullValueHandling = NullValueHandling.Ignore)]
		public List<GoPayPaymentInstrument> AllowedPaymentInstruments { get; set; }

		/// <summary>
		/// Výchozí platební nástroj - uvidí plátce jako první možnost platby
		/// </summary>
		[JsonProperty("default_payment_instrument")]
		[JsonConverter(typeof(StringEnumConverter))]
		public GoPayPaymentInstrument DefaultPaymentInstrument { get; set; }

		/// <summary>
		/// Výchozí banka resp. její SWIFT kód - pro bankovní převod bude mít plátce tuto banku předvybranou
		/// </summary>
		[JsonProperty("default_swift")]
		[JsonConverter(typeof(StringEnumConverter))]
		public GoPaySwift DefaultSwift { get; set; }

		/// <summary>
		/// Povolené banky resp. povolené SWIFT kódy
		/// </summary>
		[JsonProperty("allowed_swifts", ItemConverterType = typeof(StringEnumConverter))]
		public List<GoPaySwift> AllowedSwifts { get; set; }

		/// <summary>
		/// Kontakt na plátce
		/// </summary>
		[JsonProperty("contact")]
		public GoPayContact Contact { get; set; }
	}
}
