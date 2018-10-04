using Newtonsoft.Json;

namespace Havit.GoPay.DataObjects
{
	/// <summary>
	/// Návratová a notifikaèní URL
	/// </summary>
	public class GoPayCallback
	{
		/// <summary>
		/// URL adresa pro návrat na eshop
		/// </summary>
		[JsonProperty("return_url")]
		public string ReturnUrl { get; set; }

		/// <summary>
		/// URL adresa pro odeslání asynchronní notifikace v pøípadì zmìny stavu platby
		/// </summary>
		[JsonProperty("notification_url")]
		public string NotificationUrl { get; set; }
	}
}