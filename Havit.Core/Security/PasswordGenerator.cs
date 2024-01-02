using System;
using System.Security.Cryptography;
using System.Text;

namespace Havit.Security;

/// <summary>
/// Password generator.
/// </summary>
/// <remarks>
/// Originally based on http://www.codeproject.com/csharp/pwdgen.asp
/// </remarks>
public class PasswordGenerator
{
	/// <summary>
	/// Minimum password length. Default is 6.
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
	/// Maximum password length. Default is 10.
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
	/// Set of characters to select from for the generated password.
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
	/// Indicates whether repeating characters are allowed in the password. Default is <c>true</c>.
	/// </summary>
	public bool AllowRepeatingCharacters
	{
		get { return this._allowRepeatingCharacters; }
		set { this._allowRepeatingCharacters = value; }
	}
	private bool _allowRepeatingCharacters;

	/// <summary>
	/// Indicates whether the password can contain clusters of the same character. Default is <c>false</c>.
	/// </summary>
	public bool AllowConsecutiveCharacters
	{
		get { return this._allowConsecutiveCharacters; }
		set { this._allowConsecutiveCharacters = value; }
	}
	private bool _allowConsecutiveCharacters;

	/// <summary>
	/// String of characters that we do not want in the password.
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
	/// Creates an instance of PasswordGenerator and sets default values for the complexity of the generated password.
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
	/// Returns the upper index of the character array from which selection can be made for the generated password.
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
	/// Generates a random number using crypto-API.
	/// </summary>
	/// <param name="lBound">lower bound</param>
	/// <param name="uBound">upper bound</param>
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
	/// Returns a random character.
	/// </summary>
	protected char GetRandomCharacter()
	{
		int upperBound = GetCharacterArrayUpperBound();

		int randomCharPosition = GetCryptographicRandomNumber(pwdCharArray.GetLowerBound(0), upperBound);

		char randomChar = pwdCharArray[randomCharPosition];

		return randomChar;
	}

	/// <summary>
	/// Generates a password of complexity according to the set generator.
	/// </summary>
	/// <returns>generated password</returns>
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
			// If the password is to be larger than the possible number of characters, then it must be shortened.
			// The minimum length is already ensured by ValidateSettings();
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
	/// Checks the generator settings and throws exceptions if necessary.
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
	/// Generates a password of complexity according to the required parameters.
	/// </summary>
	/// <param name="minimumLength">minimum password length</param>
	/// <param name="maximumLength">maximum password length</param>
	/// <param name="passwordCharacterSet">Set of characters to select from for the generated password.</param>
	/// <param name="allowRepeatingCharacters">Indicates whether repeating characters are allowed in the password.</param>
	/// <param name="allowConsecutiveCharacters">Indicates whether the password can contain clusters of the same character.</param>
	/// <returns>generated password meeting the input requirements</returns>
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
	/// Generates a password of complexity according to the required parameters.
	/// </summary>
	/// <param name="minimumLength">minimum password length</param>
	/// <param name="maximumLength">maximum password length</param>
	/// <param name="passwordCharacterSet">Set of characters to select from for the generated password.</param>
	/// <returns>generated password meeting the input requirements</returns>
	public static string Generate(int minimumLength, int maximumLength, PasswordCharacterSet passwordCharacterSet)
	{
		PasswordGenerator passwordGenerator = new PasswordGenerator();
		passwordGenerator.MinimumLength = minimumLength;
		passwordGenerator.MaximumLength = maximumLength;
		passwordGenerator.PasswordCharacterSet = passwordCharacterSet;

		return passwordGenerator.Generate();
	}

	/// <summary>
	/// Generates a password of complexity according to the required parameters.
	/// </summary>
	/// <param name="length">password length</param>
	/// <param name="passwordCharacterSet">Set of characters to select from for the generated password.</param>
	/// <returns>generated password meeting the input requirements</returns>
	public static string Generate(int length, PasswordCharacterSet passwordCharacterSet)
	{
		PasswordGenerator passwordGenerator = new PasswordGenerator();
		passwordGenerator.MinimumLength = length;
		passwordGenerator.MaximumLength = length;
		passwordGenerator.PasswordCharacterSet = passwordCharacterSet;

		return passwordGenerator.Generate();
	}
}
