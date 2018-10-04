using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.GoPay.Codebooks;
using Newtonsoft.Json;

namespace Havit.GoPay.DataObjects
{
	/// <summary>
	/// Reprezentuje banku povolenou pro platební metodu
	/// </summary>
	public class GoPayEnabledSwift
	{
		/// <summary>
		/// Lokalizované názvy bank
		/// </summary>
		[JsonProperty("label")]
		public Dictionary<string, string> Label { get; set; }

		/// <summary>
		/// Odkazy na obrázky/loga banky
		/// </summary>
		[JsonProperty("image")]
		public GoPayPaymentMethodImage Image { get; set; }

		/// <summary>
		/// Měny banky
		/// </summary>
		[JsonProperty("currencies")]
		public Dictionary<string, GoPayCurrencyItem> Currencies { get; set; }
	}
}
