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
		/// Zahrne do počtu dnů i koncové datum (od pondělí do pátku bude 5 pracovních dnů).
		/// </summary>
		IncludeEndDate = 0,

		/// <summary>
		/// Vyloučí z počtu pracovních dnů koncové datum (standardní rozdíl dvou dat; pokud jsou shodná, rozdíl je 0).
		/// </summary>
		ExcludeEndDate = 1
	}
}
