using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Havit.GoPay.Codebooks
{
	/// <summary>
	/// Kódy platebních metod
	/// </summary>
	public enum GoPayPaymentInstrument
	{
		/// <summary>
		/// Platební karty
		/// </summary>
		[EnumMember(Value = "PAYMENT_CARD")]
		PAYMENT_CARD,

		/// <summary>
		/// Bankovní převody
		/// </summary>
		[EnumMember(Value = "BANK_ACCOUNT")]
		BANK_ACCOUNT,

		/// <summary>
		/// Premium SMS
		/// </summary>
		[EnumMember(Value = "PRSMS")]
		PRSMS,

		/// <summary>
		/// Mplatba
		/// </summary>
		[EnumMember(Value = "MPAYMENT")]
		MPAYMENT,

		/// <summary>
		/// paysafecard
		/// </summary>
		[EnumMember(Value = "PAYSAFECARD")]
		PAYSAFECARD,

		/// <summary>
		/// superCASH
		/// </summary>
		[EnumMember(Value = "SUPERCASH")]
		SUPERCASH,

		/// <summary>
		/// GoPay účet
		/// </summary>
		[EnumMember(Value = "GOPAY")]
		GOPAY,

		/// <summary>
		/// PayPal účet
		/// </summary>
		[EnumMember(Value = "PAYPAL")]
		PAYPAL,

		/// <summary>
		/// Platba bitcoiny
		/// </summary>
		[EnumMember(Value = "BITCOIN")]
		BITCOIN
	}
}