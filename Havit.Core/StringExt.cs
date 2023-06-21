using System;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using Havit.Diagnostics.Contracts;

namespace Havit;

/// <summary>
/// Rozšiřující funkce pro práci s textovými řetězci <see cref="System.String"/>.
/// Třída poskytuje statické metody a konstanty, je neinstanční.
/// </summary>
public static class StringExt
{
	/// <summary>
	/// Returns a string containing a specified number of characters from the left side of a string.
	/// </summary>
	/// <param name="str">String expression from which the leftmost characters are returned.</param>
	/// <param name="length">Numeric expression indicating how many characters to return. If 0, a zero-length string ("") is returned. If greater than or equal to the number of characters in Str, the entire string is returned.</param>
	/// <returns>string containing a specified number of characters from the left side of a string</returns>
	public static string Left(this string str, int length)
	{
		Contract.Requires<ArgumentOutOfRangeException>(length >= 0, "Argument length nesmí být menší než 0.");

		if ((length == 0) || (str == null))
		{
			return String.Empty;
		}
		if (length >= str.Length)
		{
			return str;
		}
		return str.Substring(0, length);
	}

	/// <summary>
	/// Returns a string containing a specified number of characters from the right side of a string.
	/// </summary>
	/// <param name="str">String expression from which the rightmost characters are returned.</param>
	/// <param name="length">Numeric expression indicating how many characters to return. If 0, a zero-length string ("") is returned. If greater than or equal to the number of characters in <c>str</c>, the entire string is returned.</param>
	/// <returns>string containing a specified number of characters from the right side of a string</returns>
	public static string Right(this string str, int length)
	{
		Contract.Requires<ArgumentOutOfRangeException>(length >= 0, "Argument length nesmí být menší než 0.");

		if ((length == 0) || (str == null))
		{
			return String.Empty;
		}
		int strLength = str.Length;
		if (length >= strLength)
		{
			return str;
		}
		return str.Substring(strLength - length, length);
	}

	/// <summary>
	/// Odebere diakritiku z textu, tj. převede na text bez diakritiky.
	/// </summary>
	/// <remarks>Odebírá veškerou diakritiku všech národních znaků obecně.</remarks>
	/// <param name="text">Text, kterému má být diakritika odebrána.</param>
	/// <returns>text bez diakritiky</returns>
	public static string OdeberDiakritiku(string text)
	{
		StringBuilder sb = new StringBuilder();

		text = text.Normalize(NormalizationForm.FormD);

		for (int i = 0; i < text.Length; i++)
		{
			if (CharUnicodeInfo.GetUnicodeCategory(text[i]) != UnicodeCategory.NonSpacingMark)
			{
				sb.Append(text[i]);
			}
		}

		return sb.ToString();
	}

	/// <summary>
	/// Vrátí char-reprezentaci (0..9, A..F) šestnáctkové číslice (0-15).
	/// </summary>
	/// <remarks>Z důvodu rychlosti neprovádí kontrolu rozsahu a převede např. i číslici 16 jako G.</remarks>
	/// <param name="cislice">Číslice (0..15)</param>
	/// <returns>char-reprezentace (0..9, A..F) šestnáctkové číslice (0-15).</returns>
	public static char IntToHex(int cislice)
	{
		if (cislice <= 9)
		{
			return (char)((ushort)(cislice + 0x30));
		}
		return (char)((ushort)((cislice - 10) + 0x61));
	}

	/// <summary>
	/// Normalizuje textový řetězec do podoby použitelné v URL adrese (pro SEO).
	/// 1) Převede na malá písmena.
	/// 2) Odebere diakritiku.
	/// 3) vše mimo písmen a číslic nahradí za pomlčku (včetně whitespace). 
	/// 4) potom vícenásobné pomlčky sloučí v jednu. 
	/// 5) odebere případné pomčky na začátku a konci řetězce. 
	/// </summary>
	/// <param name="text">vstupní text</param>
	/// <returns>normalizovaný text pro URL (SEO)</returns>
	public static string NormalizeForUrl(string text)
	{
		text = text.ToLower();
		text = StringExt.OdeberDiakritiku(text);
		text = Regex.Replace(text, "[^A-Za-z0-9]", "-");
		text = Regex.Replace(text, @"-{2,}", "-");
		text = text.Trim('-');

		return text;
	} 
}
