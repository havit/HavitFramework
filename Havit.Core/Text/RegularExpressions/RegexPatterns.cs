using System;
using System.Text.RegularExpressions;

namespace Havit.Text.RegularExpressions;

/// <summary>
/// Typical search patterns for regular expressions.
/// </summary>
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
	public const string Identifier = @"^[a-zA-Z_]{1}[a-zA-Z0-9_]+$";

	/// <summary>
	/// Pattern for checking time. 24-hour format, colon separator, optional seconds. For example, 23:59:00.
	/// Does not accept 24:00.
	/// </summary>
	public const string Time24h = @"^(20|21|22|23|[01]\d|\d)(([:][0-5]\d){1,2})$";

	/// <summary>
	/// Pattern for checking IPv4 addresses.
	/// </summary>
	public const string IPAddress = @"^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\."
									+ @"(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\."
									+ @"(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\."
									+ @"(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$";

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
		string regexPattern = wildcardExpression;
		regexPattern = regexPattern.Replace("\\", "\\\\"); // double backslashes
		regexPattern = regexPattern.Replace("^", "\\^");
		regexPattern = regexPattern.Replace("$", "\\$");
		regexPattern = regexPattern.Replace("+", "\\+");
		regexPattern = regexPattern.Replace(".", "\\.");
		regexPattern = regexPattern.Replace("(", "\\(");
		regexPattern = regexPattern.Replace(")", "\\)");
		regexPattern = regexPattern.Replace("|", "\\|");
		regexPattern = regexPattern.Replace("{", "\\{");
		regexPattern = regexPattern.Replace("}", "\\}");
		regexPattern = regexPattern.Replace("[", "\\[");
		regexPattern = regexPattern.Replace("]", "\\]");
		regexPattern = regexPattern.Replace("?", "\\?");

		// asterisk is a special symbol for us
		regexPattern = regexPattern.Replace("*", "((.|\n)*)");
		// searching from the beginning
		regexPattern = "^" + regexPattern;
		// if there is an asterisk, we want an "exact" match
		// if there is no asterisk, we want the search to behave as if there was an asterisk at the end, in the words of regular expressions, no need for $ at the end.
		if (wildcardExpression.Contains("*"))
		{
			regexPattern += "$";
		}

		return regexPattern;
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
