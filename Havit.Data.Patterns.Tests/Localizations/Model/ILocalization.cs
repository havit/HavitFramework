using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Model.Localizations;

namespace Havit.Data.Patterns.Tests.Localizations.Model;

public interface ILocalization<TLocalizedEntity> : ILocalization<TLocalizedEntity, Language>
{
	new TLocalizedEntity Parent { get; set; }
	int ParentId { get; set; }

	new Language Language { get; set; }
	int LanguageId { get; set; }
}
