using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services.TimeServices;

/// <summary>
/// Abstraktní předek pro implementaci TimeServices.
/// Zajišťuje, aby aktuální datum bylo vždy bráno z aktuálního času (GetCurrentTime) odebráním časové složky (DateTime.Date).
/// </summary>
public abstract class TimeServiceBase : ITimeService
{
	/// <summary>
	/// Vrací aktuální čas.
	/// </summary>
	public abstract DateTime GetCurrentTime();

	/// <summary>
	/// Vrací aktuální datum (bez času).
	/// </summary>
	public DateTime GetCurrentDate()
	{
		return GetCurrentTime().Date;
	}
}
