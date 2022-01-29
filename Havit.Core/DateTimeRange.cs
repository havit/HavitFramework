using System;

namespace Havit;

/// <summary>
/// Indicates range of date - start date and end date.
/// </summary>
public record struct DateTimeRange
{
	/// <summary>
	/// Start date of the range.
	/// </summary>
	public DateTime? StartDate { get; private set; }

	/// <summary>
	/// End date of the range.
	/// </summary>
	public DateTime? EndDate { get; private set; }

	/// <summary>
	/// Creates new instance of the struct.
	/// </summary>
	public DateTimeRange(DateTime? startDate, DateTime? endDate)
	{
		StartDate = startDate;
		EndDate = endDate;
	}
}