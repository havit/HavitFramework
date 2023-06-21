using Havit.Model.Localizations;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.TestsInfrastructure;

public class Language : ILanguage
{
	public int Id { get; set; }

	public string Culture { get; set; }

	public string UiCulture { get; set; }
}
