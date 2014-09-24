using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Security
{
	/// <summary>
	/// Pomocné metody práci s hashi hesel.
	/// </summary>
	public static class PasswordHashService
	{
		#region CreateSalt
		/// <summary>
		/// Generuje náhodnou sůl.
		/// Implemnetováno pomocí PasswordGeneratoru.
		/// </summary>
		/// <param name="saltLength">Požadovaná délka soli (výchozí délka je 8 znaků).</param>
		/// <param name="passwordCharacterSet">Sada znaků pro výběr soli (výchozí hodnota je PasswordCharacterSet.LettersAndDigits).</param>
		public static string CreateSalt(int saltLength = 8, PasswordCharacterSet passwordCharacterSet = PasswordCharacterSet.LettersAndDigits)
		{
			return PasswordGenerator.Generate(saltLength, saltLength, passwordCharacterSet, true, true);
		}
		#endregion

		#region ComputeSHA512HashString
		/// <summary>
		/// Kalkuluje hash algoritmem SHA512.
		/// Z hodnoty sestavené z hesla a soli (a to nejprve je sůl, poté heslo) je získána UTF-16 little endien posloupnost bytů (Encoding.Unicode.GetBytes), jejíž hash je kalkulován.
		/// Výsledek  je vrácen jako text převedením každého byte hashe do řetězce reprezentující byte v šestnáctkové soustavě (ToString("X2")).
		/// 
		/// Metoda vrací stejný výsledek, jako T-SQL funkce HASHBYTES v následujícím příkladu. Před textovou hodnotou v T-SQL je potřeba uvést N, jinak dává jiný výsledek!
		/// C#: HashCalculator.ComputeSHA512HashString("plainTextPassword", "salt")
		/// T-SQL: select HASHBYTES('SHA2_512', N'saltpassword')
		/// 
		/// HASH: ACDDA85B31ACA524AF221BF8BB635583167414180D55985294A64E48E82796627CCCAE6CA4A3C3B2B568478B0265FE62753C37119B899BE7E632D434C8B2A54E
		/// 
		/// Pozor, metoda díky použitému kódování vrací jinou hodnotu než "běžné" kalkulátory na webu, které používají UTF-8. Naší výhodou je kompatibilita s metodou HASHBYTES v T-SQL.
		/// </summary>
		/// <param name="plainTextPassword">Heslo, jehož hash chceme získat.</param>
		/// <param name="salt">Sůl.</param>
		/// <returns>Kalkulovaný SHA 512 ze soli a hesla, převedený do řetězce .</returns>
		public static string ComputeSHA512HashString(string plainTextPassword, string salt = "")
		{
			string value = salt + plainTextPassword;

			SHA512Managed sha = new SHA512Managed();
			var hash = sha.ComputeHash(Encoding.Unicode.GetBytes(value));
			return String.Join("", hash.Select(x => x.ToString("X2")));
		}
		#endregion

		#region MatchesSHA512HashString
		/// <summary>
		/// Vrací true, pokud passwordHash odpovídá hashi zkalkulovanému z plainTextPasswordu s danou solí.
		/// </summary>
		/// <param name="plainTextPassword">Heslo, jehož hash chceme ověřit.</param>
		/// <param name="salt">Sůl hesla, jehož hash chceme ověřit.</param>
		/// <param name="passwordHash">Ověřovaný hash hesla.</param>
		public static bool MatchesSHA512HashString(string plainTextPassword, string salt, string passwordHash)
		{
			string computedPasswordHash = ComputeSHA512HashString(plainTextPassword, salt);
			return String.Equals(computedPasswordHash, passwordHash, StringComparison.InvariantCultureIgnoreCase);
		}
		#endregion
	}
}
