using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit
{
	/// <summary>
	/// Exception for recoverable failures which should be presented to the user.
	/// </summary>
	/// <remarks>
	/// This exception is handled by our new UI stacks and is usualy displayed ase Messenger error.
	/// </remarks>
	public class OperationFailedException : Exception
    {
		/// <inheritdoc/>
		public OperationFailedException(string message) : base(message)
		{
			// NOOP
		}

		/// <inheritdoc/>
		public OperationFailedException(string message, Exception innerException) : base(message, innerException)
		{
			// NOOP
		}
	}
}
