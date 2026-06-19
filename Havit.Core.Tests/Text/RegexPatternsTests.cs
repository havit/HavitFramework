using System.Text.RegularExpressions;
using Havit.Text.RegularExpressions;

namespace Havit.Core.Tests.Text;

[TestClass]
public class RegexPatternsTests
{
	[TestMethod]
	public void RegexPatterns_EmailStrict()
	{
		Assert.IsTrue(IsEmailAddressValid("kanda@havit.cz"), "kanda@havit.cz");
		Assert.IsTrue(IsEmailAddressValid("kanda@i.cz"), "kanda@i.cz");
		Assert.IsTrue(IsEmailAddressValid("123@i.cz"), "123@i.cz");
		Assert.IsTrue(IsEmailAddressValid("k.a.n.d.a@h.a.v.i.t.cz"), "k.a.n.d.a@h.a.v.i.t.cz");
		Assert.IsTrue(IsEmailAddressValid("a@b.cz"), "a@b.cz");
		Assert.IsTrue(IsEmailAddressValid("a@b.info"), "a@b.info");
		Assert.IsTrue(IsEmailAddressValid("o'realy@havit.cz"), "o'realy@havit.cz");
		Assert.IsTrue(IsEmailAddressValid("james@007.uk"), "james@007.uk");
		Assert.IsTrue(IsEmailAddressValid("007@007.uk"), "007@007.uk");
		Assert.IsTrue(IsEmailAddressValid("0-0-7@0-0-7.uk"), "0-0-7@0-0-7.uk");
		Assert.IsTrue(IsEmailAddressValid("0+0+7@0-0-7.uk"), "0+0+7@0-0-7.uk");

		Assert.IsFalse(IsEmailAddressValid("@havit.cz"), "@havit.cz");
		Assert.IsFalse(IsEmailAddressValid("kanda@"), "kanda@");
		Assert.IsFalse(IsEmailAddressValid("kanda@havit"), "kanda@havit");
		Assert.IsFalse(IsEmailAddressValid("kanda@havit..cz"), "kanda@havit..cz");
		Assert.IsFalse(IsEmailAddressValid("kanda@ha..vit.cz"), "kanda@ha..vit.cz");
		Assert.IsFalse(IsEmailAddressValid("k..anda@havit.cz"), "k..anda@havit.cz");

		// IDN není podporováno v .NETu
		Assert.IsFalse(IsEmailAddressValid("jiří@kanda.eu"), "jiří@kanda.eu");
		Assert.IsFalse(IsEmailAddressValid("email@jiříkanda.eu"), "email@jiříkanda.eu");
		Assert.IsFalse(IsEmailAddressValid("můjmail@jiříkanda.eu"), "můjmail@jiříkanda.eu");
		Assert.IsFalse(IsEmailAddressValid("můjmail@jiříkanda.eu"), "můjmail@jiříkanda.eu");
		Assert.IsFalse(IsEmailAddressValid("můjmail@jiříkanda.eu"), "můjmail@jiříkanda.eu");
	}

	private bool IsEmailAddressValid(string emailAddress)
	{
		return Regex.IsMatch(emailAddress, Havit.Text.RegularExpressions.RegexPatterns.EmailStrict);
	}

	[TestMethod]
	public void RegexPatterns_Identifier()
	{
		// platné identifikátory
		Assert.IsTrue(IsIdentifierValid("identifier"), "identifier");
		Assert.IsTrue(IsIdentifierValid("_identifier"), "_identifier");
		Assert.IsTrue(IsIdentifierValid("identifier123"), "identifier123");
		Assert.IsTrue(IsIdentifierValid("Identifier_123"), "Identifier_123");
		Assert.IsTrue(IsIdentifierValid("a"), "a"); // jednoznakový identifikátor
		Assert.IsTrue(IsIdentifierValid("_"), "_");

		// neplatné identifikátory
		Assert.IsFalse(IsIdentifierValid(""), "(empty)");
		Assert.IsFalse(IsIdentifierValid("1identifier"), "1identifier");
		Assert.IsFalse(IsIdentifierValid("identi fier"), "identi fier");
		Assert.IsFalse(IsIdentifierValid("identi-fier"), "identi-fier");
	}

