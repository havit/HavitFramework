using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services.Ares
{
	/// <summary>
	/// Předek výjimek vracených z ARESu.
	/// </summary>
	public abstract class AresBaseException : ApplicationException
	{
		#region Construktor
		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="message">Exception message.</param>
		internal AresBaseException(string message)
			: base(message)
		{

		}
		#endregion
	}
}
