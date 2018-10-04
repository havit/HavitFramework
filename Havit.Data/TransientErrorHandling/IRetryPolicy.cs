using System;

namespace Havit.Data.TransientErrorHandling
{
	/// <summary>
	/// Získává informaci o tom, zda má být pokus o provedení dané akce v pøípadì neúspìchu opakován.
	/// </summary>
	internal interface IRetryPolicy
	{
		/// <summary>
		/// Vrací informaci o tom, jestli má být proveden další pokus a s jakým odstupem.
		/// </summary>
		RetryPolicyInfo GetRetryPolicyInfo(int attemptNumber, Exception exception);
	}
}