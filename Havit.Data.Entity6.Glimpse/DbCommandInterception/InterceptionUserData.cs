using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Entity.Glimpse.DbCommandInterception
{
	/// <summary>
	/// User data pro interceptor.
	/// Slouží k uložení stopek pro měření doby trvání dotazu.
	/// </summary>
	internal class InterceptionUserData
	{
		/// <summary>
		/// Stopky pro měření doby trvání dotazu.
		/// </summary>
		public Stopwatch Stopwatch { get; set; }
	}
}
