using System;
namespace Havit.Services.TimeServices;

/// <summary>
/// Interface představující informace o jednom dni pro business-calendar.
/// </summary>
public interface IDateInfo
{
	/// <summary>
	/// Datum, kterého se objekt týká.
	/// </summary>
	DateTime Date { get; }

	/// <summary>
	/// Indikuje, zdali je den svátkem.
	/// </summary>
	bool IsHoliday { get; }
}
