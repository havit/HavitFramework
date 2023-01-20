namespace Havit.Services.BusinessCalendars
{
	/// <summary>
	/// Třída vracející strategii pro určení, které dny jsou víkendem.
	/// </summary>
	public static class BusinessCalendarWeekendStrategy
	{
		/// <summary>
		/// Vrací strategii, která považuje za víkend sobotu a neděli.
		/// </summary>
		public static IIsWeekendStrategy GetSaturdaySundayStrategy()
		{
			return new BusinessCalendarSaturdaySundayWeekendStrategy();
		}

		/// <summary>
		/// Vrací strategii, která považuje za víkend pátek a sobotu.
		/// </summary>
		public static IIsWeekendStrategy GetFridaySaturdayStrategy()
		{
			return new BusinessCalendarFridaySaturdayWeekendStrategy();
		}
	}
}
