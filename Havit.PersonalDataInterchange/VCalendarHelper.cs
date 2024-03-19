namespace Havit.PersonalDataInterchange;

internal static class VCalendarHelper
{
	public static string FormatDateTime(DateTime date)
	{
		return date.ToUniversalTime().ToString("yyyyMMdd\\THHmmssZ");
	}

	public static string FormatAllDayDateTime(DateTime date)
	{
		return date.ToString("yyyyMMdd");
	}
}
