using Havit.Data.EntityFrameworkCore.Attributes;
using Havit.EFCoreTests.Model.Localizations;
using Havit.Model.Localizations;

namespace Havit.EFCoreTests.Model
{
	[Cache]
	public class StavLocalization : ILocalization<Stav>
	{
		public int Id { get; set; }

		public Stav Parent { get; set; }
		public int ParentId { get; set; }
		public Language Language { get; set; }
		public int LanguageId { get; set; }

		// ...
	}
}