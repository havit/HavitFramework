namespace Havit.Ares.FinancniSprava;

/// <summary>
/// Registrované číslo u finančního úřadu.
/// </summary>
public class PlatceDphCisloUctu
{
	/// <summary>
	/// Datum zveřejnění účtu
	/// </summary>
	public DateTime DatumZverejneni { get; set; }

	/// <summary>
	/// Datum ukončení zveřejnění účtu
	/// </summary>
	/// 
	public DateTime? DatumUkonceni { get; set; }

	/// <summary>
	/// Číslo účtu
	/// </summary>
	public string CisloUctu { get; set; }

	/// <summary>
	/// Předčíslo účtu
	/// </summary>
	public string Predcisli { get; set; }

	/// <summary>
	/// Kód banky
	/// </summary>
	public string KodBanky { get; set; }

	/// <summary>
	/// Standardní forma čísla účtu: Předčíslí-Číslo účtu/Kód banky
	/// </summary>
	public string FullCisloUctu => (string.IsNullOrEmpty(Predcisli) ? "" : Predcisli + "-") + $"{CisloUctu}/{KodBanky}";
}
