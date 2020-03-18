using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Havit.GoogleAnalytics.Measurements.Transactions
{
	/// <summary>
	/// Standard Ecommerce Transaction (for GTM data-layer)
	/// https://support.google.com/tagmanager/answer/6107169
	/// </summary>
	public class MeasurementTransaction : MeasurementModelBase
	{
		/// <summary>
		/// [Required]
		/// Defines transaction hit type.
		/// </summary>
		public override MeasurementHitType HitType => MeasurementHitType.Transaction;

		/// <summary>
		/// [Required]
		/// A unique identifier for the transaction. 
		/// This value should be the same for both the Transaction hit and Items hits associated to the particular transaction.
		/// </summary>
		[Required]
		[ParameterName("ti")]
		public string TransactionId { get; set; }

		/// <summary>
		/// [Optional]
		/// Specifies the affiliation or store name.
		/// </summary>
		[ParameterName("ta")]
		public string Affiliation { get; set; }

		/// <summary>
		/// [Optional]
		/// Specifies the total revenue associated with the transaction. 
		/// This value should include any shipping or tax costs.
		/// </summary>
		[ParameterName("tr")]
		public decimal? Revenue { get; set; }

		/// <summary>
		/// [Optional]
		/// Specifies the total shipping cost of the transaction.
		/// </summary>
		[ParameterName("ts")]
		public decimal? Shipping { get; set; }

		/// <summary>
		/// [Optional]
		/// Specifies the total tax of the transaction.
		/// </summary>
		[ParameterName("tt")]
		public decimal? Tax { get; set; }
	}
}