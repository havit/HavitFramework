using System.Globalization;

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
		using (EnterScope(culture))
		{
			return methodDelegate();
		}
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
		using (EnterScope(culture))
		{
			methodDelegate();
		}
	}

	/// <summary>
	/// Enters the scope in which the current thread's culture is set to the specified culture.
	/// Scope is exited by calling the returned IDisposable instance, usually in a using statement.
	/// </summary>
	/// <param name="cultureInfo">The CultureInfo for the scope.</param>
	/// <returns>IDisposable instance returning the original CultureInfos</returns>
	public static IDisposable EnterScope(CultureInfo cultureInfo)
	{
		// at first let's store the original culture infos
		var disposable = new CultureInfoHelperDisposeAction(Thread.CurrentThread.CurrentCulture, Thread.CurrentThread.CurrentUICulture);

		// and then set the new culture infos
		Thread.CurrentThread.CurrentCulture = cultureInfo;
		Thread.CurrentThread.CurrentUICulture = cultureInfo;

		return disposable;
	}

	private class CultureInfoHelperDisposeAction(CultureInfo _originalCulture, CultureInfo _originalUICulture) : IDisposable
	{
		public void Dispose()
		{
			Thread.CurrentThread.CurrentCulture = _originalCulture;
			Thread.CurrentThread.CurrentUICulture = _originalUICulture;
		}
	}
}
