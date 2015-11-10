using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Model.Localizations
{
	/// <summary>
	/// Lokalizace jiné třídy.
	/// </summary>
	/// <typeparam name="TLocalizedEntity">Lokalizovaná třída.</typeparam>
	/// <typeparam name="TLanguage">Jazyk.</typeparam>
	public interface ILocalization<out TLocalizedEntity, out TLanguage>
		where TLanguage : ILanguage
	{
		/// <summary>
		/// Objekt, který je lokalizován.
		/// </summary>
		TLocalizedEntity Parent { get; }

		/// <summary>
		/// Jazyk, pro který je objekt lokalizován.
		/// </summary>
		TLanguage Language { get; }
	}
}
