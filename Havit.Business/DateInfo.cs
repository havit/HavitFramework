using System;

namespace Havit.Business
{
	/// <summary>
	/// Třída pro informace o dni.
	/// </summary>
	public class DateInfo : IDateInfo
	{
		#region Data Fields - hodnoty
		/// <summary>
		/// Vrátí den, kterému DateInfo patří.
		/// </summary>
		public DateTime Date
		{
			get { return _date; }
		}
		private readonly DateTime _date;

		/// <summary>
		/// Indikuje, zdali je den svátkem.
		/// </summary>
		public bool IsHoliday
		{
			get { return _isHoliday; }
		}
		private bool _isHoliday;

		/// <summary>
		/// Textový popis svátku, pokud je den svátkem.
		/// </summary>
		public string HolidayDescription
		{
			get { return _holidayDescription; }
		}
		private string _holidayDescription;
		#endregion

		#region Constructor
		/// <summary>
		/// Vytvoří instanci <see cref="DateInfo"/>.
		/// </summary>
		/// <param name="date">den, který má být reprezentován</param>
		public DateInfo(DateTime date)
		{
			this._date = date.Date;
		}
		#endregion

		#region SetAsHoliday
		/// <summary>
		/// Nastaví den jako svátek.
		/// </summary>
		/// <param name="holidayDescription">textový popis svátku</param>
		public void SetAsHoliday(string holidayDescription)
		{
			this._isHoliday = true;
			this._holidayDescription = holidayDescription;
		}

		/// <summary>
		/// Nastaví den jako svátek.
		/// </summary>
		public void SetAsHoliday()
		{
			SetAsHoliday(null);
		}
		#endregion
	}
}
