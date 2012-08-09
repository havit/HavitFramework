using System;

namespace Havit.Business
{
	/// <summary>
	/// Tøída pro informace o dni.
	/// </summary>
	public class DateInfo
	{
		#region Data Fields - hodnoty
		/// <summary>
		/// Vrátí den, kterému DateInfo patøí.
		/// </summary>
		public DateTime Date
		{
			get { return date; }
		}
		private readonly DateTime date;

		/// <summary>
		/// Indikuje, zda-li je den svátkem.
		/// </summary>
		public bool IsHoliday
		{
			get { return isHoliday; }
		}
		private bool isHoliday;

		/// <summary>
		/// Textový popis svátku, pokud je den svátkem.
		/// </summary>
		public string HolidayDescription
		{
			get { return holidayDescription; }
		}
		private string holidayDescription;
		#endregion

		#region Constructor
		/// <summary>
		/// Vytvoøí instanci <see cref="DateInfo"/>.
		/// </summary>
		/// <param name="date">den, který má být reprezentován</param>
		public DateInfo(DateTime date)
		{
			this.date = date.Date;
		}
		#endregion

		#region SetAsHoliday
		/// <summary>
		/// Nastaví den jako svátek.
		/// </summary>
		/// <param name="holidayDescription">textový popis svátku</param>
		public void SetAsHoliday(string holidayDescription)
		{
			this.isHoliday = true;
			this.holidayDescription = holidayDescription;
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
