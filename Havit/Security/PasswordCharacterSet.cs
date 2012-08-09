using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Security
{
	/// <summary>
	/// Sada znakù, z níž <see cref="PasswordGenerator"/> vybírá znaky pro generování hesla.
	/// </summary>
	public enum PasswordCharacterSet
	{
		/// <summary>
		/// Pouze malá písmena.
		/// </summary>
		LowerCaseLetters,

		/// <summary>
		/// Velká a malá písmena.
		/// </summary>
		Letters,

		/// <summary>
		/// Písmena (velká i malá) a èíslice.
		/// </summary>
		LettersAndDigits,

		/// <summary>
		/// Písmena (velká i malá), èíslice a speciální znaky.
		/// </summary>
		LettersDigitsAndSpecialCharacters
	}
}
