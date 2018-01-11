using System;
using System.Text.RegularExpressions;

namespace Havit.Text.RegularExpressions
{
	/// <summary>
	/// Typické vyhledávací vzory pro regulární výrazy.
	/// </summary>
	public static class RegexPatterns
	{
		#region EmailStrict
		/// <summary>
		/// Pattern pro kontrolu běžného e-mailu:
		/// <list type="bullet">
		///		<item>povoleny jsou pouze znaky anglické abecedy, tečky, podtržítka, pomlčky a plus</item>
		///		<item>dva různé symboly nesmí následovat po sobě, stejné (s výjimkou tečky) mohou [test--test@test.com] projde, [test..test@test.com] neprojde</item>
		///		<item>nesmí začínat symbolem</item>
		///		<item>TLD musí mít 2-20 znaků (.travelersinsurance, .northwesternmutual https://en.wikipedia.org/wiki/List_of_Internet_top-level_domains) </item>
		///		<item>v doméně smí být tečky a pomlčky, ale nesmí následovat</item>
		///		<item>nepříjímá IP adresy</item>
		///		<item>nepřijímá rozšířený syntax typu [Petr Novak &lt;novak@test.com&gt;]</item>
		/// </list>
		/// </summary>
		/// <remarks>
		/// http://www.regexlib.com/REDetails.aspx?regexp_id=295
		/// </remarks>
		public const string EmailStrict = @"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.)|([A-Za-z0-9]+\++)|([A-Za-z0-9]+'+))*[A-Za-z0-9]+"
										+ @"@(([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.))*[A-Za-z0-9]{1,63}\.[a-zA-Z]{2,20}$";
		#endregion
		
		#region Identifier
		/// <summary>
		/// Pattern pro kontrolu identifikátorů.
		/// Identifikátor musí začínat písmenem nebo podtržítkem, nesledovat mohou i číslice.
		/// </summary>
		public const string Identifier = @"^[a-zA-Z_]{1}[a-zA-Z0-9_]+$";
		#endregion

		#region Time24h
		/// <summary>
		/// Pattern pro kontrolu času. 24-hodinnový formát, odělovač dvojtečka, nepovinné vteřiny. Např. 23:59:00.
		/// Nepřijímá 24:00.
		/// </summary>
		public const string Time24h = @"^(20|21|22|23|[01]\d|\d)(([:][0-5]\d){1,2})$";
		#endregion

		#region IPAddress
		/// <summary>
		/// Pattern pro kontrolu IP adresy v4.
		/// </summary>
		public const string IPAddress = @"^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\."
										+ @"(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\."
										+ @"(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\."
										+ @"(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$";
		#endregion

		#region Integer
		/// <summary>
		/// Pattern pro ověření celých čísel.
		/// </summary>
		/// <remarks>
		/// Přijímá: [1], [+15], [0], [-10], [+0]<br/>
		/// Odmítá: [1.0], [abc], [+], [1,15]
		/// </remarks>
		public const string Integer = @"^[-+]?\d+$";
		#endregion

		#region GetWildcardRegex, IsWildcardMatch, GetWildcardRegexPattern (private)
		/// <summary>
		/// Vrátí regulární výraz pro hledání v textu.
		/// Více o myšlence wildcardů je uvedeno u metody TextCondition.CreateWildcards.
		/// </summary>
		/// <param name="wildcardExpression">Text, který má být hledán a pro který se tvoří regulární výraz.</param>
		public static Regex GetWildcardRegex(string wildcardExpression)
		{
			string regexPattern = GetWildcardRegexPattern(wildcardExpression);
			return new Regex(regexPattern, RegexOptions.IgnoreCase);
		} 

		/// <summary>
		/// Vrátí true, pokud textToBeSearched obsahuje hledaný vzorek wildcardExpressionToSearch (s logikou wildcards - uvedena u metody CreateWildcards).
		/// </summary>
		/// <param name="wildcardExpressionToSearch">Vzorek, který je vyhledáván.</param>
		/// <param name="textToBeSearched">Text, který je prohledáván.</param>
		public static bool IsWildcardMatch(string wildcardExpressionToSearch, string textToBeSearched)
		{
			string regexPattern = GetWildcardRegexPattern(wildcardExpressionToSearch);
			return Regex.IsMatch(textToBeSearched, regexPattern, RegexOptions.IgnoreCase);
		}

		/// <summary>
		/// Vrátí pattern pro regulární výraz na základě výrazu.
		/// </summary>
		/// <param name="wildcardExpression">Výraz (text), pro který se vytváří regulární výraz.</param>
		private static string GetWildcardRegexPattern(string wildcardExpression)
		{
			string regexPattern = wildcardExpression;
			regexPattern = regexPattern.Replace("\\", "\\\\"); // zdvojíme zpětná lomítka
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

			// hvězdička je pro nás zvláštní symbol
			regexPattern = regexPattern.Replace("*", "((.|\n)*)");
			// hledáme od začátku
			regexPattern = "^" + regexPattern;
			// pokud je hvězdička, pak hledáme "přesnou" shodu
			// pokud hvezdička není, chceme, aby se hledání chovalo, jako by byla hvězdička na konci, slovy regulárních výrazů pak netřeba $ na konci.
			if (wildcardExpression.Contains("*"))
			{
				regexPattern += "$";
			}

			return regexPattern;
		}
		#endregion

	}
}
