using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Havit.GoPay.Codebooks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Havit.GoPay.DataObjects
{
	public sealed class GoPayRequest : GoPayRequestBase
	{
		/// <summary>
		/// Plátce platby
		/// </summary>
		[JsonProperty("payer", NullValueHandling = NullValueHandling.Ignore)]
		public GoPayPayer Payer { get; set; }

		/// <summary>
		/// Příjemce platby
		/// </summary>
		[JsonProperty("target", NullValueHandling = NullValueHandling.Ignore)]
		public GoPayTarget Target { get; set; }

		/// <summary>
		/// Celková částka platby - suma celkových částek položek objednávky
		/// </summary>
		[JsonProperty("amount")]
		internal long TotalAmountMultipliedBy100 => Items.Sum(item => item.Count * item.AmountMultipliedBy100);

		/// <summary>
		/// Měna platby
		/// </summary>
		[JsonProperty("currency")]
		[JsonConverter(typeof(StringEnumConverter))]
		public GoPayCurrency Currency { get; set; }

		/// <summary>
		/// Číslo objednávky
		/// </summary>
		[JsonProperty("order_number")]
		public string OrderNumber { get; set; }

		/// <summary>
		/// Popis objednávky
		/// </summary>
		[JsonProperty("order_description")]
		public string OrderDescription { get; set; }

		/// <summary>
		/// Detailně rozepsané jednotlivé položky objednávky z nichž je vypočtena částka platby
		/// </summary>
		[JsonProperty("items")]
		public List<GoPayRequestItem> Items { get; set; }

		/// <summary>
		/// Návratová a notifikační URL
		/// </summary>
		[JsonProperty("callback", NullValueHandling = NullValueHandling.Ignore)]
		public GoPayCallback Callback { get; set; }

		/// <summary>
		/// Dodatečné parametry platby
		/// </summary>
		[JsonProperty("additional_params", NullValueHandling = NullValueHandling.Ignore)]
		public List<GoPayAdditionalParameter> AdditionalParameters { get; set; }

		/// <summary>
		/// Natavení jazyka platební brány
		/// </summary>
		[JsonProperty("lang", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(StringEnumConverter))]
		public GoPayLanguage? Language { get; set; }

		/// <summary>
		/// Aktivace předautorizované platby
		/// </summary>
		[JsonProperty("preauthorization", NullValueHandling = NullValueHandling.Ignore)]
		public bool? Preauthorization { get; set; }

		/// <summary>
		/// Nastavení opakované platby
		/// </summary>
		[JsonProperty("recurrence", NullValueHandling = NullValueHandling.Ignore)]
		public GoPayRecurrence Recurrence { get; set; }

		public GoPayRequest(string accessToken) : base(accessToken)
		{
		}
	}
}
