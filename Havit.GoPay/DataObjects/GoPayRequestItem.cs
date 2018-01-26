using Newtonsoft.Json;

namespace Havit.GoPay.DataObjects
{
	/// <summary>
	/// Poloûka objedn·vky
	/// </summary>
	public class GoPayRequestItem
	{
		/// <summary>
		/// PoËet poloûek produktu
		/// </summary>
		[JsonProperty("count")]
		public long Count { get; set; }

		/// <summary>
		/// N·zev produktu
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; set; }

		/// <summary>
		/// »·stka za jednotku produktu
		/// </summary>
		[JsonIgnore]
		public decimal Amount { get; set; }

		/// <summary>
		/// »·stka platby n·sobena 100
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
}