using System;

namespace Havit.Services.TimeServices;

/// <summary>
/// Calculates number of days between two dates, with excluding weekends and public holidays.
/// </summary>
public interface IWorkingDaysCalculator
{
	/// <summary>
	/// Gets or sets the days of week considered a weekend.
	/// Default is WeekendDays.SaturdayAndSunday.
	/// </summary>
	WorkingDaysCalculator.WeekendDays WeekendConfiguration { get; set; }

	/// <summary>
	/// Returns date, which is the x-th business day after the date specified (i.e. 1 = next business day). Time portion of the date will be preserved.
	/// </summary>
	DateTime AddBusinessDays(DateTime date, int businessDaysToAdd);

	/// <summary>
	/// Calculates number of business days in between two dates.
	/// </summary>
	int CountBusinessDays(DateTime startDate, DateTime endDate, bool includeEndDate);

	/// <summary>
	/// Returns next business day. Time portion of the date will be preserved.
	/// </summary>
	DateTime GetNextBusinessDay(DateTime date);

	/// <summary>
	/// Returns previous business day. Time portion of the date will be preserved.
	/// </summary>
	DateTime GetPreviousBusinessDay(DateTime date);

	/// <summary>
	/// Indicates whether the specified date is a business day (not a weekend nor a holiday).		
	/// </summary>
	bool IsBusinessDay(DateTime date);

	/// <summary>
	/// Indicates whether the specified date is a holiday. Does not take weekends into account.
	/// </summary>
	bool IsHoliday(DateTime date);

	/// <summary>
	/// Indicates whether the specified date is a weekend day. Does not take holidays into account.
	/// </summary>
	bool IsWeekend(DateTime date);
}