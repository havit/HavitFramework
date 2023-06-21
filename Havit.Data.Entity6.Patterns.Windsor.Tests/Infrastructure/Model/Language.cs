using Havit.Model.Localizations;

namespace Havit.Data.Entity6.Patterns.Windsor.Tests.Infrastructure.Model;

public class Language : ILanguage
{
	public int Id { get; set; }

	public string Culture { get; }

	public string UiCulture { get; }
}
