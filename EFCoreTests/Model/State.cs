using Havit.Model.Localizations;

namespace Havit.EFCoreTests.Model;

public class State : ILocalized<StateLocalization, Language>
{
	public int Id { get; set; }

	public List<StateLocalization> Localizations { get; set; }
}
