using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Havit.Services.StateAdministration
{
	/// <summary>
	/// Třída pro práci s rodným číslem.
	/// </summary>
	public static class RodneCisloServices
	{
		#region Validate
		/// <summary>
		/// Zkontroluje formát rodného čísla. Vrací true, pokud jde o platné RČ.
		/// Používá algoritmus popsaný na http://phpfashion.com/jak-overit-platne-ic-a-rodne-cislo
		/// </summary>
		/// <remarks>
		/// ...
		/// </remarks>
		public static bool Validate(string rodneCislo)
		{
			Match match = Regex.Match(rodneCislo, @"^\s*(?<year>\d\d)(?<month>\d\d)(?<day>\d\d)[ /]?(?<ext>\d\d\d)(?<checksum>\d?)\s*$");

			if (!match.Success)
			{
				return false;
			}

			int year = Convert.ToInt32(match.Groups["year"].Value);
			int month = Convert.ToInt32(match.Groups["month"].Value);
			int day = Convert.ToInt32(match.Groups["day"].Value);
			int ext = Convert.ToInt32(match.Groups["ext"].Value);

			// do roku 1954 přidělovaná devítimístná RČ nelze ověřit
			if (String.IsNullOrEmpty(match.Groups["checksum"].Value))
			{
				return (year < 54);
			}

			// kontrolní číslice
			int checksum = Convert.ToInt32(match.Groups["checksum"].Value);
			int mod = ((year * 10000000) + (month * 100000) + (day * 1000) + ext) % 11;
			if (mod == 10)
			{
				mod = 0;
			}

			if (mod != checksum)
			{
				return false;
			}

			year += (year < 54) ? 2000 : 1000;

			// k měsíci může být připočteno 20, 50 nebo 70
			if ((month > 70) && (year > 2003))
			{
				month -= 70;
			}

			if (month > 50)
			{
				month -= 50;
			}

			if ((month > 20) && (year > 2003))
			{
				month -= 20;
			}

			// vyhodnocení správnosti

			if ((month < 1) || (month > 12))
			{
				return false;
			}

			if ((day < 1) || (day > DateTime.DaysInMonth(year, month)))
			{
				return false;
			}

			return true;
		}
		#endregion
	}
}
