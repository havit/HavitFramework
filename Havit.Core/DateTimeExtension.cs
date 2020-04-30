using Havit.Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit
{
	public static class DateTimeExtension
	{
		/// <summary>
		/// Vrátí čas (timespan) zbývající do dalšího "celého roundingu". 
		/// Např. pokud je rounding TimeSpan.FromHours(1), vrací čas zbývající do další celé hodiny.
		/// Viz příklady.
		/// </summary>
		/// <param name="dateTime">DateTime, od kterého se počítá.</param>
		/// <param name="rounding">Rounding, pro který hledáme zbývající čas.</param>
		/// <example>
		/// Pokud je rounding TimeSpan.FromHours(1), pak pro dateTime 16:40 (bez ohledu na datum) je vrácen TimeSpan.FromMinutes(20), protože zbývá 20 minut do příští celé hodiny.
		/// Pokud je rounding TimeSpan.FromHours(1), pak pro dateTime 16:59 (bez ohledu na datum) je vrácen TimeSpan.FromMinutes(1), protože zbývá 1 minuta do příští celé hodiny.
		/// Pokud je rounding TimeSpan.FromHours(1), pak pro dateTime 16:00 (bez ohledu na datum) je vrácen TimeSpan.FromMinutes(60), protože zbývá 60 minut do příští celé hodiny.
		/// Pokud je rounding TimeSpan.FromMinutes(5), pak pro dateTime 16:00 (bez ohledu na datum) je vrácen TimeSpan.FromMinutes(5), protože zbývá 5 minut do příštích celých 5 minut, což je 16:05.
		/// Pokud je rounding TimeSpan.FromMinutes(5), pak pro dateTime 16:03 (bez ohledu na datum) je vrácen TimeSpan.FromMinutes(2), protože zbývavají 2 minuty do příštích celých 5 minut, což je 16:05.
		/// </example>
		public static TimeSpan GetRemainingToNext(this DateTime dateTime, TimeSpan rounding)
		{
			Contract.Requires<ArgumentException>(rounding >= TimeSpan.Zero, nameof(rounding));

			return new TimeSpan(rounding.Ticks - (dateTime.Ticks % rounding.Ticks));
		}
	}
}
