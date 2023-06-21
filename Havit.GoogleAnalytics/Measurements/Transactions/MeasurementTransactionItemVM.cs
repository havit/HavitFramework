using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Havit.GoogleAnalytics.Measurements.Transactions;

/// <summary>
/// Transaction item
/// </summary>
public class MeasurementTransactionItemVM
    {
	/// <summary>
	/// [Required]
	/// Specifies the item name.
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// [Optional]
	/// Specifies the price for a single item / unit.
	/// </summary>
	public decimal? Price { get; set; }

	/// <summary>
	/// [Optional]
	/// Specifies the number of items purchased.
	/// </summary>
	public int? Quantity { get; set; }

	/// <summary>
	/// Specifies the SKU or item code.
	/// </summary>
	public string Code { get; set; }

	/// <summary>
	/// [Optional]
	/// Specifies the category that the item belongs to.
	/// </summary>
	public string Category { get; set; }
}
