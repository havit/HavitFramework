using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Havit.GoPay.Codebooks
{
	/// <summary>
	/// Měna platby
	/// </summary>
	public enum GoPayCurrency
	{
		/// <summary>
		/// České koruny
		/// </summary>
		[EnumMember(Value = "CZK")]
		CZK,

		/// <summary>
		/// Eura
		/// </summary>
		[EnumMember(Value = "EUR")]
		EUR
	}
}