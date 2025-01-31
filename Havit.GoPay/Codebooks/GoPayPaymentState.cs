using System.Runtime.Serialization;

namespace Havit.GoPay.Codebooks;

/// <summary>
/// Stav platby
/// </summary>
public enum GoPayPaymentState
{
	/// <summary>
	/// Platba založena
	/// </summary>
	[EnumMember(Value = "CREATED")]
	Created,

	/// <summary>
	/// Platební metoda vybrána
	/// </summary>
	[EnumMember(Value = "PAYMENT_METHOD_CHOSEN")]
	PaymentMethodChosen,

	/// <summary>
	/// Platba zaplacena
	/// </summary>
	[EnumMember(Value = "PAID")]
	Paid,

	/// <summary>
	/// Platba předautorizována
	/// </summary>
	[EnumMember(Value = "AUTHORIZED")]
	Authorized,

	/// <summary>
	/// Platba zrušena
	/// </summary>
	[EnumMember(Value = "CANCELED")]
	Canceled,

	/// <summary>
	/// Vypršelá platnost platby
	/// </summary>
	[EnumMember(Value = "TIMEOUTED")]
	Timeouted,

	/// <summary>
	/// O platbu bylo zažádáno
	/// </summary>
	[EnumMember(Value = "REQUESTED")]
	Requested,

	/// <summary>
	/// Platba refundována
	/// </summary>
	[EnumMember(Value = "REFUNDED")]
	Refunded,

	/// <summary>
	/// Platba částečně refundována
	/// </summary>
	[EnumMember(Value = "PARTIALLY_REFUNDED")]
	PartiallyRefunded,

	/// <summary>
	/// Platba stržena
	/// </summary>
	[EnumMember(Value = "CAPTURED")]
	Captured
}
