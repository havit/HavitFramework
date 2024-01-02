using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Security;

/// <summary>
/// Set of characters from which <see cref="PasswordGenerator"/> selects characters for password generation.
/// </summary>
public enum PasswordCharacterSet
{
	/// <summary>
	/// Lowercase letters only.
	/// </summary>
	LowerCaseLetters,

	/// <summary>
	/// Uppercase and lowercase letters.
	/// </summary>
	Letters,

	/// <summary>
	/// Letters (uppercase and lowercase) and digits.
	/// </summary>
	LettersAndDigits,

	/// <summary>
	/// Letters (uppercase and lowercase), digits, and special characters.
	/// </summary>
	LettersDigitsAndSpecialCharacters
}
