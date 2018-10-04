using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.GoPay.DataObjects.Errors
{
	/// <summary>
	/// Rozsah chyby
	/// </summary>
	public enum GoPayErrorItemScope
	{
		/// <summary>
		/// Field error
		/// </summary>
		F,

		/// <summary>
		/// Global error
		/// </summary>
		G
	}
}
