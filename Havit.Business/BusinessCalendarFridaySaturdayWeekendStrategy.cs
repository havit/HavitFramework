using System;

namespace Havit.Business
{
	internal class BusinessCalendarFridaySaturdayWeekendStrategy : IIsWeekendStrategy
	{
		public bool IsWeekend(DateTime date)
		{
			return (date.DayOfWeek == DayOfWeek.Friday) || (date.DayOfWeek == DayOfWeek.Saturday);
		}
	}
}