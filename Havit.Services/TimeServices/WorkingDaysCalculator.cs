using Havit.Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services.TimeServices
{
	/// <summary>
	/// Calculates number of days between two dates, with excluding weekends and public holidays.
	/// </summary>
	public class WorkingDaysCalculator
    {
		private readonly IDateInfoProvider dateInfoProvider;

		/// <summary>
		/// Gets or sets the days of week considered a weekend.
		/// Default is WeekendDays.SaturdayAndSunday.
		/// </summary>
		/// <remarks>
		/// Vast majority of usages stay with the default value so we don't want to force setting of this option through the constructor.
		/// </remarks>
		public WeekendDays Weekend { get; set; } = WeekendDays.SaturdayAndSunday;

		/// <summary>
		/// Creates new <see cref="WorkingDaysCalculator"/> instance.<br/>
		/// The IDateInfoProvider supports the calendar with information about bank holidays and other non-working days.
		/// </summary>
		public WorkingDaysCalculator(IDateInfoProvider dateInfoProvider)
		{
			Contract.Requires<ArgumentNullException>(dateInfoProvider != null, nameof(dateInfoProvider));

			this.dateInfoProvider = dateInfoProvider;
		}

		/// <summary>
		/// Returns next business day. Time portion of the date will be preserved.
		/// </summary>
		public DateTime GetNextBusinessDay(DateTime date)
		{
			do
			{
				date = date.AddDays(1);
			}
			while (!IsBusinessDay(date));

			return date;
		}

		/// <summary>
		/// Returns previous business day. Time portion of the date will be preserved.
		/// </summary>
		public DateTime GetPreviousBusinessDay(DateTime date)
		{
			do
			{
				date = date.AddDays(-1);
			}
			while (!IsBusinessDay(date));

			return date;
		}

		/// <summary>
		/// Returns date, which is the x-th business day after the date specified (i.e. 1 = next business day). Time portion of the date will be preserved.
		/// </summary>
		public DateTime AddBusinessDays(DateTime date, int businessDaysToAdd)
		{
			if (businessDaysToAdd >= 0)
			{
				for (int i = 0; i < businessDaysToAdd; i++)
				{
					date = GetNextBusinessDay(date);
				}
			}
			else
			{
				for (int i = 0; i > businessDaysToAdd; i--)
				{
					date = GetPreviousBusinessDay(date);
				}
			}
			return date;
		}

		/// <summary>
		/// Indicates whether the specified date is a business day (not a weekend nor a holiday).		
		/// </summary>
		public virtual bool IsBusinessDay(DateTime date)
		{
			if (IsWeekend(date) || IsHoliday(date))
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Indicates whether the specified date is a holiday. Does not take weekends into account.
		/// </summary>
		public virtual bool IsHoliday(DateTime date)
		{
			IDateInfo dateInfo =  dateInfoProvider.GetDateInfo(date.Date);
			if (dateInfo != null)
			{
				return dateInfo.IsHoliday;
			}
			return false;
		}

		/// <summary>
		/// Indicates whether the specified date is a weekend day. Does not take holidays into account.
		/// </summary>
		public virtual bool IsWeekend(DateTime date)
		{
			return date.DayOfWeek switch
			{
				DayOfWeek.Monday => Weekend.HasFlag(WeekendDays.Monday),
				DayOfWeek.Tuesday => Weekend.HasFlag(WeekendDays.Tuesday),
				DayOfWeek.Wednesday => Weekend.HasFlag(WeekendDays.Wednesday),
				DayOfWeek.Thursday => Weekend.HasFlag(WeekendDays.Thursday),
				DayOfWeek.Friday => Weekend.HasFlag(WeekendDays.Friday),
				DayOfWeek.Saturday => Weekend.HasFlag(WeekendDays.Saturday),
				DayOfWeek.Sunday => Weekend.HasFlag(WeekendDays.Sunday),
				_ => throw new InvalidOperationException("Unknown DayOfWeek.")
			};
		}

		/// <summary>
		/// Calculates number of business days in between two dates.
		/// </summary>
		public int CountBusinessDays(DateTime startDate, DateTime endDate, bool includeEndDate)
		{
			if (startDate > endDate)
			{
				return -CountBusinessDays(endDate, startDate, includeEndDate);
			}

			int counter = 0;
			DateTime currentDate = startDate;

			while (currentDate.Date < endDate.Date)
			{
				if (this.IsBusinessDay(currentDate))
				{
					counter++;
				}
				currentDate = currentDate.AddDays(1);
			}

			if (includeEndDate && this.IsBusinessDay(endDate))
			{
				counter++;
			}

			return counter;
		}

		/// <summary>
		/// Flag for specification of weekend days.
		/// </summary>
		[Flags]
		public enum WeekendDays
		{
			None =				0,
			Monday =			0b_0000_0001,
			Tuesday =			0b_0000_0010,
			Wednesday =			0b_0000_0100,
			Thursday =			0b_0000_1000,
			Friday =			0b_0001_0000,
			Saturday =			0b_0010_0000,
			Sunday =			0b_0100_0000,
			SaturdayAndSunday = 0b_0110_0000,
			FridayAndSaturday = 0b_0011_0000
		}
	}
}