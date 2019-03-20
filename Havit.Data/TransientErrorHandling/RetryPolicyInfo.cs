using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.TransientErrorHandling
{
	/// <summary>
	/// Informace o záměru opakovat akci, jež selhala.
	/// </summary>
	internal class RetryPolicyInfo
	{
		/// <summary>
		/// Indikuje, zda má být pokus opakován.
		/// </summary>
		public bool RetryAttempt { get; set; }

		/// <summary>
		/// Indikuje čas čekání před dalším pokusem.
		/// </summary>
		public int DelayBeforeRetry { get; set; }
	}
}
