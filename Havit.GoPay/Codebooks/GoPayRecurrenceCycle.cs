using System.Runtime.Serialization;

namespace Havit.GoPay.Codebooks;

/// <summary>
/// Cyklus opakované platby
/// </summary>
public enum GoPayRecurrenceCycle
{
	/// <summary>
	/// Denní cyklus
	/// </summary>
	[EnumMember(Value = "DAY")]
	Day,

	/// <summary>
	/// Týdenní cyklus
	/// </summary>
	[EnumMember(Value = "WEEK")]
	Week,

	/// <summary>
	/// Měsíční cyklus
	/// </summary>
	[EnumMember(Value = "MONTH")]
	Month,

	/// <summary>
	/// Na vyžádání
	/// </summary>
	[EnumMember(Value = "ON_DEMAND")]
	OnDemand
}