using Havit.Model.Localizations;

namespace Havit.EFCoreTests.Model;

public class StateLocalization : ILocalization<State, Language>
{
	public int Id { get; set; }

	public State Parent { get; set; }
	public int ParentId { get; set; }

	public Language Language { get; set; }
	public int LanguageId { get; set; }
}
