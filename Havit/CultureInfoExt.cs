using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Havit
{
	/// <summary>
	/// Rozšiřující metody pro CultureInfo.
	/// </summary>
	public static class CultureInfoExt
	{
		/// <summary>
		/// Vykoná metodu v kontextu s culture. Vrací návratovou hodnotu metody.
		/// </summary>
		/// <remarks>
		/// Culture je pro aktuální thread po vykonání metody obnovena.
		/// </remarks>
		/// <param name="culture">CultureInfo v jehož kontextu chceme, aby se metoda vykonala.</param>
		/// <param name="methodDelegate">Metoda, která se vykoná v kontextu CultureInfo. Návratová hodnota předané metody je vrácena touto metodou.</param>
		public static TResult ExecuteMethod<TResult>(this CultureInfo culture, Func<TResult> methodDelegate)
		{
			CultureInfo oldCulture = Thread.CurrentThread.CurrentCulture;
			CultureInfo oldUICulture = Thread.CurrentThread.CurrentUICulture;
			TResult result;

			try
			{
				Thread.CurrentThread.CurrentCulture = culture;
				Thread.CurrentThread.CurrentUICulture = culture;

				result = methodDelegate();
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = oldCulture;
				Thread.CurrentThread.CurrentUICulture = oldUICulture;
			}

			return result;
		}

		/// <summary>
		/// Vykoná metodu v kontextu s culture.
		/// </summary>
		/// <remarks>
		/// Culture je pro aktuální thread po vykonání metody obnovena.
		/// </remarks>
		/// <param name="culture">CultureInfo v jehož kontextu chceme aby se metoda vykonala.</param>
		/// <param name="methodDelegate">Metoda, která se vykoná v kontextu CultureInfo.</param>
		public static void ExecuteMethod(this CultureInfo culture, Action methodDelegate)
		{
			ExecuteMethod<object>(culture, () =>
			{
				methodDelegate();
				return null;
			});
		}
	}
}
