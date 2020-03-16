using System.Collections;
using System.Collections.Generic;

namespace Havit.GoogleAnalytics.Measurements.Transactions
{
	/// <summary>
	/// Standard Ecommerce Transaction (for GTM data-layer)
	/// https://support.google.com/tagmanager/answer/6107169
	/// </summary>
	public class GAEMeasurementTransaction : MeasurementModelBase
	{
		/// <summary>
		/// [Required]
		/// Defines transaction hit type.
		/// </summary>
		public override MeasurementHitType HitType => MeasurementHitType.Transaction;

		/// <summary>
		/// [Required]
		/// Unique transaction identifier 
		/// </summary>
		public string TransactionId { get; set; }

		/// <summary>
		/// [Optional]
		/// Partner or store 
		/// </summary>
		public string TransactionAffiliation { get; set; }

		/// <summary>
		/// [Required]
		/// Total value of the transaction
		/// </summary>
		public decimal TransactionTotal { get; set; }

		/// <summary>
		/// [Optional]
		/// Shipping charge for the transaction
		/// </summary>
		public decimal? TransactionShipping { get; set; }

		/// <summary>
		/// [Optional]
		/// Tax amount for the transaction 
		/// </summary>
		public decimal? TransactionTax { get; set; }

		/// <summary>
		/// [Optional]
		/// List of items purchased in the transaction 
		/// </summary>
		public IList<GATransactionProduct> TransactionProducts { get; } = new List<GATransactionProduct>();
	}
}