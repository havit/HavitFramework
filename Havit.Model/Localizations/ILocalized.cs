using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Model.Localizations
{
	/// <summary>
	/// Lokalizovaný objekt.
	/// </summary>
	/// <typeparam name="TLocalizationEntity">Třída lokalizující objekt.</typeparam>
	/// <typeparam name="TLanguage">Jazyk.</typeparam>
	public interface ILocalized<TLocalizationEntity, out TLanguage>
		where TLanguage : ILanguage
	{
		/// <summary>
		/// Kolekce lokalizací objektu.
		/// </summary>
		List<TLocalizationEntity> Localizations { get; }
	}
}
