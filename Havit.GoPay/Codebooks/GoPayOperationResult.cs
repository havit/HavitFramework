using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Havit.GoPay.Codebooks
{
	/// <summary>
	/// Výsledek operace
	/// </summary>
	public enum GoPayOperationResult
	{
		/// <summary>
		/// Požadavek přijat
		/// </summary>
		[EnumMember(Value = "ACCEPTED")]
		Accepted,

		/// <summary>
		/// Operace provedena
		/// </summary>
		[EnumMember(Value = "FINISHED")]
		Finished,

		/// <summary>
		/// Operace skončila chybou
		/// </summary>
		[EnumMember(Value = "FAILED")]
		Failed
	}
}
