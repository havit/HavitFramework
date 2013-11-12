using System;
using System.Collections;
using System.Collections.Generic;

namespace Havit.Business
{
	/// <summary>
	/// Třída poskytující funkčnost pro práci s pracovním kalendářem,
	/// pracovními dny, svátky, atp.
	/// </summary>
	/// <remarks>
	/// Pracovním dnem (business day) je den, který není sobotou, nedělí ani svátkem.<br/>
	/// Třída se instancializuje se sadou významných dnů (zpravidla svátků), nebo bez svátků (pracovním
	/// dnem je pak den, který není sobotou ani nedělí).<br/>
	/// Jako svátky (holiday) lze samozřejmě předat i různé dovolené apod.<br/>
	/// <br/>
	/// Jednou vytvořenou instanci třídy lze s výhodou opakovaně používat.
	/// </remarks>
	public class BusinessCalendar
	{
		#region dates (private)
		/// <summary>
		/// Interní seznam významných dnů, tj. dnů, které se liší od běžného pracovního dne (např. svátků, atp.).<br/>
		/// Klíč je DateTime, hodnota je DateInfo.
		/// </summary>
		private readonly IDictionary<DateTime, IDateInfo> dates = null;
		#endregion

		#region Constructors
		/// <summary>
		/// Vytvoří instanci <see cref="BusinessCalendar"/> bez významných dnů.<br/>
		/// Pracovními dny budou všechny dny mimo víkendů, dokud nebudou přidány nějaké svátky.
		/// </summary>
		public BusinessCalendar()
		{
			this.dates = new Dictionary<DateTime, IDateInfo>();
		}

		/// <summary>
		/// Vytvoří instanci <see cref="BusinessCalendar"/>.<br/>
		/// </summary>
		/// <param name="dateInfoDictionary">dictionary s informacemi o významných dnech</param>
		public BusinessCalendar(IDictionary<DateTime, IDateInfo> dateInfoDictionary)
		{
			this.dates = dateInfoDictionary;
		}

		/// <summary>
		/// Vytvoří instanci <see cref="BusinessCalendar"/>.<br/>
		/// Svátky jsou předány v poli <see cref="System.DateTime"/>.
		/// </summary>
		/// <param name="holidays">pole svátků <see cref="System.DateTime"/></param>
		public BusinessCalendar(DateTime[] holidays)
		{
			this.dates = new Dictionary<DateTime, IDateInfo>();
			foreach (DateTime holiday in holidays)
			{
				DateInfo di = new DateInfo(holiday.Date);
				di.SetAsHoliday();
				this.dates.Add(holiday.Date, di);
			}
		}
		#endregion

		#region FillDates
		/// <summary>
		/// Přidá do nastavení <see cref="BusinessCalendar"/> významné dny. Pokud již některý den v kalendáři existuje, přepíše ho. 
		/// </summary>
		/// <typeparam name="T">typ významných dnů (musí implementovat rozhraní <see cref="IDateInfo"/>)</typeparam>
		/// <param name="dateInfos">kolekce významných dnů</param>
		public void FillDates<T>(IEnumerable<T> dateInfos)
			where T : IDateInfo
		{
			foreach (T item in dateInfos)
			{
				dates[item.Date.Date] = item;
			}
		}
		#endregion

		#region GetNextBusinessDay, GetPreviousBusinessDay
		/// <summary>
		/// Určí následující pracovní den.
		/// </summary>
		/// <param name="time"><see cref="System.DateTime"/>, ke kterému má být následující pracovní den určen.</param>
		/// <returns><see cref="System.DateTime"/>, který je následujícím pracovním dnem.</returns>
		/// <remarks>Časový údaj zůstane nedotčen.</remarks>
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
		/// Určí předchozí pracovní den.
		/// </summary>
		/// <param name="time"><see cref="System.DateTime"/>, ke kterému má být předchozí pracovní den určen.</param>
		/// <returns><see cref="System.DateTime"/>, který je předchozím pracovním dnem.</returns>
		/// <remarks>Časový údaj zůstane nedotčen.</remarks>
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
		/// Určí den, který je x-tým následujícím pracovním dnem po dni zadaném.
		/// </summary>
		/// <param name="time"><see cref="System.DateTime"/>, od kterého se určovaný den odvíjí.</param>
		/// <param name="businessDays">kolikátý pracovní den má být určen</param>
		/// <returns><see cref="System.DateTime"/>, který je x-tým následujícím pracovním dnem po dni zadaném.</returns>
		/// <remarks>Časový údaj zůstane nedotčen.</remarks>
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
		/// Určí, zdali je zadaný den dnem pracovním.
		/// </summary>
		/// <param name="time"><see cref="DateTime"/>, u kterého chceme vlastnosti zjistit</param>
		/// <returns><b>false</b>, pokud je <see cref="DateTime"/> víkendem nebo svátkem; jinak <b>true</b></returns>
		public virtual bool IsBusinessDay(DateTime time)
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
		/// Zjistí, zdali je <see cref="System.DateTime"/> svátkem (dovolenou, ...).
		/// </summary>
		/// <param name="time"><see cref="System.DateTime"/>, u kterého má být vlastnost zjištěna</param>
		/// <returns><b>true</b>, pokud je den v seznamu svátků, s nimiž byl <see cref="BusinessCalendar"/> instanciován; jinak <b>false</b></returns>
		public virtual bool IsHoliday(DateTime time)
		{
			if (dates == null)
			{
				return false;
			}

			IDateInfo dateInfo;
			if (dates.TryGetValue(time.Date, out dateInfo))
			{
				return dateInfo.IsHoliday;
			}
			return false;
		}
		#endregion

		#region IsWeekend
		/// <summary>
		/// Určí, zdali je zadaný den sobotou nebo nedělí.
		/// </summary>
		/// <param name="time"><see cref="System.DateTime"/>, u kterého určujeme</param>
		/// <returns><b>true</b>, pokud je zadaný <see cref="System.DateTime"/> sobota nebo neděle; jinak <b>false</b></returns>
		public virtual bool IsWeekend(DateTime time)
		{
			DayOfWeek dayOfWeek = time.DayOfWeek;
			if ((dayOfWeek == DayOfWeek.Saturday) || (dayOfWeek == DayOfWeek.Sunday))
			{
				return true;
			}
			return false;
		}
		#endregion

		#region CountBusinessDays
		/// <summary>
		/// Spočítá počet pracovních dní mezi dvěma daty. 
		/// </summary>
		/// <param name="startDate">počáteční datum</param>
		/// <param name="endDate">koncové datum</param>
		/// <param name="options">Options pro počítání dnů.</param>
		/// <returns>počet pracovních dnů mezi počátečním a koncovým datem (v závislosti na <c>options</c>)</returns>
		public int CountBusinessDays(DateTime startDate, DateTime endDate, CountBusinessDaysOptions options)
		{
			// pokud jsou data obráceně, vrátíme záporný výsledek sama sebe v opačném pořadí dat
			if (startDate > endDate)
			{
				return -CountBusinessDays(endDate, startDate, options);
			}

			int counter = 0;
			DateTime currentDate = startDate;

			// procházíme všecha data až před endDate 
			while (currentDate.Date < endDate.Date)
			{
				if (this.IsBusinessDay(currentDate))
				{
					counter++;
				}
				currentDate = currentDate.AddDays(1);
			}

			// pokud chceme zohlednit endDate pak ho započteme (pokud je pracovní)
			if ((options == CountBusinessDaysOptions.IncludeEndDate) && (this.IsBusinessDay(endDate)))
			{
				counter++;
			}

			return counter;
		}
		#endregion
	}
}
