using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using Havit.Diagnostics.Contracts;

namespace Havit;

/// <summary>
/// Extension methods for working with <see cref="System.String"/>.
/// Provides static methods and constants, it is non-instantiable.
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
		Contract.Requires<ArgumentOutOfRangeException>(length >= 0, "Argument length must not be less than 0.");

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
		Contract.Requires<ArgumentOutOfRangeException>(length >= 0, "Argument length must not be less than 0.");

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
	/// Removes diacritics from the text, i.e. converts it to text without diacritics.
	/// </summary>
	/// <remarks>Removes all diacritics from all national characters in general.</remarks>
	/// <param name="text">The text from which diacritics should be removed.</param>
	/// <returns>text without diacritics</returns>
	public static string RemoveDiacritics(this string text)
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
	/// Removes diacritics from the text, i.e. converts it to text without diacritics.
	/// </summary>
	/// <remarks>Removes all diacritics from all national characters in general.</remarks>
	/// <param name="text">The text from which diacritics should be removed.</param>
	/// <returns>text without diacritics</returns>
	// TODO [Obsolete("Use RemoveDiacritics instead.")]
	public static string OdeberDiakritiku(string text) => RemoveDiacritics(text);

	/// <summary>
	/// Returns the char representation (0..9, A..F) of a hexadecimal digit (0-15).
	/// </summary>
	/// <remarks>Due to speed, it does not perform range checking and converts, for example, the digit 16 as G.</remarks>
	/// <param name="digit">Digit (0..15)</param>
	/// <returns>char representation (0..9, A..F) of a hexadecimal digit (0-15).</returns>
	public static char IntToHex(int digit)
	{
		if (digit <= 9)
		{
			return (char)((ushort)(digit + 0x30));
		}
		return (char)((ushort)((digit - 10) + 0x61));
	}

	/// <summary>
	/// Normalizes a text string for use in a URL address (for SEO).
	/// 1) Converts to lowercase.
	/// 2) Removes diacritics.
	/// 3) replaces everything except letters and numbers with a hyphen (including whitespace).
	/// 4) then merges multiple hyphens into one.
	/// 5) removes any hyphens at the beginning and end of the string.
	/// </summary>
	/// <param name="text">input text</param>
	/// <returns>normalized text for URL (SEO)</returns>
	public static string NormalizeForUrl(this string text)
	{
		text = text.ToLower();
		text = StringExt.RemoveDiacritics(text);
		text = Regex.Replace(text, "[^A-Za-z0-9]", "-");
		text = Regex.Replace(text, @"-{2,}", "-");
		text = text.Trim('-');

		return text;
	}
}
