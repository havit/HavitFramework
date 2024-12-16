using System.Text.RegularExpressions;

namespace Havit.Services.StateAdministration;

/// <summary>
/// Třída pro práci s rodným číslem.
/// </summary>
public static class RodneCisloServices
{
	/// <summary>
	/// Zkontroluje formát rodného čísla. Vrací true, pokud jde o platné RČ a EČP.
	/// Akceptuje verzi s lomítkem i bez něj.
	/// Používá algoritmus popsaný na https://www.cssz.cz/standardni-kontrola-rodneho-cisla-a-evidencniho-cisla-pojistence,
	/// přičemž dělitelnost RČ/EČP je podle https://www.kurzy.cz/vypocet/rodne-cislo-validace/.
	/// </summary>
	public static bool Validate(string rodneCislo)
	{
		if (string.IsNullOrWhiteSpace(rodneCislo))
		{
			return false;
		}

		Match match = Regex.Match(rodneCislo, @"^\s*(?<year>\d\d)(?<month>\d\d)(?<day>\d\d)[ /]?(?<ext>\d\d\d)(?<checksum>\d?)\s*$");

		if (!match.Success)
		{
			return false;
		}

		string checksumString = match.Groups["checksum"].Value;
		string extString = match.Groups["ext"].Value;

		int year = Convert.ToInt32(match.Groups["year"].Value);
		int month = Convert.ToInt32(match.Groups["month"].Value);
		int day = Convert.ToInt32(match.Groups["day"].Value);
		int ext = Convert.ToInt32(extString);
		bool isRc = false;
		bool isEcp = false;


		if (string.IsNullOrEmpty(checksumString) && extString == "000")
		{
			return false;
		}

		if (!IsDivisibleByEleven(year, month, day, ext, checksumString))
		{
			return false;
		}

		// k měsíci může být připočteno 20, 50 nebo 70
		if (month > 50)
		{
			month -= 50;
		}

		if (month > 20)
		{
			isRc = true;
			month -= 20;
		}

		// V případě EČP je vždy ke dnu připočteno 40
		if (day > 40)
		{
			isEcp = true;
			day -= 40;
		}

		if (isEcp && isRc)
		{
			return false;
		}

		if (string.IsNullOrEmpty(checksumString))
		{
			year += (year > 53) ? 1800 : 1900;
		}
		else
		{
			year += (year > 53) ? 1900 : 2000;
		}

		if (!IsCorrectBirthDate(year, month, day))
		{
			return false;
		}

		if (isEcp && !IsCorrectEcp(checksumString, extString, ext))
		{
			return false;
		}

		return true;
	}

	private static bool IsCorrectEcp(string checksumString, string extString, int ext)
	{
		if (string.IsNullOrEmpty(checksumString) && ext < 600)
		{
			return false;
		}

		if (!string.IsNullOrEmpty(checksumString))
		{
			string extendedExtString = extString + checksumString;
			int extendedExt = Convert.ToInt32(extendedExtString);
			if (extendedExt < 6000)
			{
				return false;
			}
		}

		return true;
	}

	private static bool IsDivisibleByEleven(int year, int month, int day, int ext, string checksumString)
	{
		if (string.IsNullOrEmpty(checksumString))
		{
			// Pro devítimístná RČ/EČP nelze validovat.
			return true;
		}

		int checksum = Convert.ToInt32(checksumString);
		int mod = ((year * 10000000) + (month * 100000) + (day * 1000) + ext) % 11;
		if (mod == 10)
		{
			mod = 0;
		}

		return (mod == checksum);
	}

	private static bool IsCorrectBirthDate(int year, int month, int day)
	{
		DateTime birthDate;
		try
		{
			birthDate = new DateTime(year, month, day);
		}
		catch (ArgumentOutOfRangeException)
		{
			return false;
		}

		return true;
	}
}
