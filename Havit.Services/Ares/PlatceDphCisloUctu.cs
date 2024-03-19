
namespace Havit.Services.Ares;

public class PlatceDphCisloUctu
{
	public DateTime DatumZverejneni { get; set; }
	public DateTime DatumUkonceni { get; set; }
	public string Cislo { get; set; }
	public string Predcisli { get; set; }
	public string KodBanky { get; set; }
	public string CisloFull { get { return (Predcisli == null || Predcisli == "" ? "" : Predcisli + "-") + $"{Cislo}/{KodBanky}"; } }
}
