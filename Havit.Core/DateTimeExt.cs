using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit
{
	/// <summary>
	/// Rozšiřující funkce pro práci s daty (datumy) <see cref="DateTime"/>.	
	/// </summary>
	public static class DateTimeExt
	{		
		/// <summary>
		/// Vrátí nejmenší (nejdřívější) ze zadaných dat.
		/// </summary>
		public static DateTime Min(params DateTime[] values)
		{
			return values.Min();
		}

		/// <summary>
		/// Vrátí největší (nejposlednější) ze zadaných dat.
		/// </summary>
		public static DateTime Max(params DateTime[] values)
		{
			return values.Max();
		}
	}
}