	private bool IsIdentifierValid(string identifier)
	{
		return Regex.IsMatch(identifier, RegexPatterns.Identifier);
	}

	[TestMethod]
	public void RegexPatterns_IPAddress()
	{
		// platné adresy
		Assert.IsTrue(IsIPAddressValid("1.0.0.0"), "1.0.0.0");
		Assert.IsTrue(IsIPAddressValid("192.168.1.1"), "192.168.1.1");
		Assert.IsTrue(IsIPAddressValid("255.255.255.255"), "255.255.255.255");
		Assert.IsTrue(IsIPAddressValid("8.8.8.8"), "8.8.8.8");

		// neplatné adresy
		Assert.IsFalse(IsIPAddressValid("0.0.0.0"), "0.0.0.0"); // první oktet nesmí být 0
		Assert.IsFalse(IsIPAddressValid("256.1.1.1"), "256.1.1.1");
		Assert.IsFalse(IsIPAddressValid("1.2.3"), "1.2.3");
		Assert.IsFalse(IsIPAddressValid("1.2.3.4.5"), "1.2.3.4.5");

		// vedoucí nuly nejsou akceptovány
		Assert.IsFalse(IsIPAddressValid("001.010.000.011"), "001.010.000.011");
		Assert.IsFalse(IsIPAddressValid("192.168.001.1"), "192.168.001.1");
		Assert.IsFalse(IsIPAddressValid("01.2.3.4"), "01.2.3.4");
	}

	[TestMethod]
	public void RegexPatterns_Time24h()
	{
		Assert.IsTrue(IsMatch(RegexPatterns.Time24h, "23:59"), "23:59");
		Assert.IsTrue(IsMatch(RegexPatterns.Time24h, "23:59:00"), "23:59:00");
		Assert.IsTrue(IsMatch(RegexPatterns.Time24h, "0:00"), "0:00");

		Assert.IsFalse(IsMatch(RegexPatterns.Time24h, "24:00"), "24:00");
		Assert.IsFalse(IsMatch(RegexPatterns.Time24h, "23:60"), "23:60");
	}

	[TestMethod]
	public void RegexPatterns_PatternsRejectTrailingNewline()
	{
		// $ matchuje i před koncovým \n; \z nikoli - validační patterny musí koncový \n odmítnout
		Assert.IsFalse(IsMatch(RegexPatterns.Integer, "123\n"), "Integer 123\\n");
		Assert.IsFalse(IsMatch(RegexPatterns.Identifier, "abc\n"), "Identifier abc\\n");
		Assert.IsFalse(IsEmailAddressValid("a@b.cz\n"), "EmailStrict a@b.cz\\n");
		Assert.IsFalse(IsIPAddressValid("1.2.3.4\n"), "IPAddress 1.2.3.4\\n");
		Assert.IsFalse(IsMatch(RegexPatterns.Time24h, "23:59\n"), "Time24h 23:59\\n");
	}

	private bool IsIPAddressValid(string ipAddress)
	{
		return Regex.IsMatch(ipAddress, RegexPatterns.IPAddress);
	}

	private bool IsMatch(string pattern, string input)
	{
		return Regex.IsMatch(input, pattern);
	}

