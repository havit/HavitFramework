using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services.TimeServices
{
	/// <summary>
	/// Abstraktní předek pro službu vracející aktuální čas a datum v určité časové zóně.
	/// Časová zóna je určena abstraktní vlastností CurrentTimeZone, jejíž implementace se požaduje v potomcích.
	/// </summary>
	public abstract class TimeZoneTimeServiceBase : TimeServiceBase
	{
		/// <summary>
		/// Gets the application time zone.
		/// </summary>
		protected abstract TimeZoneInfo CurrentTimeZone { get; }

		/// <summary>
		/// Vrací aktuální čas v časové zóně dle vlastnosti CurrentTimeZone.
		/// </summary>
		public override sealed DateTime GetCurrentTime()
		{
			return TimeZoneInfo.ConvertTime(DateTime.Now, CurrentTimeZone);
		}
	}
}
