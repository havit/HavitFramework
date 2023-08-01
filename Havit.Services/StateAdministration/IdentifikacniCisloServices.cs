using System.Text.RegularExpressions;

namespace Havit.Services.StateAdministration;

/// <summary>
/// Třída pro práci s identifikačním číslem (IČ, dříve IČO).
/// </summary>
public static class IdentifikacniCisloServices
{
	/// <summary>
	/// Zkontroluje formát identifikačního čísla. Vrací true, pokud jde o platné IČ.
	/// Používá algoritmus popsaný na http://phpfashion.com/jak-overit-platne-ic-a-rodne-cislo
	/// </summary>
	/// <remarks>
	/// Jak se ověřuje IČ? Například 69663963.
	///
	/// První až sedmou číslici vynásobíme čísly 8, 7, 6, 5, 4, 3, 2 a součiny sečteme:
	/// 
	/// soucet = 6*8 + 9*7 + 6*6 + 6*5 + 3*4 + 9*3 + 6*2 = 228
	/// spočítáme zbytek po dělení jedenácti:  zbytek = soucet % 11
	/// 
	/// pro poslední osmou číslici c musí platit:
	/// je-li zbytek 0 nebo 10, pak c = 1
	/// je-li zbytek 1, pak c = 0
	/// v ostatních případech je c = 11 - zbytek
	/// </remarks>
	public static bool Validate(string identifikacniCislo)
	{
		if (!Regex.IsMatch(identifikacniCislo, @"^\d{3,8}$"))
		{
			return false;
		}

		identifikacniCislo = identifikacniCislo.PadLeft(8, '0');

		int soucet = 0;
		// Číselnou hodnotu 1 až 7 pozice vynásobíme číslem (8 - index) a součiny sečteme.
		for (int i = 0; i < 7; i++)
		{
			soucet += int.Parse(identifikacniCislo[i].ToString()) * (8 - i);
		}

		int zbytek = soucet % 11;
		int posledniCislice = int.Parse(identifikacniCislo[7].ToString());

		return (((zbytek == 0) || (zbytek == 10)) && (posledniCislice == 1))
			|| ((zbytek == 1) && (posledniCislice == 0))
			|| ((zbytek > 1) && (zbytek < 10) && ((11 - zbytek) == posledniCislice));
	}
}
