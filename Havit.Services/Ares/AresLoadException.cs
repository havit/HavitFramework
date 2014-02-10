using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services.Ares
{
	/// <summary>
	/// Výjimka identifikující neúspěch při čtení dat z ARESu (ARES nedostupný, timeout, atp.).
	/// </summary>
	public class AresLoadException : AresBaseException
	{
		#region Construktor
		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="message">Chyba vrácená systém při příštupu do ARESu.</param>
		internal AresLoadException(string message)
			: base(message)
		{

		}
		#endregion

	}
}
