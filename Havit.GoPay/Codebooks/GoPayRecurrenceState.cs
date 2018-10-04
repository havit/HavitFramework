using System.Runtime.Serialization;

namespace Havit.GoPay.Codebooks
{
	/// <summary>
	/// Stav opakované platby
	/// </summary>
	public enum GoPayRecurrenceState
	{
		/// <summary>
		/// Zažádáno o opakování platby
		/// </summary>
		[EnumMember(Value = "REQUESTED")]
		Requested,

		/// <summary>
		/// Opakování platby začalo platit
		/// </summary>
		[EnumMember(Value = "STARTED")]
		Started,

		/// <summary>
		/// Opakování platby skončilo
		/// </summary>
		[EnumMember(Value = "STOPPED")]
		Stopped
	}
}