using Newtonsoft.Json;

namespace Havit.GoPay.DataObjects;

/// <summary>
/// Položka objednávky
/// </summary>
public class GoPayRequestItem
{
	/// <summary>
	/// Počet položek produktu
	/// </summary>
	[JsonProperty("count")]
	public long Count { get; set; }

	/// <summary>
	/// Název produktu
	/// </summary>
	[JsonProperty("name")]
	public string Name { get; set; }

	/// <summary>
	/// Částka za jednotku produktu
	/// </summary>
	[JsonIgnore]
	public decimal Amount { get; set; }

	/// <summary>
	/// Částka platby násobena 100
	/// </summary>
	[JsonProperty("amount")]
	internal long AmountMultipliedBy100 => (long)(Amount * 100);

	/// <summary>
	/// Konstruktor
	/// </summary>
	public GoPayRequestItem()
	{
		Count = 1;
	}
}