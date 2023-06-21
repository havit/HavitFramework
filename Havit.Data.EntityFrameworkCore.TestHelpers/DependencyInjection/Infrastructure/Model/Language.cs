using Havit.Model.Localizations;

namespace Havit.Data.EntityFrameworkCore.TestHelpers.DependencyInjection.Infrastructure.Model;

public class Language : ILanguage
{
	public int Id { get; set; }

	public string Culture { get; }

	public string UiCulture { get; }
}
