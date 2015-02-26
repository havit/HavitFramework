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
		/// Indikuje, zdali je den svátkem. Pokud je hodnota false, je den považován za pracovní.
		/// </summary>
		bool IsHoliday { get; } 
		#endregion
	}
}
