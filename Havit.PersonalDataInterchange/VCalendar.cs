using System.Collections.ObjectModel;

namespace Havit.PersonalDataInterchange;

/// <summary>
/// Calendar object
/// </summary>
public class VCalendar : VDocumentBase
{
	/// <summary>
	/// Calendar items
	/// </summary>
	public Collection<VCalendarItemBase> Items { get; } = new();

	/// <summary>
	/// VCalendar version
	/// </summary>
	public override double Version => 2.0;

	/// <summary>
	/// Content type used for sending the calendar to the client
	/// </summary>
	public override string ContentType => "text/calendar";

	/// <summary>
	/// Calendar name
	/// </summary>
	public string Name { get; set; }

	/// <inheritdoc />
	public override void WriteToStream(StreamWriter writer)
	{
		writer.WriteLine("BEGIN:VCALENDAR");
		writer.WriteLine($"VERSION:{Version:n0}.0");
		writer.WriteLine("METHOD:PUBLISH");
		writer.WriteLine($"PRODID:{ProductID}");
		writer.WriteLine("X-WR-CALNAME:" + Name);

		foreach (VCalendarItemBase item in Items)
		{
			item.WriteToStream(writer);
		}

		writer.WriteLine("END:VCALENDAR");
	}
}
