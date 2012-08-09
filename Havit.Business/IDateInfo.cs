using System;
namespace Havit.Business
{
	/// <summary>
	/// Interface představující informace o jednom dni pro business-calendar.
	/// </summary>
	public interface IDateInfo
	{
		#region Date
		/// <summary>
		/// Datum, kterého se objekt týká.
		/// </summary>
		DateTime Date { get; } 
		#endregion

		#region IsHoliday
		/// <summary>
		/// Indikuje, zda-li je den svátkem.
		/// </summary>
		bool IsHoliday { get; } 
		#endregion
	}
}
