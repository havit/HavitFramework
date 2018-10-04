using System.Globalization;

namespace Havit.Data.Patterns.Localizations
{
	/// <summary>
	/// Služba pro poskytnutí aktuální Culture/UICulture.
	/// </summary>
	public interface ICurrentCultureService
	{
		/// <summary>
		/// Vrací aktuální Culture.
		/// </summary>
		CultureInfo GetCurrentCulture();

		/// <summary>
		/// Vrací aktuální UICulture.
		/// </summary>
		// ReSharper disable once InconsistentNaming
		CultureInfo GetCurrentUICulture();
	}
}
