using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.TransientErrorHandling;

/// <summary>
/// Provádí akci s opakováním v případě selhání.
/// </summary>
internal static class TransientErrorHandler
{
	/// <summary>
	/// Provádí danou akci, opakování se řídí výchozí TransientErrorRetryPolicy.
	/// </summary>
	public static TResult ExecuteAction<TResult>(Func<TResult> action, Func<bool> actionShouldRetry)
	{
		return ExecuteAction(action, actionShouldRetry, new TransientErrorRetryPolicy());
	}

	/// <summary>
	/// Provádí danou akci, opakování se řídí předanou danou RetryPolicy.
	/// </summary>
	public static TResult ExecuteAction<TResult>(Func<TResult> action, Func<bool> actionShouldRetry, IRetryPolicy retryPolicy)
	{
		int attemptNumber = 0;
		while (true)
		{
			try
			{
				attemptNumber += 1;
                    return action();
			}
			catch (Exception exception)
			{
				RetryPolicyInfo retryPolicyInfo = retryPolicy.GetRetryPolicyInfo(attemptNumber, exception);

				if (!actionShouldRetry() || !retryPolicyInfo.RetryAttempt)
				{
					throw;
				}

				if (retryPolicyInfo.DelayBeforeRetry > 0)
				{
					Task.Delay(retryPolicyInfo.DelayBeforeRetry).Wait();
				}
			}
		}
	}
}
