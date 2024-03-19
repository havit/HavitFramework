namespace Havit.Ares;

public class PlatceDphResponse
{
	public bool IsNalezeno { get; set; }
	public bool IsSpolehlivy { get { return !IsNespolehlivy; } }
	public bool IsNespolehlivy { get; set; }
	public DateTime NespolehlivyOd { get; set; }
	public List<PlatceDphCisloUctu> CislaUctu { get; set; }
	public string Dic { get; set; }
	public string CisloFu { get; set; }
	public string NazevFu { get; set; }
	public string ResponseRaw { get; set; }
	public PlatceDphResponse()
	{
		CislaUctu = new List<PlatceDphCisloUctu>();
	}
}
