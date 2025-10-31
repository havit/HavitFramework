using System.Runtime.Serialization;

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
}
