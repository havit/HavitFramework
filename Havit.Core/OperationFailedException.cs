using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit;

/// <summary>
/// Exception for recoverable failures which should be presented to the user.
/// </summary>
/// <remarks>
/// This exception is handled by our new UI stacks and is usually displayed as Messenger error.
/// </remarks>
public class OperationFailedException : Exception
{
	/// <summary>
	/// Initializes a new instance of the OperationFailedException class with a specified error message.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	public OperationFailedException(string message) : base(message)
	{
		// NOOP
	}

	/// <summary>
	/// Initializes a new instance of the OperationFailedException class with a specified error message and a reference to the inner exception that is the cause of this exception.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	/// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
	public OperationFailedException(string message, Exception innerException) : base(message, innerException)
	{
		// NOOP
	}
}
