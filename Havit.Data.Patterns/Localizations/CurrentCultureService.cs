using System.Globalization;

namespace Havit.Data.Patterns.Localizations
{
	/// <summary>
	/// Služba pro poskytnutí aktuálního Culture/UICulture. Získává hodnotu z System.Globalization.CultureInfo.CurrentCulture/CurrentUICulture.
	/// </summary>
	public class CurrentCultureService : ICurrentCultureService
	{
		/// <summary>
		/// Vrací aktuální Culture.
		/// </summary>
		public CultureInfo GetCurrentCulture()
		{
			return System.Globalization.CultureInfo.CurrentCulture;
		}

		/// <summary>
		/// Vrací aktuální UICulture.
		/// </summary>
		public CultureInfo GetCurrentUICulture()
		{
			return System.Globalization.CultureInfo.CurrentUICulture;
		}
	}
}
