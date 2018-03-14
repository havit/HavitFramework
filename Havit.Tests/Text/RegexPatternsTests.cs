using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
using Havit.Text.RegularExpressions;

namespace Havit.Tests.Text
{
	/// <summary>
	/// Test RegexPatterns.
	/// </summary>
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

	}
}
