namespace Havit.GoPay.DataObjects;

/// <summary>
/// Dodatečné parametry platby
/// </summary>
public class GoPayAdditionalParameter
{
	/// <summary>
	/// Název parametru
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// Hodnota volitelného parametru
	/// </summary>
	public string Value { get; set; }
}