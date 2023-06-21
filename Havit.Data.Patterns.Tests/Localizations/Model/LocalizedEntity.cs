using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Model.Localizations;

namespace Havit.Data.Patterns.Tests.Localizations.Model;

    public class LocalizedEntity : ILocalized<LocalizedEntityLocalization>
{
	public int Id { get; set; }

	public List<LocalizedEntityLocalization> Localizations { get; set; }
}
