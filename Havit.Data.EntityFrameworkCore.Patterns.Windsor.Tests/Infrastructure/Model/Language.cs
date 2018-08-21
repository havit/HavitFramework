using Havit.Model.Localizations;

namespace Havit.Data.EntityFrameworkCore.Patterns.Windsor.Tests.Infrastructure.Model
{
	public class Language : ILanguage
	{
		public int Id { get; set; }

		public string Culture { get; }

		public string UiCulture { get; }
	}
}
