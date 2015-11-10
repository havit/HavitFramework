using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Web.UI
{
	/// <summary>
	/// Výjimka oznamující nenačtení viewstate.
	/// </summary>
	[Serializable]
	public class ViewStateLoadFailedException : Exception
	{
		#region Constructors
		/// <summary>
		/// Konstruktor.
		/// </summary>
		public ViewStateLoadFailedException()
		{
			
		}

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public ViewStateLoadFailedException(string message)
			: base(message)
		{

		}

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public ViewStateLoadFailedException(string message, Exception innerException)
			: base(message, innerException)
		{

		}
		#endregion

	}
}
