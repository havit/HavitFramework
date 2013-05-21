using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Services.Ares
{
	/// <summary>
	/// Výjimka identifikující chybu vrácenou z obchodního rejstříku.
	/// </summary>
	public class AresException : ApplicationException
	{
		#region Construktor
		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="message">Chyba vrácená obchodním rejstříkem.</param>
		internal AresException(string message)
			: base(message)
		{

		}
		#endregion
	}
}
