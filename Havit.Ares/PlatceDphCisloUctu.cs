
namespace Havit.Ares;

/// <summary>
/// Registrované číslo u FU
/// </summary>
public class PlatceDphCisloUctu
{
	/// Datum zveřejnění účtu
	public DateTime DatumZverejneni { get; set; }
	/// Datum ukončení zveřejnění účtu
	public DateTime DatumUkonceni { get; set; }
	/// Číslo účtu
	public string Cislo { get; set; }
	/// Předčíslo účtu
	public string Predcisli { get; set; }
	/// Kód banky
	public string KodBanky { get; set; }
	/// Standardní forma čísla účtu: Předčíslí-Číslo účtu/Kód banky
	public string CisloFull { get { return (Predcisli == null || Predcisli == "" ? "" : Predcisli + "-") + $"{Cislo}/{KodBanky}"; } }
}
