using System.Text;
using System.Text.RegularExpressions;

namespace Havit.Text.RegularExpressions;

/// <summary>
/// Typical search patterns for regular expressions.
/// </summary>
/// <remarks>
/// The validation patterns are intentionally anchored by <c>^</c> and <c>$</c> only, so that they are usable by the
/// JavaScript (ECMAScript) regular expression engine as well - they can therefore be handed over to client-side
/// validation (RegularExpressionAttribute, RegularExpressionValidator, HTML pattern attribute, ...) without any
/// translation. Do not replace <c>$</c> with <c>\z</c>: <c>\z</c> is a .NET-only construct that JavaScript rejects.
/// <para>
/// Note that in .NET <c>$</c> also matches right before a single trailing <c>\n</c>, so e.g. "123\n" passes
/// as a valid integer (JavaScript is stricter here and rejects it). Anything beyond that trailing <c>\n</c>
/// is rejected by both engines ("123\r\n", "123\n\n", "123\nabc"). Trim the input if you need strict behavior.
/// </para>
/// Do not combine the patterns with <see cref="RegexOptions.Multiline"/>: it would turn <c>^</c> and <c>$</c> into
/// per-line anchors and the patterns would start matching individual lines of a multi-line input.
/// </remarks>
public static class RegexPatterns
{
	/// <summary>
	/// Pattern for checking a common email:
	/// <list type="bullet">
	///     <item>only characters of the English alphabet, dots, underscores, dashes, and plus signs are allowed</item>
	///     <item>two different symbols cannot follow each other, the same (except for a dot) can [test--test@test.com] passes, [test..test@test.com] does not pass</item>
	///     <item>cannot start with a symbol</item>
	///     <item>TLD must have 2-20 characters (.travelersinsurance, .northwesternmutual https://en.wikipedia.org/wiki/List_of_Internet_top-level_domains)</item>
	///     <item>dots and dashes are allowed in the domain, but cannot follow each other</item>
	///     <item>does not accept IP addresses</item>
	///     <item>does not accept extended syntax like [Petr Novak &lt;novak@test.com&gt;]</item>
	/// </list>
	/// </summary>
	/// <remarks>
	/// http://www.regexlib.com/REDetails.aspx?regexp_id=295
	/// </remarks>
	public const string EmailStrict = @"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.)|([A-Za-z0-9]+\++)|([A-Za-z0-9]+'+))*[A-Za-z0-9]+"
									+ @"@(([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.))*[A-Za-z0-9]{1,63}\.[a-zA-Z]{2,20}$";

	/// <summary>
	/// Pattern for checking identifiers.
	/// The identifier must start with a letter or underscore, followed by letters, digits, or underscores.
	/// </summary>
	public const string Identifier = @"^[a-zA-Z_][a-zA-Z0-9_]*$";

	/// <summary>
	/// Pattern for checking time. 24-hour format, colon separator, optional seconds. For example, 23:59:00.
	/// Does not accept 24:00.
	/// </summary>
	public const string Time24h = @"^(20|21|22|23|[01]\d|\d)(([:][0-5]\d){1,2})$";

	/// <summary>
	/// Pattern for checking IPv4 addresses.
	/// The first octet must be 1-255, the remaining octets 0-255. Octets with leading zeros (e.g. 001) are not accepted.
	/// </summary>
	public const string IPAddress = @"^(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[1-9])\."
									+ @"(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\."
									+ @"(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\."
									+ @"(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])$";

	/// <summary>
	/// Pattern for checking integers.
	/// </summary>
	/// <remarks>
	/// Accepts: [1], [+15], [0], [-10], [+0]<br/>
	/// Rejects: [1.0], [abc], [+], [1,15]
	/// </remarks>
	public const string Integer = @"^[-+]?\d+$";

	/// <summary>
	/// Returns a regular expression for searching in text.
	/// More about the wildcard idea is described in the TextCondition.CreateWildcards method.
	/// </summary>
	/// <param name="wildcardExpression">The text to be searched and for which the regular expression is created.</param>
	public static Regex GetWildcardRegex(string wildcardExpression)
	{
		string regexPattern = GetWildcardRegexPattern(wildcardExpression);
		return new Regex(regexPattern, RegexOptions.IgnoreCase);
	}

	/// <summary>
	/// Returns true if textToBeSearched contains the searched wildcardExpressionToSearch pattern (with wildcard logic - described in the CreateWildcards method).
	/// </summary>
	/// <param name="wildcardExpressionToSearch">The pattern being searched.</param>
	/// <param name="textToBeSearched">The text being searched.</param>
	public static bool IsWildcardMatch(string wildcardExpressionToSearch, string textToBeSearched)
	{
		string regexPattern = GetWildcardRegexPattern(wildcardExpressionToSearch);
		return Regex.IsMatch(textToBeSearched, regexPattern, RegexOptions.IgnoreCase);
	}

	/// <summary>
	/// Returns a pattern for a regular expression based on the expression.
	/// </summary>
	/// <param name="wildcardExpression">The expression (text) for which the regular expression is created.</param>
	private static string GetWildcardRegexPattern(string wildcardExpression)
	{
		// Single pass over the input (instead of ~14 chained string.Replace calls, each allocating an intermediate string).
		StringBuilder regexPattern = new StringBuilder(wildcardExpression.Length + 2);
		regexPattern.Append('^'); // searching from the beginning

		bool containsAsterisk = false;
		foreach (char c in wildcardExpression)
		{
			switch (c)
			{
				case '\\':
				case '^':
				case '$':
				case '+':
				case '.':
				case '(':
				case ')':
				case '|':
				case '{':
				case '}':
				case '[':
				case ']':
				case '?':
					regexPattern.Append('\\').Append(c);
					break;

				case '*':
					// asterisk is a special symbol for us
					regexPattern.Append("((.|\n)*)");
					containsAsterisk = true;
					break;

				default:
					regexPattern.Append(c);
					break;
			}
		}

		// if there is an asterisk, we want an "exact" match
		// if there is no asterisk, we want the search to behave as if there was an asterisk at the end, in the words of regular expressions, no need for $ at the end.
		if (containsAsterisk)
		{
			regexPattern.Append('$');
		}

		return regexPattern.ToString();
	}

	/// <summary>
	/// Returns whether the file name matches the file mask - supports '*' and '?' characters. Used in CMD.
	/// </summary>
	public static bool IsFileWildcardMatch(String fileName, String searchPattern)
	{
		// https://stackoverflow.com/questions/30299671/matching-strings-with-wildcard
		string regular = "^" + Regex.Escape(searchPattern).Replace("\\?", ".").Replace("\\*", ".*") + "$";
		return Regex.IsMatch(fileName, regular);
	}
}
