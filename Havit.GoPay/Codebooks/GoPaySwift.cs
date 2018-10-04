using System.Runtime.Serialization;

namespace Havit.GoPay.Codebooks
{
	/// <summary>
	/// SWIFT kódy bank
	/// </summary>
	public enum GoPaySwift
	{
		/// <summary>
		/// Česká spořitelna
		/// </summary>
		[EnumMember(Value = "GIBACZPX")]
		GIBACZPX,

		/// <summary>
		/// Komerční Banka
		/// </summary>
		[EnumMember(Value = "KOMBCZPP")]
		KOMBCZPP,

		/// <summary>
		/// Raiffeisenbank
		/// </summary>
		[EnumMember(Value = "RZBCCZPP")]
		RZBCCZPP,

		/// <summary>
		/// mBank
		/// </summary>
		[EnumMember(Value = "BREXCZPP")]
		BREXCZPP,

		/// <summary>
		/// FIO Banka
		/// </summary>
		[EnumMember(Value = "FIOBCZPP")]
		FIOBCZPP,

		/// <summary>
		/// ČSOB
		/// </summary>
		[EnumMember(Value = "CEKOCZPP")]
		CEKOCZPP,

		/// <summary>
		/// ERA
		/// </summary>
		[EnumMember(Value = "CEKOCZPP_ERA")]
		CEKOCZPP_ERA,

		/// <summary>
		/// Všeobecná úverová banka
		/// </summary>
		[EnumMember(Value = "SUBASKBX")]
		SUBASKBX,

		/// <summary>
		/// Tatra Banka
		/// </summary>
		[EnumMember(Value = "TATRSKBX")]
		TATRSKBX,

		/// <summary>
		/// Unicredit Bank SK
		/// </summary>
		[EnumMember(Value = "UNCRSKBX")]
		UNCRSKBX,

		/// <summary>
		/// Slovenská spořitelna
		/// </summary>
		[EnumMember(Value = "GIBASKBX")]
		GIBASKBX,

		/// <summary>
		/// OTP Banka
		/// </summary>
		[EnumMember(Value = "OTPVSKBX")]
		OTPVSKBX,

		/// <summary>
		/// Poštová Banka
		/// </summary>
		[EnumMember(Value = "POBNSKBA")]
		POBNSKBA,

		/// <summary>
		/// ČSOB SK
		/// </summary>
		[EnumMember(Value = "CEKOSKBX")]
		CEKOSKBX,

		/// <summary>
		/// Sberbank Slovensko
		/// </summary>
		[EnumMember(Value = "LUBASKBX")]
		LUBASKBX,

		/// <summary>
		/// Speciální swift bez předvýběru konkrétní banky
		/// </summary>
		[EnumMember(Value = "OTHERS")]
		OTHERS
	}
}