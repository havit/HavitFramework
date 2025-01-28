using Havit.Model.Localizations;

namespace Havit.EFCoreTests.Model;

public class Language : ILanguage
{
	public int Id { get; set; }

	public string Culture { get; set; }

	public string UiCulture { get; set; }

	public string Symbol { get; set; }

	public enum Entry
	{
		Czech,
		English
	}
}


