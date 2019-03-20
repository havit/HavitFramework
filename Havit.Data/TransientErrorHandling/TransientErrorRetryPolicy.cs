using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.TransientErrorHandling
{
	/// <summary>
	/// Získává informaci o tom, zda má být pokus o provedení dané akce v případě neúspěchu opakován.
	/// Opakovány jsou SQL transientní chyby až do počtu pokusů dle parametru v konstruktoru.
	/// </summary>
	internal class TransientErrorRetryPolicy : IRetryPolicy
	{
		private readonly int maxAttempts;
		private readonly int[] delays;

		/// <summary>
		/// Konstruktor pro výchozí chování - 3 pokusy, druhý a třetí pokus jsou s odstupem 10 sekund.
		/// </summary>
		public TransientErrorRetryPolicy() : this(3, new int[] { 10000 })
		{
		}

		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="maxAttempts">Maximální počet provedených pokusů.</param>
		/// <param name="delays">Odstupy mezi jednotlivými pokusy v milisekundách. Pole nesmí být prázdné.</param>
		public TransientErrorRetryPolicy(int maxAttempts, int[] delays)
		{
			Contract.Requires(delays == null || delays.Length > 0);

			this.maxAttempts = maxAttempts;
			this.delays = delays ?? new int[] { 0 };
		}

		/// <summary>
		/// Vrací informaci o tom, jestli má být proveden další pokus a s jakým odstupem.
		/// </summary>
		public virtual RetryPolicyInfo GetRetryPolicyInfo(int attemptNumber, Exception exception)
		{
			RetryPolicyInfo result = new RetryPolicyInfo
			{
				RetryAttempt = (attemptNumber < maxAttempts) && SqlDatabaseTransientErrorDetectionStrategy.IsTransient(exception),
				DelayBeforeRetry = delays[Math.Min(attemptNumber - 1, delays.Length - 1)]
			};

			return result;
		}
	}
}
