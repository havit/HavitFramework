using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Havit;

/// <summary>
/// Extension methods for CultureInfo.
/// </summary>
public static class CultureInfoExt
{
	/// <summary>
	/// Executes a method in the context of a culture. Returns the method's return value.
	/// </summary>
	/// <remarks>
	/// The culture is restored for the current thread after the method is executed.
	/// </remarks>
	/// <param name="culture">The CultureInfo in whose context we want the method to be executed.</param>
	/// <param name="methodDelegate">The method to be executed in the context of the CultureInfo. The return value of the passed method is returned by this method.</param>
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
	/// Executes a method in the context of a culture.
	/// </summary>
	/// <remarks>
	/// The culture is restored for the current thread after the method is executed.
	/// </remarks>
	/// <param name="culture">The CultureInfo in whose context we want the method to be executed.</param>
	/// <param name="methodDelegate">The method to be executed in the context of the CultureInfo.</param>
	public static void ExecuteMethod(this CultureInfo culture, Action methodDelegate)
	{
		ExecuteMethod<object>(culture, () =>
		{
			methodDelegate();
			return null;
		});
	}
}
