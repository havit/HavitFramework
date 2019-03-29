using Newtonsoft.Json;

namespace Havit.GoPay.DataObjects
{
	/// <summary>
	/// Návratová a notifikační URL
	/// </summary>
	public class GoPayCallback
	{
		/// <summary>
		/// URL adresa pro návrat na eshop
		/// </summary>
		[JsonProperty("return_url")]
		public string ReturnUrl { get; set; }

		/// <summary>
		/// URL adresa pro odeslání asynchronní notifikace v případě změny stavu platby
		/// </summary>
		[JsonProperty("notification_url")]
		public string NotificationUrl { get; set; }
	}
}