using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using Havit.Security;

namespace Havit.Tests.Security;

[TestClass]
public class PasswordGeneratorTests
{
	[TestMethod]
	public void PasswordGenerator_Generate_HasLengthBetweenMinimumAndMaximum()
	{
		int minimumLength = 10;
		int maximumLength = 20;
		PasswordCharacterSet passwordCharacterSet = PasswordCharacterSet.LowerCaseLetters;
		bool allowRepeatingCharacters = false;
		bool allowConsecutiveCharacters = false;

		string actual;

		actual = Havit.Security.PasswordGenerator.Generate(minimumLength, maximumLength, passwordCharacterSet, allowRepeatingCharacters, allowConsecutiveCharacters);

		Assert.IsTrue(actual.Length >= minimumLength);
		Assert.IsTrue(actual.Length <= maximumLength);
	}

	[TestMethod]
	[ExpectedException(typeof(InvalidOperationException))]
	public void PasswordGenerator_Generate_ThrowsExceptionForLonPasswords()
	{
		int minimumLength = 100;
		int maximumLength = 100;
		PasswordCharacterSet passwordCharacterSet = PasswordCharacterSet.LowerCaseLetters;
		bool allowRepeatingCharacters = false;
		bool allowConsecutiveCharacters = false;

		string actual;

		actual = Havit.Security.PasswordGenerator.Generate(minimumLength, maximumLength, passwordCharacterSet, allowRepeatingCharacters, allowConsecutiveCharacters);
	}

}
