using Havit.Security;

namespace Havit.Core.Tests.Security;

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
	// dříve se generátor pro tento případ zacyklil v nekonečné smyčce; potřebujeme tvrdý (nekooperativní) timeout,
	// kooperativní by smyčku ignorující cancellation token nepřerušil
#pragma warning disable MSTEST0045
	[Timeout(30000)]
#pragma warning restore MSTEST0045
	public void PasswordGenerator_Generate_NoRepeats_LengthEqualToSetSizeYieldsWholeSet()
	{
		// Arrange
		// LowerCaseLetters = 26 znaků (a-z); délka rovná velikosti sady bez opakování => heslo musí obsahovat všechny znaky sady,
		// tedy i poslední znak 'z' (off-by-one ho dříve nikdy nevygeneroval) a zároveň se nesmí zacyklit.
		const int setSize = 26;

		// Act
		string actual = PasswordGenerator.Generate(setSize, setSize, PasswordCharacterSet.LowerCaseLetters, allowRepeatingCharacters: false, allowConsecutiveCharacters: true);

		// Assert
		Assert.AreEqual(setSize, actual.Length);
		Assert.HasCount(setSize, actual.Distinct(), "Heslo neobsahuje všechny znaky sady.");
		Assert.Contains('z', actual, "Poslední znak sady ('z') nebyl vygenerován.");
		Assert.Contains('a', actual);
	}

	[TestMethod]
	public void PasswordGenerator_Generate_LastCharacterOfSetIsReachable()
	{
		// Arrange + Act
		// posbíráme znaky z mnoha vygenerovaných hesel a ověříme, že se objeví i poslední znak sady ('9' pro LettersAndDigits)
		HashSet<char> generatedCharacters = new HashSet<char>();
		for (int i = 0; i < 200; i++)
		{
			generatedCharacters.UnionWith(PasswordGenerator.Generate(20, 20, PasswordCharacterSet.LettersAndDigits));
		}

		// Assert
		Assert.Contains('9', generatedCharacters, "Poslední znak sady LettersAndDigits ('9') nebyl mezi vygenerovanými znaky.");
	}

	[TestMethod]
	public void PasswordGenerator_Generate_MaximumLengthIsReachable()
	{
		// Arrange
		const int minimumLength = 5;
		const int maximumLength = 8;

		int observedMin = int.MaxValue;
		int observedMax = int.MinValue;

		// Act
		for (int i = 0; i < 500; i++)
		{
			int length = PasswordGenerator.Generate(minimumLength, maximumLength, PasswordCharacterSet.LettersAndDigits).Length;
			observedMin = Math.Min(observedMin, length);
			observedMax = Math.Max(observedMax, length);
		}

		// Assert
		Assert.IsGreaterThanOrEqualTo(minimumLength, observedMin, "Vygenerovaná délka klesla pod MinimumLength.");
		Assert.IsLessThanOrEqualTo(maximumLength, observedMax, "Vygenerovaná délka překročila MaximumLength.");
		Assert.AreEqual(maximumLength, observedMax, "MaximumLength se jako délka hesla nikdy nevygeneroval.");
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
