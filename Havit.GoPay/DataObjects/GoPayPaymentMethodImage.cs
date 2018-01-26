using Newtonsoft.Json;

namespace Havit.GoPay.DataObjects
{
	/// <summary>
	/// Logo platební metody
	/// </summary>
	public class GoPayPaymentMethodImage
	{
		/// <summary>
		/// Normální formát loga
		/// </summary>
		[JsonProperty("normal")]
		public string Normal { get; set; }

		/// <summary>
		/// Velký formát loga
		/// </summary>
		[JsonProperty("large")]
		public string Large { get; set; }
	}
}