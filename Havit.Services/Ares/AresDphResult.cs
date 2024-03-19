
namespace Havit.Services.Ares;
// Zpracovane vysledky ze dvou volani ARES + nespolehlivyPlatce (z MFCR)
// Pokud je vyplněno datum zániku - firma neexistuje. Pokud v textu je 'k likvidaci', jedná se o podnik v insolvenci (jak je to u OSVC nevím)
// CislaUctu - seznam účtů kam lze faktury platit (zpravidla jeden, ale takový O2 jich má 19)

public class AresDphResponse
{
	public EkonomickySubjekt AresElement { get; set; }
	public PlatceDphResponse PlatceDphElement { get; set; }
}
