using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Havit.GoogleAnalytics.Measurements
{
	/// <summary>
	/// Transaction product
	/// </summary>
	public class GATransactionProduct
	{
		/// <summary>
		/// Product name [Required]
		/// </summary>
		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }

		/// <summary>
		/// Product SKU [Required]
		/// </summary>
		[JsonProperty(PropertyName = "sku")]
		public string Sku { get; set; }

		/// <summary>
		/// Product category [Optional]
		/// </summary>
		[JsonProperty(PropertyName = "category")]
		public string Category { get; set; }

		/// <summary>
		/// Unit price [Required]
		/// </summary>
		[JsonProperty(PropertyName = "price")]
		public decimal Price { get; set; }

		/// <summary>
		/// Number of items [Required]
		/// </summary>
		[JsonProperty(PropertyName = "quantity")]
		public int Quantity { get; set; }
	}
}