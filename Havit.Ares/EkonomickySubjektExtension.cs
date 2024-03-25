using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Ares;

/// Vypočítané hodnoty z ARES
public class EkonomickySubjektExtension
{
	/// Název Rejstříkového soudu. Text z číselníku Rejstříkový soud
	public string RejstrikovySoudText { get; set; }
	/// Spisová značka . Kód + Text z číselníku.  Tak ja má být zapsáno na faktuře 
	public string SpisovaZnackaFull { get; set; }
	/// Název Finančního úřadu. Text z číselníku Finanční úřad
	public string FinancniUradText { get; set; }
	/// Právní forma společnosti (OSVČ, s.r.o., a.s., s.p.). Text z číselníku Právní forma
	public string PravniFormaText { get; set; }
	/// PSČ s mezerou (6 znaků).  
	public string SidloPscText { get; set; }
	/// řádky adresy Sídla (3 řádky). Podobně jako Doručovací adresa. 
	public string[] SidloAddressLines { get; set; }
	/// Výsledek porovnání adresy Sídla a Doručovací adresy
	public bool IsDorucovaciAdresaStejna { get; set; }
	/// Zdali se subjekt nachází v rejstříku DPH 
	public bool IsPlatceDph { get; set; }
}