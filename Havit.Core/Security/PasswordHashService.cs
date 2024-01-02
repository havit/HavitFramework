using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Security;

/// <summary>
/// Helper methods for working with password hashes.
/// </summary>
public static class PasswordHashService
{
	/// <summary>
	/// Generates a random salt.
	/// Implemented using PasswordGenerator.
	/// </summary>
	/// <param name="saltLength">The desired length of the salt (default length is 8 characters).</param>
	/// <param name="passwordCharacterSet">The character set for selecting the salt (default value is PasswordCharacterSet.LettersAndDigits).</param>
	public static string CreateSalt(int saltLength = 8, PasswordCharacterSet passwordCharacterSet = PasswordCharacterSet.LettersAndDigits)
	{
		return PasswordGenerator.Generate(saltLength, saltLength, passwordCharacterSet, true, true);
	}

	/// <summary>
	/// Calculates the SHA512 hash.
	/// The value composed of the password and salt (with the salt first, then the password) is converted to a UTF-16 little endian byte sequence (Encoding.Unicode.GetBytes), and its hash is calculated.
	/// The result is returned as a string by converting each byte of the hash to a string representing the byte in hexadecimal format (ToString("X2")).
	/// 
	/// The method returns the same result as the T-SQL function HASHBYTES in the following example. The text value in T-SQL needs to be prefixed with N, otherwise it gives a different result!
	/// C#: HashCalculator.ComputeSHA512HashString("password", "salt")
	/// T-SQL: select HASHBYTES('SHA2_512', N'saltpassword')
	/// 
	/// HASH: ACDDA85B31ACA524AF221BF8BB635583167414180D55985294A64E48E82796627CCCAE6CA4A3C3B2B568478B0265FE62753C37119B899BE7E632D434C8B2A54E
	/// 
	/// Note that the method returns a different value than "regular" calculators on the web that use UTF-8 due to the encoding used. Our advantage is compatibility with the HASHBYTES method in T-SQL.
	/// </summary>
	/// <param name="plainTextPassword">The password for which we want to obtain the hash.</param>
	/// <param name="salt">The salt.</param>
	/// <returns>The calculated SHA512 hash from the salt and password, converted to a string.</returns>
	public static string ComputeSHA512HashString(string plainTextPassword, string salt = "")
	{
		string value = salt + plainTextPassword;

		SHA512 sha = SHA512.Create();
		var hash = sha.ComputeHash(Encoding.Unicode.GetBytes(value));
		return String.Join("", hash.Select(x => x.ToString("X2")));
	}

	/// <summary>
	/// Returns true if the passwordHash corresponds to the hash calculated from the plainTextPassword with the given salt.
	/// </summary>
	/// <param name="plainTextPassword">The password for which we want to verify the hash.</param>
	/// <param name="salt">The salt of the password for which we want to verify the hash.</param>
	/// <param name="passwordHash">The hash of the password being verified.</param>
	public static bool VerifySHA512HashString(string plainTextPassword, string salt, string passwordHash)
	{
		string computedPasswordHash = ComputeSHA512HashString(plainTextPassword, salt);
		return String.Equals(computedPasswordHash, passwordHash, StringComparison.InvariantCultureIgnoreCase);
	}
}
