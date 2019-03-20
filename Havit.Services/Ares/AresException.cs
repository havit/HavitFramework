using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Services.Ares
{
	/// <summary>
	/// Výjimka identifikující chybu vrácenou z ARESu.
	/// </summary>
	public class AresException : AresBaseException
	{
		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="message">Chyba vrácená obchodním rejstříkem.</param>
		internal AresException(string message)
			: base(message)
		{

		}
	}
}
