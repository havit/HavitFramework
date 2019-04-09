using Havit.EFCoreTests.Model.Localizations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.EFCoreTests.Model
{
	public class Stav : ILocalized<StavLocalization>
	{
		public int Id { get; set; }
		public List<StavLocalization> Localizations { get; } = new List<StavLocalization>();
	}
}
