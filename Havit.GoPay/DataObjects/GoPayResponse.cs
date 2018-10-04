using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Havit.GoPay.Codebooks;
using Havit.GoPay.DataObjects.Errors;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Havit.GoPay.DataObjects
{
	/// <summary>
	/// Odpověď od GoPay API.
	/// </summary>
	public class GoPayResponse
	{
		/// <summary>
		/// ID platby
		/// </summary>
		[JsonProperty("id")]
		public long Id { get; internal set; }

		/// <summary>
		/// ID platby, ze které vzešla opakovaná platba
		/// </summary>
		[JsonProperty("parent_id")]
		public long? ParentId { get; internal set; }

		/// <summary>
		/// Číslo objednávky
		/// </summary>
		[JsonProperty("order_number")]
		public string OrderNumber { get; internal set; }

		/// <summary>
		/// Stav platby
		/// </summary>
		[JsonProperty("state")]
		[JsonConverter(typeof(StringEnumConverter))]
		public GoPayPaymentState State { get; internal set; }

		/// <summary>
		/// Částka platby násobena 100
		/// </summary>
		[JsonProperty("amount")]
		internal long AmountMultipliedBy100 { get; set; }

		/// <summary>
		/// Částka platby
		/// </summary>
		[JsonIgnore]
		public decimal Amount => (decimal)AmountMultipliedBy100 / 100;

		/// <summary>
		/// Měna platby
		/// </summary>
		[JsonProperty("currency")]
		[JsonConverter(typeof(StringEnumConverter))]
		public GoPayCurrency? Currency { get; internal set; }

		/// <summary>
		/// Plátce platby
		/// </summary>
		[JsonProperty("payer")]
		public GoPayPayer Payer { get; internal set; }

		/// <summary>
		/// Příjemce platby
		/// </summary>
		[JsonProperty("target")]
		public GoPayTarget Target { get; internal set; }

		/// <summary>
		/// Stav požadavku
		/// </summary>
		[JsonProperty("result")]
		[JsonConverter(typeof(StringEnumConverter))]
		public GoPayOperationResult? Result { get; internal set; }

		/// <summary>
		/// Předautorizace platby
		/// </summary>
		[JsonProperty("preauthorization")]
		public GoPayPreauthorization Preauthorization { get; internal set; }

		/// <summary>
		/// Opakovaná platba
		/// </summary>
		[JsonProperty("recurrence")]
		public GoPayRecurrence Recurrence { get; internal set; }

		/// <summary>
		/// Platební metody
		/// </summary>
		[JsonProperty("groups")]
		public Dictionary<string, GoPayPaymentMethod> PaymentMethods { get; internal set; }

		/// <summary>
		/// Povolené platební nástroje
		/// </summary>
		[JsonProperty("enabledPaymentInstruments")]
		public Dictionary<string, GoPayPaymentInstrumentItem> EnabledPaymentInstruments { get; internal set; }

		/// <summary>
		/// URL platební brány
		/// </summary>
		[JsonProperty("gw_url")]
		public string GatewayUrl { get; internal set; }

		/// <summary>
		/// Chyby při volání GoPay API
		/// </summary>
		[JsonProperty("errors")]
		public virtual GoPayResponseErrorItem[] Errors { get; set; }

		/// <summary>
		/// Datum chyby
		/// </summary>
		[JsonProperty("date_issued")]
		public DateTime? ErrorDateTime { get; set; }

		/// <summary>
		/// Indikuje, zdali GoPay API vrátilo odpověď s chybou
		/// </summary>
		public bool HasErrors => (Errors != null);

		/// <summary>
		/// Typ tokenu
		/// </summary>
		[JsonProperty("token_type")]
		public string TokenType { get; internal set; }

		/// <summary>
		/// Hodnota tokenu
		/// </summary>
		[JsonProperty("access_token")]
		public string AccessToken { get; set; }

		/// <summary>
		/// Expirace tokenu v sekundách
		/// </summary>
		[JsonProperty("expires_in")]
		public int TokenExpiresInSeconds { get; internal set; }
	}
}
