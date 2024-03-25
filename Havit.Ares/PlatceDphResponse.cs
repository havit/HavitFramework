namespace Havit.Ares;

/// <summary>
/// Response volání Service PlatceDphService
/// </summary>
public class PlatceDphResponse
{
	/// Spolehlivý plátce DPH
	public bool IsSpolehlivy { get { return !IsNespolehlivy; } }
	/// Nespolehlivý plátce DPH 
	public bool IsNespolehlivy { get; set; }
	/// Datum zveřejnění jako nespolehlivý plátce. Spolehlivý = MinDate() 
	public DateTime NespolehlivyOd { get; set; }
	/// Nalezené Dic. 
	public string Dic { get; set; }
	/// Klíč pro hledání v číselníku Finanční úřad
	public string CisloFu { get; set; }
	/// Název finančního úřadu z číselníku
	public string NazevFu { get; set; }
	/// List všech účtů registrovaných u FU 
	public List<PlatceDphCisloUctu> CislaUctu { get; set; }
	/// Konstruktor
	public PlatceDphResponse()
	{
		CislaUctu = new List<PlatceDphCisloUctu>();
	}
}
