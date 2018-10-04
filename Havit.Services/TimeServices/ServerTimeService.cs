using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services.TimeServices
{
	/// <summary>
	/// Služba vracející aktuální čas a datum dle času serveru (DateTime.Now).
	/// </summary>
	public class ServerTimeService : TimeServiceBase
	{
		/// <summary>
		/// Vrací aktuální čas.
		/// </summary>
		public override DateTime GetCurrentTime()
		{
			return DateTime.Now;
		}
	}
}
