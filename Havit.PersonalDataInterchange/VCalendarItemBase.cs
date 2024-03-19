namespace Havit.PersonalDataInterchange;

/// <summary>
/// Base class for calendar items
/// </summary>
public abstract class VCalendarItemBase
{
	/// <summary>
	/// Date and time when the item was created
	/// </summary>
	public DateTime? Created { get; set; }

	/// <summary>
	/// Description of the item
	/// </summary>
	public string Description { get; set; }

	/// <summary>
	/// Start of the event
	/// </summary>
	public DateTime? Start { get; set; }

	/// <summary>
	/// Date of the last modification
	/// </summary>
	public DateTime? LastModification { get; set; }

	/// <summary>
	/// Organizer information of the event
	/// </summary>
	public string Organizer { get; set; }

	/// <summary>
	/// Sequence number used to determine the order of queries if the calendar contains multiple items
	/// </summary>
	public string Seq { get; set; }

	/// <summary>
	/// Busy status
	/// </summary>
	public string Status { get; set; }

	/// <summary>
	/// Summary of the item
	/// </summary>
	public string Summary { get; set; }

	/// <summary>
	/// Event ID used to determine the order of queries if the calendar contains multiple items
	/// </summary>
	public string UID { get; set; }

	/// <summary>
	/// URL for extended item information
	/// </summary>
	public string Url { get; set; }

	/// <summary>
	/// Categories that the event belongs to
	/// </summary>
	/// <remarks>
	/// Values are separated by commas.
	/// </remarks>
	public string Categories { get; set; }

	internal abstract void WriteToStream(StreamWriter writer);
}
