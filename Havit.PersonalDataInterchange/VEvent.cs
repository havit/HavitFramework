namespace Havit.PersonalDataInterchange;

/// <summary>
/// Class representing a scheduled event
/// </summary>
public class VEvent : VCalendarItemBase
{
	/// <summary>
	/// Used to determine the order of queries if the calendar contains multiple events
	/// </summary>
	public DateTime? Stamp { get; set; }

	/// <summary>
	/// Indicates an all-day event
	/// </summary>
	public bool AllDay { get; set; }

	/// <summary>
	/// End of the event
	/// </summary>
	public DateTime? End { get; set; }

	/// <summary>
	/// Sound alert
	/// </summary>
	public VAlarm Alarm { get; set; }

	internal override void WriteToStream(StreamWriter writer)
	{
		writer.WriteLine("BEGIN:VEVENT");
		writer.WriteLine("UID:" + UID);
		if (Stamp != null)
		{
			writer.WriteLine("DTSTAMP:" + VCalendarHelper.FormatDateTime(Stamp.Value));
		}

		if (Start != null)
		{
			writer.WriteLine("DTSTART:" + VCalendarHelper.FormatDateTime(Start.Value));
		}

		if (End != null)
		{
			writer.WriteLine("DTEND:" + VCalendarHelper.FormatDateTime(End.Value.AddDays(1).AddMinutes(-1)));
		}
		else if (Start != null)
		{
			writer.WriteLine("DTEND:" + VCalendarHelper.FormatDateTime(Start.Value.AddDays(1).AddMinutes(-1)));
		}

		writer.WriteLine("SUMMARY:" + Summary);
		writer.WriteLine("DESCRIPTION:" + Description);
		writer.WriteLine("CLASS:PUBLIC");
		if (!string.IsNullOrEmpty(Categories))
		{
			writer.WriteLine("CATEGORIES:" + Categories);
		}
		if (Alarm != null)
		{
			Alarm.WriteToStream(writer);
		}

		if (AllDay)
		{
			writer.WriteLine("X-MICROSOFT-CDO-ALLDAYEVENT:TRUE");
		}
		writer.WriteLine("END:VEVENT");
	}
}
