using System.Runtime.InteropServices;
using Havit.Model.Localizations;

namespace Havit.Data.Patterns.Tests.Localizations.Model
{
	public class LocalizedEntityLocalization : ILocalization<LocalizedEntity>
	{
		public int Id { get; set; }

		public LocalizedEntity Parent { get; set; }
		public int ParentId { get; set; }

		public Language Language { get; set; }
		public int LanguageId { get; set; }
	}
}