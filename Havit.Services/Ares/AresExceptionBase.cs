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
	public abstract class AresExceptionBase : ApplicationException
	{
		#region Construktor
		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="message">Exception message.</param>
		internal AresExceptionBase(string message)
			: base(message)
		{

		}
		#endregion
	}
}