	[TestMethod]
	public void RegexPatterns_IsWildcardMatch()
	{
		Assert.IsTrue(RegexPatterns.IsWildcardMatch("kolo", "kolo"));
		Assert.IsTrue(RegexPatterns.IsWildcardMatch("kolo", "kolotoč"));
		Assert.IsFalse(RegexPatterns.IsWildcardMatch("kolo", "okolo"));

		Assert.IsTrue(RegexPatterns.IsWildcardMatch("k*o", "kolo"));
		Assert.IsTrue(RegexPatterns.IsWildcardMatch("k*o", "koulo"));
		Assert.IsTrue(RegexPatterns.IsWildcardMatch("k*lo", "kolo"));
		Assert.IsTrue(RegexPatterns.IsWildcardMatch("k*olo", "kolo"));
		Assert.IsFalse(RegexPatterns.IsWildcardMatch("k*o", "kolotoč"));
		Assert.IsFalse(RegexPatterns.IsWildcardMatch("k*o", "okolo"));

		Assert.IsTrue(RegexPatterns.IsWildcardMatch("k.lo", "k.lo"));
		Assert.IsTrue(RegexPatterns.IsWildcardMatch("k?lo", "k?lo"));
		Assert.IsFalse(RegexPatterns.IsWildcardMatch("k.lo", "kolo"));
		Assert.IsFalse(RegexPatterns.IsWildcardMatch("k?lo", "kolo"));

		Assert.IsTrue(RegexPatterns.IsWildcardMatch("*description*", "<p>descriptionX</p>"));
		Assert.IsFalse(RegexPatterns.IsWildcardMatch("description*", "<p>descriptionX</p>"));
		Assert.IsFalse(RegexPatterns.IsWildcardMatch("description*", "<p>descriptionX</p>"));
		Assert.IsTrue(RegexPatterns.IsWildcardMatch("*description*", "<p>descriptionX</p>"));
		// test výrazu na dalším řádku
		Assert.IsTrue(RegexPatterns.IsWildcardMatch("*description*", @"<p>
				<u>descriptionX</u>
			</p>"));

		// test režimu RegexOptions.Multiline
		Assert.IsFalse(RegexPatterns.IsWildcardMatch("description", @"<p>
descriptionX</u>
			</p>"));
	}

	[TestMethod]
	public void RegexPatterns_IsWildcardMatch_EscapesRegexSpecialCharacters()
	{
		// All regex-special characters (except '*', which is our wildcard) must be treated as literals.
		// Positive: a pattern matches itself literally (no asterisk -> anchored prefix match).
		// Negative: an input that would match only if the character were a regex metacharacter must NOT match.

		// '+' (quantifier)
		Assert.IsTrue(RegexPatterns.IsWildcardMatch("a+b", "a+b"), "a+b literal");
		Assert.IsFalse(RegexPatterns.IsWildcardMatch("a+b", "aaab"), "'+' must not act as a quantifier");

		// '(' ')' (group)
		Assert.IsTrue(RegexPatterns.IsWildcardMatch("(ab)", "(ab)"), "(ab) literal");
		Assert.IsFalse(RegexPatterns.IsWildcardMatch("(ab)", "ab"), "'(' ')' must not act as a group");

		// '[' ']' (character class)
		Assert.IsTrue(RegexPatterns.IsWildcardMatch("[ab]", "[ab]"), "[ab] literal");
		Assert.IsFalse(RegexPatterns.IsWildcardMatch("[ab]", "a"), "'[' ']' must not act as a character class");

		// '|' (alternation)
		Assert.IsTrue(RegexPatterns.IsWildcardMatch("a|b", "a|b"), "a|b literal");
		Assert.IsFalse(RegexPatterns.IsWildcardMatch("a|b", "b"), "'|' must not act as alternation");

		// '{' '}' (quantifier)
		Assert.IsTrue(RegexPatterns.IsWildcardMatch("a{2}", "a{2}"), "a{2} literal");
		Assert.IsFalse(RegexPatterns.IsWildcardMatch("a{2}", "aa"), "'{' '}' must not act as a quantifier");

		// '$' (end anchor)
		Assert.IsTrue(RegexPatterns.IsWildcardMatch("a$", "a$"), "a$ literal");
		Assert.IsFalse(RegexPatterns.IsWildcardMatch("a$", "a"), "'$' must not act as an end anchor");

		// '\' (escape) and '.' (any char)
		Assert.IsTrue(RegexPatterns.IsWildcardMatch(@"a\b", @"a\b"), @"a\b literal");
		Assert.IsTrue(RegexPatterns.IsWildcardMatch("a.b", "a.b"), "a.b literal");
		Assert.IsFalse(RegexPatterns.IsWildcardMatch("a.b", "axb"), "'.' must not match any character");
	}

}
