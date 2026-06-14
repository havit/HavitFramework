namespace Havit.Data.Patterns.Tests.DataSeeds.Infrastructure;

/// <summary>
/// Bázová entita pro testy dědičnosti při seedování (scénář Zvíře ← Pes, Kočka).
/// Záměrně není abstraktní, aby šlo testovat i seedování instancí přímo bázového typu.
/// </summary>
public class Animal
{
	public int Id { get; set; }

	public string Symbol { get; set; }
}
