using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Havit.Diagnostics.Contracts;

/// <summary>
/// Výjimka vyhazovaná třídou Contract v případě nespnění podmínky contractu.
/// </summary>
public sealed class ContractException : Exception
{
	/// <summary>
	/// Konstructor.
	/// </summary>
	public ContractException(string message) : base(message)
	{
	}

	/// <summary>
	/// Konstruktor.
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
