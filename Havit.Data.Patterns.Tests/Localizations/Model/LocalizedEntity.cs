namespace Havit.Data.Patterns.Tests.Localizations.Model;

public class LocalizedEntity : ILocalized<LocalizedEntityLocalization>
{
	public int Id { get; set; }

	public List<LocalizedEntityLocalization> Localizations { get; set; }
}
