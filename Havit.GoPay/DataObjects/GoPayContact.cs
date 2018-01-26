using Newtonsoft.Json;

namespace Havit.GoPay.DataObjects
{
	/// <summary>
	/// Informace o zákazníkovi
	/// </summary>
	public class GoPayContact
	{
		/// <summary>
		/// Jméno
		/// </summary>
		[JsonProperty("first_name")]
		public string FirstName { get; set; }

		/// <summary>
		/// Příjmení
		/// </summary>
		[JsonProperty("last_name")]
		public string LastName { get; set; }

		/// <summary>
		/// Email
		/// </summary>
		[JsonProperty("email")]
		public string Email { get; set; }

		/// <summary>
		/// Telefonní číslo
		/// </summary>
		[JsonProperty("phone_number")]
		public string PhoneNumber { get; set; }

		/// <summary>
		/// Město
		/// </summary>
		[JsonProperty("city")]
		public string City { get; set; }

		/// <summary>
		/// Ulice
		/// </summary>
		[JsonProperty("street")]
		public string Street { get; set; }

		/// <summary>
		/// Poštovní směrovací číslo
		/// </summary>
		[JsonProperty("postal_code")]
		public string PostalCode { get; set; }

		/// <summary>
		/// Kód země - např. CZE dle ISO 3166-1 alpha-3
		/// </summary>
		[JsonProperty("country_code")]
		public string CountryCode { get; set; }
	}
}