using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.AspNet.Mvc.Messenger
{
	/// <summary>
	/// Message type.
	/// </summary>
	public enum MessageType
	{
		/// <summary>
		/// Informace, potvrzení operace.
		/// </summary>
		Info = 1,

		/// <summary>
		/// Úspěšná operace.
		/// </summary>
		Success = 2,

		/// <summary>
		/// Varování, např. upozornění na další nutné kroky k dokončení operace.
		/// </summary>
		Warning = 3,

		/// <summary>
		/// Chyba.
		/// </summary>
		Error = 4
	}
}
