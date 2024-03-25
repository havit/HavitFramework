namespace Havit.Ares.FinancniSprava;

/// <summary>
/// Návratová hodnota volání PlatceDphService.
/// </summary>
public class PlatceDphResult
{
	/// <summary>
	/// Nespolehlivý plátce DPH 
	/// </summary>
	public bool IsNespolehlivy { get; set; }

	/// <summary>
	/// Datum zveřejnění jako nespolehlivý plátce. 
	/// </summary>
	public DateTime? NespolehlivyOd { get; set; }

	/// <summary>
	/// Nalezené DIČ. 
	/// </summary>
	public string Dic { get; set; }

	/// <summary>
	/// Klíč pro hledání v číselníku Finanční úřad
	/// </summary>
	public string CisloFinancnihoUradu { get; set; }

	/// <summary>
	/// Název finančního úřadu z číselníku.
	/// </summary>
	public string NazevFinancnihoUradu { get; set; }

	/// <summary>
	/// Čísla účtů registrovaných u finančního úřadu.
	/// </summary>
	public List<PlatceDphCisloUctu> CislaUctu { get; } = new List<PlatceDphCisloUctu>();
}
