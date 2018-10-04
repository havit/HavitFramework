using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Model.Localizations;

namespace Havit.Data.Patterns.Tests.Localizations.Model
{
	public interface ILocalized<TLocalizationEntity> : ILocalized<TLocalizationEntity, Language>
	{
		new List<TLocalizationEntity> Localizations { get; set; }
	}
}
