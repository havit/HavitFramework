using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;

namespace Havit.GoogleAnalytics.Measurements.Transactions
{
	/// <summary>
	/// Transaction item
	/// </summary>
	public class MeasurementTransactionItem : MeasurementModelBase
	{
		internal MeasurementTransactionItem(string transactionId, MeasurementTransactionItemVM itemVM)
		{
			TransactionId = transactionId;
			Name = itemVM.Name;
			Price = itemVM.Price;
			Quantity = itemVM.Quantity;
			Code = itemVM.Code;
			Category = itemVM.Category;
		}

		/// <summary>
		/// [Required]
		/// Defines transaction item hit type.
		/// </summary>
		public override MeasurementHitType HitType => MeasurementHitType.Item;

		/// <summary>
		/// [Required]
		/// A unique identifier for the transaction. 
		/// This value should be the same for both the Transaction hit and Items hits associated to the particular transaction.
		/// </summary>
		[Required]
		[ParameterName("ti")]
		public string TransactionId { get; set; }

		/// <summary>
		/// [Required]
		/// Specifies the item name.
		/// </summary>
		[Required]
		[ParameterName("in")]
		public string Name { get; set; }

		/// <summary>
		/// [Optional]
		/// Specifies the price for a single item / unit.
		/// </summary>
		[ParameterName("ip")]
		public decimal? Price { get; set; }

		/// <summary>
		/// [Optional]
		/// Specifies the number of items purchased.
		/// </summary>
		[ParameterName("iq")]
		public int? Quantity { get; set; }

		/// <summary>
		/// Specifies the SKU or item code.
		/// </summary>
		[ParameterName("ic")]
		public string Code { get; set; }

		/// <summary>
		/// [Optional]
		/// Specifies the category that the item belongs to.
		/// </summary>
		[ParameterName("iv")]
		public string Category { get; set; }
	}
}