using Havit.Diagnostics.Contracts;

namespace Havit;

/// <summary>
/// Extension methods for DateTime.
/// </summary>
public static class DateTimeExtension
{
	/// <summary>
	/// Returns the time (timespan) remaining until the next "whole rounding".
	/// For example, if the rounding is TimeSpan.FromHours(1), it returns the time remaining until the next whole hour.
	/// See examples.
	/// </summary>
	/// <param name="dateTime">The DateTime from which to calculate.</param>
	/// <param name="rounding">The rounding for which to find the remaining time.</param>
	/// <example>
	/// If the rounding is TimeSpan.FromHours(1), then for dateTime 16:40 (regardless of the date), TimeSpan.FromMinutes(20) is returned, because there are 20 minutes remaining until the next whole hour.
	/// If the rounding is TimeSpan.FromHours(1), then for dateTime 16:59 (regardless of the date), TimeSpan.FromMinutes(1) is returned, because there is 1 minute remaining until the next whole hour.
	/// If the rounding is TimeSpan.FromHours(1), then for dateTime 16:00 (regardless of the date), TimeSpan.FromMinutes(60) is returned, because there are 60 minutes remaining until the next whole hour.
	/// If the rounding is TimeSpan.FromMinutes(5), then for dateTime 16:00 (regardless of the date), TimeSpan.FromMinutes(5) is returned, because there are 5 minutes remaining until the next whole 5 minutes, which is 16:05.
	/// If the rounding is TimeSpan.FromMinutes(5), then for dateTime 16:03 (regardless of the date), TimeSpan.FromMinutes(2) is returned, because there are 2 minutes remaining until the next whole 5 minutes, which is 16:05.
	/// </example>
	public static TimeSpan GetRemainingToNext(this DateTime dateTime, TimeSpan rounding)
	{
		Contract.Requires<ArgumentException>(rounding >= TimeSpan.Zero, nameof(rounding));

		return new TimeSpan(rounding.Ticks - (dateTime.Ticks % rounding.Ticks));
	}
}
