using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business
{
	/// <summary>
	/// Options pro volání metody <see cref="BusinessCalendar.CountBusinessDays"/>.
	/// </summary>
	public enum CountBusinessDaysOptions
	{
		/// <summary>
		/// Zahrne do poètu dnù i koncové datum (od pondìlí do pátku bude 5 pracovních dnù).
		/// </summary>
		IncludeEndDate = 0,

		/// <summary>
		/// Vylouèí z poètu pracovních dnù koncové datum (standardní rozdíl dvou dat; pokud jsou shodná, rozdíl je 0).
		/// </summary>
		ExcludeEndDate = 1
	}
}
