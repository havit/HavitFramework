namespace Havit.Ares.Ares;

/// <summary>
/// Vypočítané hodnoty z ARES
/// </summary>
public class EkonomickySubjektExtension
{
	/// <summary>
	/// Název Rejstříkového soudu. Text z číselníku Rejstříkový soud
	/// </summary>
	public string RejstrikovySoudText { get; set; }

	/// <summary>
	/// Spisová značka . Kód + Text z číselníku.  Tak ja má být zapsáno na faktuře 
	/// </summary>
	public string SpisovaZnackaFull { get; set; }

	/// <summary>
	/// Název Finančního úřadu. Text z číselníku Finanční úřad
	/// </summary>
	public string FinancniUradText { get; set; }

	/// <summary>
	/// Právní forma společnosti (OSVČ, s.r.o., a.s., s.p.). Text z číselníku Právní forma
	/// </summary>
	public string PravniFormaText { get; set; }

	/// <summary>
	/// PSČ s mezerou (6 znaků).  
	/// </summary>
	public string SidloPscText { get; set; }

	/// <summary>
	/// řádky adresy Sídla (3 řádky). Podobně jako Doručovací adresa. 
	/// </summary>
	public string[] SidloAddressLines { get; set; }

	/// <summary>
	/// Výsledek porovnání adresy Sídla a Doručovací adresy
	/// </summary>
	public bool IsDorucovaciAdresaStejna { get; set; }

	/// <summary>
	/// Zdali se subjekt nachází v rejstříku DPH 
	/// </summary>
	public bool IsPlatceDph { get; set; }
}