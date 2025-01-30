namespace Havit.Finance;

/// <summary>Indicates when payments are due when calling financial methods.</summary>
public enum DueDate
{
	/// <summary>Falls at the end of the date interval</summary>
	EndOfPeriod = 0,

	/// <summary>Falls at the beginning of the date interval</summary>
	BeginningOfPeriod = 1
}
