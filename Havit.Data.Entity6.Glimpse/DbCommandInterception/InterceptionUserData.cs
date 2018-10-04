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
	/// Slouží k
	///		a) zachování původních hodnoty v UserData, která mohla být nastavena jiným interceptorem.
	///		b) uložení stopek pro měření doby trvání dotazu
	/// </summary>
	internal class InterceptionUserData
	{
		#region OriginalUserData
		/// <summary>
		/// Slouží k zachování původních hodnoty v UserData, která mohla být nastavena jiným interceptorem.
		/// </summary>
		public object OriginalUserData { get; set; }
		#endregion

		#region Stopwatch
		/// <summary>
		/// Stopky pro měření doby trvání dotazu.
		/// </summary>
		public Stopwatch Stopwatch { get; set; }
		#endregion
	}
}
