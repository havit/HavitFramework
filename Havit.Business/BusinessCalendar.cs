using System;
using System.Collections;

namespace Havit.Business
{
	/// <summary>
	/// Tøída poskytující funkènost pro práci s pracovním kalendáøem,
	/// pracovními dny, svátky, atp.
	/// </summary>
	/// <remarks>
	/// Pracovním dnem (business day) je den, který není sobotou, nedìlí ani svátkem.<br/>
	/// Tøída se instancializuje se sadou svátkù (holidays), nebo bez svátkù (pracovním
	/// dnem je pak den, který není sobotou ani nedìlí).<br/>
	/// Jako svátky (holiday) lze samozøejmì pøedat i rùzné dovolené apod.<br/>
	/// <br/>
	/// Jednou vytvoøenou instanci tøídy lze s výhodou cachovat.
	/// </remarks>
	public sealed class BusinessCalendar
	{
		#region Private data fields
		/// <summary>
		/// Interní dictionary svátkù.<br/>
		/// Klíè je DateTime, hodnota je DateInfo.
		/// </summary>
		private readonly DateInfoDictionary holidayDictionary = null;
		#endregion

		#region Constructors
		/// <summary>
		/// Vytvoøí instanci <see cref="BusinessCalendar"/> bez svátkù.<br/>
		/// Pracovními dny budou všechny dny mimo víkendù, dokud nebudou pøidány nìjaké svátky.
		/// </summary>
		public BusinessCalendar()
		{
		}

		/// <summary>
		/// Vytvoøí instanci <see cref="BusinessCalendar"/> se svátky.<br/>
		/// </summary>
		/// <param name="holidayDictionary"><see cref="DateInfoDictionary"/> se svátky (POUZE SE SVÁTKY!!!)</param>
		/// <remarks>
		/// Nekontroluje se, jestli mají všechny dny z holidayDictionary nastaveno <see cref="DateInfo.IsHoliday"/>.
		/// </remarks>
		public BusinessCalendar(DateInfoDictionary holidayDictionary)
		{
			this.holidayDictionary = holidayDictionary;
		}

		/// <summary>
		/// Vytvoøí instanci <see cref="BusinessCalendar"/> se svátky.<br/>
		/// Svátky jsou pøedány v poli <see cref="System.DateTime"/>.
		/// </summary>
		/// <param name="holidays">pole svátkù <see cref="System.DateTime"/></param>
		public BusinessCalendar(DateTime[] holidays)
		{
			this.holidayDictionary = new DateInfoDictionary();
			foreach (DateTime holiday in holidays)
			{
				this.holidayDictionary.Add(new DateInfo(holiday));
			}
		}
		#endregion

		#region GetNextBusinessDay, GetPreviousBusinessDay
		/// <summary>
		/// Urèí následující pracovní den.
		/// </summary>
		/// <param name="time"><see cref="System.DateTime"/>, ke kterému má být následující pracovní den urèen.</param>
		/// <returns><see cref="System.DateTime"/>, který je následujícím pracovním dnem.</returns>
		/// <remarks>Èasový údaj zùstane nedotèen.</remarks>
		public DateTime GetNextBusinessDay(DateTime time)
		{
			do
			{
				time = time.AddDays(1);
			}
			while (!IsBusinessDay(time));

			return time;
		}

		/// <summary>
		/// Urèí pøedchozí pracovní den.
		/// </summary>
		/// <param name="time"><see cref="System.DateTime"/>, ke kterému má být pøedchozí pracovní den urèen.</param>
		/// <returns><see cref="System.DateTime"/>, který je pøedchozím pracovním dnem.</returns>
		/// <remarks>Èasový údaj zùstane nedotèen.</remarks>
		public DateTime GetPreviousBusinessDay(DateTime time)
		{
			do
			{
				time = time.AddDays(-1);
			}
			while (!IsBusinessDay(time));

			return time;
		}
		#endregion

		#region AddBusinessDays
		/// <summary>
		/// Urèí den, který je x-tým následujícím pracovním dnem po dni zadaném.
		/// </summary>
		/// <param name="time"><see cref="System.DateTime"/>, od kterého se urèovaný den odvíjí.</param>
		/// <param name="businessDays">kolikátý pracovní den má být urèen</param>
		/// <returns><see cref="System.DateTime"/>, který je x-tým následujícím pracovním dnem po dni zadaném.</returns>
		/// <remarks>Èasový údaj zùstane nedotèen.</remarks>
		public DateTime AddBusinessDays(DateTime time, int businessDays)
		{
			if (businessDays >= 0)
			{
				for (int i = 0; i < businessDays; i++)
				{
					time = GetNextBusinessDay(time);
				}
			}
			else
			{
				for (int i = 0; i > businessDays; i--)
				{
					time = GetPreviousBusinessDay(time);
				}
			}
			return time;
		}
		#endregion

		#region IsBusinessDay
		/// <summary>
		/// Urèí, zda-li je zadaný den dnem pracovním.
		/// </summary>
		/// <param name="time"><see cref="DateTime"/>, u kterého chceme vlastnosti zjistit</param>
		/// <returns><b>false</b>, pokud je <see cref="DateTime"/> víkendem nebo svátkem; jinak <b>true</b></returns>
		public bool IsBusinessDay(DateTime time)
		{
			if (IsWeekend(time) || IsHoliday(time))
			{
				return false;
			}
			return true;
		}
		#endregion

		#region IsHoliday
		/// <summary>
		/// Zjistí, zda-li je <see cref="System.DateTime"/> svátkem (dovolenou, ...).
		/// </summary>
		/// <param name="time"><see cref="System.DateTime"/>, u kterého má být vlastnost zjištìna</param>
		/// <returns><b>true</b>, pokud je den v seznamu svátkù, s nimiž byl <see cref="BusinessCalendar"/> instanciován; jinak <b>false</b></returns>
		public bool IsHoliday(DateTime time)
		{
			if (holidayDictionary == null)
			{
				return false;
			}
			if (holidayDictionary.Contains(time))
			{
				return true;
			}
			return false;
		}
		#endregion

		#region IsWeekend
		/// <summary>
		/// Urèí, zda-li je zadaný den sobotou nebo nedìlí.
		/// </summary>
		/// <param name="time"><see cref="System.DateTime"/>, u kterého urèujeme</param>
		/// <returns><b>true</b>, pokud je zadaný <see cref="System.DateTime"/> sobota nebo nedìle; jinak <b>false</b></returns>
		public bool IsWeekend(DateTime time)
		{
			DayOfWeek dayOfWeek = time.DayOfWeek;
			if ((dayOfWeek == DayOfWeek.Saturday) || (dayOfWeek == DayOfWeek.Sunday))
			{
				return true;
			}
			return false;
		}
		#endregion
	}
}
