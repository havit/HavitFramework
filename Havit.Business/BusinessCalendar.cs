using System;
using System.Collections;
using System.Collections.Generic;

namespace Havit.Business
{
	/// <summary>
	/// Tøída poskytující funkènost pro práci s pracovním kalendáøem,
	/// pracovními dny, svátky, atp.
	/// </summary>
	/// <remarks>
	/// Pracovním dnem (business day) je den, který není sobotou, nedìlí ani svátkem.<br/>
	/// Tøída se instancializuje se sadou významných dnù (zpravidla svátkù), nebo bez svátkù (pracovním
	/// dnem je pak den, který není sobotou ani nedìlí).<br/>
	/// Jako svátky (holiday) lze samozøejmì pøedat i rùzné dovolené apod.<br/>
	/// <br/>
	/// Jednou vytvoøenou instanci tøídy lze s výhodou opakovanì používat.
	/// </remarks>
	public class BusinessCalendar
	{
		#region dates (private)
		/// <summary>
		/// Interní seznam významných dnù, tj. dnù, které se liší od bìžného pracovního dne (napø. svátkù, atp.).<br/>
		/// Klíè je DateTime, hodnota je DateInfo.
		/// </summary>
		private readonly IDictionary<DateTime, IDateInfo> dates = null;
		#endregion

		#region Constructors
		/// <summary>
		/// Vytvoøí instanci <see cref="BusinessCalendar"/> bez významných dnù.<br/>
		/// Pracovními dny budou všechny dny mimo víkendù, dokud nebudou pøidány nìjaké svátky.
		/// </summary>
		public BusinessCalendar()
		{
			this.dates = new Dictionary<DateTime, IDateInfo>();
		}

		/// <summary>
		/// Vytvoøí instanci <see cref="BusinessCalendar"/>.<br/>
		/// </summary>
		/// <param name="dateInfoDictionary">dictionary s informacemi o významných dnech</param>
		public BusinessCalendar(IDictionary<DateTime, IDateInfo> dateInfoDictionary)
		{
			this.dates = dateInfoDictionary;
		}

		/// <summary>
		/// Vytvoøí instanci <see cref="BusinessCalendar"/>.<br/>
		/// Svátky jsou pøedány v poli <see cref="System.DateTime"/>.
		/// </summary>
		/// <param name="holidays">pole svátkù <see cref="System.DateTime"/></param>
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
		/// Pøidá do nastavení <see cref="BusinessCalendar"/> významné dny. Pokud již nìkterý den v kalendáøi existuje, pøepíše ho. 
		/// </summary>
		/// <typeparam name="T">typ významných dnù (musí implementovat rozhraní <see cref="IDateInfo"/>)</typeparam>
		/// <param name="dateInfos">kolekce významných dnù</param>
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
		/// Zjistí, zda-li je <see cref="System.DateTime"/> svátkem (dovolenou, ...).
		/// </summary>
		/// <param name="time"><see cref="System.DateTime"/>, u kterého má být vlastnost zjištìna</param>
		/// <returns><b>true</b>, pokud je den v seznamu svátkù, s nimiž byl <see cref="BusinessCalendar"/> instanciován; jinak <b>false</b></returns>
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
		/// Urèí, zda-li je zadaný den sobotou nebo nedìlí.
		/// </summary>
		/// <param name="time"><see cref="System.DateTime"/>, u kterého urèujeme</param>
		/// <returns><b>true</b>, pokud je zadaný <see cref="System.DateTime"/> sobota nebo nedìle; jinak <b>false</b></returns>
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
		/// Spoèítá poèet pracovních dní mezi dvìma daty. 
		/// </summary>
		/// <param name="startDate">poèáteèní datum</param>
		/// <param name="endDate">koncové datum</param>
		/// <param name="options">Options pro poèítání dnù.</param>
		/// <returns>poèet pracovních dnù mezi poèáteèním a koncovým datem (v závislosti na <c>options</c>)</returns>
		public int CountBusinessDays(DateTime startDate, DateTime endDate, CountBusinessDaysOptions options)
		{
			// pokud jsou data obrácenì, vrátíme záporný výsledek sama sebe v opaèném poøadí dat
			if (startDate > endDate)
			{
				return -CountBusinessDays(endDate, startDate, options);
			}

			int counter = 0;
			DateTime currentDate = startDate;

			// procházíme všecha data až pøed endDate 
			while (currentDate.Date < endDate.Date)
			{
				if (this.IsBusinessDay(currentDate))
				{
					counter++;
				}
				currentDate = currentDate.AddDays(1);
			}

			// pokud chceme zohlednit endDate pak ho zapoèteme (pokud je pracovní)
			if ((options == CountBusinessDaysOptions.IncludeEndDate) && (this.IsBusinessDay(endDate)))
			{
				counter++;
			}

			return counter;
		}
		#endregion
	}
}
