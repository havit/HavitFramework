using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Havit.Security
{
	/// <summary>
	/// Generátor hesel.
	/// </summary>
	/// <remarks>
	/// Vychází původně z http://www.codeproject.com/csharp/pwdgen.asp
	/// </remarks>
	public class PasswordGenerator
	{
		/// <summary>
		/// Minimální délka hesla. Default 6.
		/// </summary>
		public int MinimumLength
		{
			get
			{
				return this._mininumLength;
			}
			set
			{
				this._mininumLength = value;
			}
		}
		private int _mininumLength;

		/// <summary>
		/// Maximální délka hesla. Default 10.
		/// </summary>
		public int MaximumLength
		{
			get
			{
				return this._maximumLength;
			}
			set
			{
				this._maximumLength = value;
			}
		}
		private int _maximumLength;

		/// <summary>
		/// Sada znaků, z níž se mají vybírat znaky pro generované heslo.
		/// </summary>
		public PasswordCharacterSet PasswordCharacterSet
		{
			get
			{
				return this._passwordCharacterSet;
			}
			set
			{
				this._passwordCharacterSet = value;
				passwordCharArrayUpperBound = GetCharacterArrayUpperBound();
			}
		}
		private PasswordCharacterSet _passwordCharacterSet;
		private int passwordCharArrayUpperBound;

		/// <summary>
		/// Indikuje, zdali se smí v heslu opakovat znaky. zdali může být některý znak v heslu vícekrát. Default <c>true</c>.
		/// </summary>
		public bool AllowRepeatingCharacters
		{
			get { return this._allowRepeatingCharacters; }
			set { this._allowRepeatingCharacters = value; }
		}
		private bool _allowRepeatingCharacters;

		/// <summary>
		/// Indikuje, zdali smí heslo obsahovat shluky stejných znaků. Default <c>false</c>.
		/// </summary>
		public bool AllowConsecutiveCharacters
		{
			get { return this._allowConsecutiveCharacters; }
			set { this._allowConsecutiveCharacters = value; }
		}
		private bool _allowConsecutiveCharacters;

		/// <summary>
		/// Řetězec znaků, které nechceme mít v heslu.
		/// </summary>
		public string Exclusions
		{
			get { return this._exclusions; }
			set { this._exclusions = value; }
		}
		private string _exclusions;

		private const int DefaultMinimum = 6;
		private const int DefaultMaximum = 10;
		private const int LowerCaseLettersUpperBound = 25;
		private const int LettersUpperBound = 51;
		private const int LettersAndDigitsUpperBound = 61;

		/// <summary>
		/// Vytvoří instanci PasswordGeneratoru a nastaví výchozí hodnoty pro složitost generovaného hesla.
		/// </summary>
		public PasswordGenerator()
		{
			this.MinimumLength = DefaultMinimum;
			this.MaximumLength = DefaultMaximum;
			this.AllowConsecutiveCharacters = false;
			this.AllowRepeatingCharacters = true;
			this.PasswordCharacterSet = PasswordCharacterSet.LettersAndDigits;
			this.Exclusions = null;
		}

		/// <summary>
		/// Vrátí horní index pole znaků, do kterého se smí provádět výběr pro generované heslo.
		/// </summary>
		private int GetCharacterArrayUpperBound()
		{
			int upperBound = pwdCharArray.GetUpperBound(0);

			switch (this.PasswordCharacterSet)
			{
				case PasswordCharacterSet.LowerCaseLetters:
					upperBound = PasswordGenerator.LowerCaseLettersUpperBound;
					break;
				case PasswordCharacterSet.Letters:
					upperBound = PasswordGenerator.LettersUpperBound;
					break;
				case PasswordCharacterSet.LettersAndDigits:
					upperBound = PasswordGenerator.LettersAndDigitsUpperBound;
					break;
				case PasswordCharacterSet.LettersDigitsAndSpecialCharacters:
					// NOOP
					break;
				default:
					throw new NotSupportedException($"{PasswordCharacterSet} not supported.");
			}
			return upperBound;
		}

		/// <summary>
		/// Vygeneruje náhodné číslo pomocí crypto-API.
		/// </summary>
		/// <param name="lBound">dolní mez</param>
		/// <param name="uBound">horní mez</param>
		protected int GetCryptographicRandomNumber(int lBound, int uBound)
		{
			// Assumes lBound >= 0 && lBound < uBound
			// returns an int >= lBound and < uBound
			uint urndnum;
			byte[] rndnum = new Byte[4];
			if (lBound == uBound - 1)
			{
				// test for degenerate case where only lBound can be returned   
				return lBound;
			}

			uint xcludeRndBase = (uint.MaxValue - (uint.MaxValue % (uint)(uBound - lBound)));

			do
			{
				randomNumberGenerator.GetBytes(rndnum);
				urndnum = System.BitConverter.ToUInt32(rndnum, 0);
			}
			while (urndnum >= xcludeRndBase);

			return (int)(urndnum % (uBound - lBound)) + lBound;
		}

		/// <summary>
		/// Vrátí náhodný znak.
		/// </summary>
		protected char GetRandomCharacter()
		{
			int upperBound = GetCharacterArrayUpperBound();

			int randomCharPosition = GetCryptographicRandomNumber(pwdCharArray.GetLowerBound(0), upperBound);

			char randomChar = pwdCharArray[randomCharPosition];

			return randomChar;
		}

		/// <summary>
		/// Vygeneruje heslo složitosti dle nastaveného generátoru.
		/// </summary>
		/// <returns>vygenerované heslo</returns>
		public string Generate()
		{
			ValidateSettings();

			int passwordLength;
			if (this.MinimumLength == this.MaximumLength)
			{
				passwordLength = this.MinimumLength;
			}
			else
			{
				// Pick random length between minimum and maximum   
				passwordLength = GetCryptographicRandomNumber(this.MinimumLength, this.MaximumLength);
			}

			if ((!AllowRepeatingCharacters) && (passwordLength > passwordCharArrayUpperBound + 1))
			{
				// Pokud má být heslo větší, než je možný počet znaků, pak ho musíme zkrátit.
				// Minimální délku už zajišťuje ValidateSettings();
				passwordLength = passwordCharArrayUpperBound + 1;
			}

			StringBuilder paswordBuffer = new StringBuilder();
			paswordBuffer.Capacity = this.MaximumLength;

			// Generate random characters
			char lastCharacter;
			char nextCharacter;

			// Initial dummy character flag
			nextCharacter = '\n';
			lastCharacter = nextCharacter;

			for (int i = 0; i < passwordLength; i++)
			{
				nextCharacter = GetRandomCharacter();

				if (!this.AllowConsecutiveCharacters)
				{
					while (lastCharacter == nextCharacter)
					{
						nextCharacter = GetRandomCharacter();
					}
				}

				if (!this.AllowRepeatingCharacters)
				{
					string temp = paswordBuffer.ToString();
					int duplicateIndex = temp.IndexOf(nextCharacter);
					while (-1 != duplicateIndex)
					{
						nextCharacter = GetRandomCharacter();
						duplicateIndex = temp.IndexOf(nextCharacter);
					}
				}

				if ((null != this.Exclusions))
				{
					while (-1 != this.Exclusions.IndexOf(nextCharacter))
					{
						nextCharacter = GetRandomCharacter();
					}
				}

				paswordBuffer.Append(nextCharacter);
				lastCharacter = nextCharacter;
			}

			if (null != paswordBuffer)
			{
				return paswordBuffer.ToString();
			}
			else
			{
				return String.Empty;
			}
		}

		/// <summary>
		/// Kontroluje nastavení generátoru a vyhazuje případné výjimky.
		/// </summary>
		private void ValidateSettings()
		{
			if (this.MaximumLength < this.MinimumLength)
			{
				throw new InvalidOperationException("MaximumLength < MinimumLength");
			}

			if ((!AllowRepeatingCharacters) && (this.MinimumLength > passwordCharArrayUpperBound + 1))
			{
				throw new InvalidOperationException("Není dostatek znaků pro vygenerování hesla požadované délky.");
			}
		}

#if NET6_0_OR_GREATER
		private readonly RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
#else
		private readonly RNGCryptoServiceProvider randomNumberGenerator = new RNGCryptoServiceProvider();
#endif
		private readonly char[] pwdCharArray = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789`~!@#$%^&*()-_=+[]{}\\|;:'\",<.>/?".ToCharArray();

		/// <summary>
		/// Vygeneruje heslo složitosti dle požadovaných parametrů.
		/// </summary>
		/// <param name="minimumLength">minimální délka hesla</param>
		/// <param name="maximumLength">maximální délka hesla</param>
		/// <param name="passwordCharacterSet">Sada znaků, z níž se mají vybírat znaky pro generované heslo.</param>
		/// <param name="allowRepeatingCharacters">Indikuje, zdali se smí v heslu opakovat znaky. zdali může být některý znak v heslu vícekrát.</param>
		/// <param name="allowConsecutiveCharacters">Indikuje, zdali smí heslo obsahovat shluky stejných znaků.</param>
		/// <returns>vygenerované heslo odpovídající vstupním požadavkům</returns>
		public static string Generate(int minimumLength, int maximumLength, PasswordCharacterSet passwordCharacterSet, bool allowRepeatingCharacters, bool allowConsecutiveCharacters)
		{
			PasswordGenerator passwordGenerator = new PasswordGenerator();
			passwordGenerator.MinimumLength = minimumLength;
			passwordGenerator.MaximumLength = maximumLength;
			passwordGenerator.PasswordCharacterSet = passwordCharacterSet;
			passwordGenerator.AllowRepeatingCharacters = allowRepeatingCharacters;
			passwordGenerator.AllowConsecutiveCharacters = allowConsecutiveCharacters;

			return passwordGenerator.Generate();
		}

		/// <summary>
		/// Vygeneruje heslo složitosti dle požadovaných parametrů.
		/// </summary>
		/// <param name="minimumLength">minimální délka hesla</param>
		/// <param name="maximumLength">maximální délka hesla</param>
		/// <param name="passwordCharacterSet">Sada znaků, z níž se mají vybírat znaky pro generované heslo.</param>
		/// <returns>vygenerované heslo odpovídající vstupním požadavkům</returns>
		public static string Generate(int minimumLength, int maximumLength, PasswordCharacterSet passwordCharacterSet)
		{
			PasswordGenerator passwordGenerator = new PasswordGenerator();
			passwordGenerator.MinimumLength = minimumLength;
			passwordGenerator.MaximumLength = maximumLength;
			passwordGenerator.PasswordCharacterSet = passwordCharacterSet;

			return passwordGenerator.Generate();
		}

		/// <summary>
		/// Vygeneruje heslo složitosti dle požadovaných parametrů.
		/// </summary>
		/// <param name="length"> délka hesla</param>
		/// <param name="passwordCharacterSet">Sada znaků, z níž se mají vybírat znaky pro generované heslo.</param>
		/// <returns>vygenerované heslo odpovídající vstupním požadavkům</returns>
		public static string Generate(int length, PasswordCharacterSet passwordCharacterSet)
		{
			PasswordGenerator passwordGenerator = new PasswordGenerator();
			passwordGenerator.MinimumLength = length;
			passwordGenerator.MaximumLength = length;
			passwordGenerator.PasswordCharacterSet = passwordCharacterSet;

			return passwordGenerator.Generate();
		}
	}
}
