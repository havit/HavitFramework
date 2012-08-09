using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace HavitTest
{
	/// <summary>
	/// Summary description for RegexPatternsTest
	/// </summary>
	[TestClass()]	
	public class RegexPatternsTest
	{
		[TestMethod]
		public void EmailStrictTest()
		{
			Assert.IsTrue(IsEmailAddressValid("kanda@havit.cz"), "kanda@havit.cz");
			Assert.IsTrue(IsEmailAddressValid("kanda@i.cz"), "kanda@i.cz");
			Assert.IsTrue(IsEmailAddressValid("123@i.cz"), "123@i.cz");
			Assert.IsTrue(IsEmailAddressValid("k.a.n.d.a@h.a.v.i.t.cz"), "k.a.n.d.a@h.a.v.i.t.cz");
			Assert.IsTrue(IsEmailAddressValid("a@b.cz"), "a@b.cz");
			Assert.IsTrue(IsEmailAddressValid("a@b.info"), "a@b.info");
			Assert.IsTrue(IsEmailAddressValid("o'realy@havit.cz"), "o'realy@havit.cz");

			Assert.IsFalse(IsEmailAddressValid("@havit.cz"), "@havit.cz");
			Assert.IsFalse(IsEmailAddressValid("kanda@"), "kanda@");
			Assert.IsFalse(IsEmailAddressValid("kanda@havit"), "kanda@havit");
			Assert.IsFalse(IsEmailAddressValid("kanda@havit..cz"), "kanda@havit..cz");
			Assert.IsFalse(IsEmailAddressValid("kanda@ha..vit.cz"), "kanda@ha..vit.cz");
			Assert.IsFalse(IsEmailAddressValid("k..anda@havit.cz"), "k..anda@havit.cz");
		}

		private bool IsEmailAddressValid(string emailAddress)
		{
			return Regex.IsMatch(emailAddress, Havit.Text.RegularExpressions.RegexPatterns.EmailStrict);
		}
	}
}
