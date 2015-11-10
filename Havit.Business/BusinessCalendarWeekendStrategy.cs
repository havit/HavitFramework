using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Business
{
	/// <summary>
	/// Třída vracející strategii pro určení, které dny jsou víkendem.
	/// </summary>
	public static class BusinessCalendarWeekendStrategy
	{
		#region GetSaturdaySundayStrategy
		/// <summary>
		/// Vrací strategii, která považuje za víkend sobotu a neděli.
		/// </summary>
		public static IIsWeekendStrategy GetSaturdaySundayStrategy()
		{
			return new BusinessCalendarSaturdaySundayWeekendStrategy();
		}
		#endregion

		#region GetFridaySaturdayStrategy
		/// <summary>
		/// Vrací strategii, která považuje za víkend pátek a sobotu.
		/// </summary>
		public static IIsWeekendStrategy GetFridaySaturdayStrategy()
		{
			return new BusinessCalendarFridaySaturdayWeekendStrategy();
		}
		#endregion

	}
}
