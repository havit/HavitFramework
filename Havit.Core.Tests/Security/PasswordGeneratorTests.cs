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

		Assert.IsGreaterThanOrEqualTo(minimumLength, actual.Length);
		Assert.IsLessThanOrEqualTo(maximumLength, actual.Length);
	}

	[TestMethod]
	public void PasswordGenerator_Generate_ThrowsExceptionForLonPasswords()
	{
		// Arrange
		int minimumLength = 100;
		int maximumLength = 100;
		PasswordCharacterSet passwordCharacterSet = PasswordCharacterSet.LowerCaseLetters;
		bool allowRepeatingCharacters = false;
		bool allowConsecutiveCharacters = false;

		// Assert
		Assert.ThrowsExactly<InvalidOperationException>(() =>
		{
			// Act
			_ = Havit.Security.PasswordGenerator.Generate(minimumLength, maximumLength, passwordCharacterSet, allowRepeatingCharacters, allowConsecutiveCharacters);
		});
	}

}
