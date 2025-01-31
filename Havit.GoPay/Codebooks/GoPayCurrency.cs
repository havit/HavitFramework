using System.Runtime.Serialization;

namespace Havit.GoPay.Codebooks;

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