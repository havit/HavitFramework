using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Havit.Diagnostics.Contracts;

/// <summary>
/// Exception thrown by the Contract class when a contract condition is not met.
/// </summary>
public sealed class ContractException : Exception
{
	/// <summary>
	/// Constructor.
	/// </summary>
	public ContractException(string message) : base(message)
	{
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	public ContractException(string message, Exception innerException) : base(message, innerException)
	{
	}

	private ContractException()
	{
	}

	private ContractException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
