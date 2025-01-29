namespace Havit.Data.EntityFrameworkCore.Threading;

/// <summary>
/// The exception that is thrown when an attempt is made to lock on SQL server resource, but the attempt was not successful.
/// </summary>
[Serializable]
public class DbLockedCriticalSectionException : Exception
{
	/// <summary>
	/// Constructor.
	/// </summary>
	public DbLockedCriticalSectionException(string message) : base(message)
	{
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	public DbLockedCriticalSectionException(string message, Exception innerException) : base(message, innerException)
	{
	}
}