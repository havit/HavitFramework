using Newtonsoft.Json;

namespace Havit.GoPay.DataObjects
{
	/// <summary>
	/// Dodatečný atribut k měně, kterou banka používá
	/// </summary>
	public class GoPayCurrencyItem
	{
		/// <summary>
		/// Stav symbolizující zda banka podporuje online převod
		/// </summary>
		[JsonProperty("isOnline")]
		public bool IsOnline { get; set; }
	}
}