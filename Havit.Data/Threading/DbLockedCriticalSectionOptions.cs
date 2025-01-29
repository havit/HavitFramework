namespace Havit.Data.Threading;

/// <summary>
/// Options for database-locked critical section.
/// </summary>
public class DbLockedCriticalSectionOptions
{
	/// <summary>
	/// Gets or sets the string used to open the connection.
	/// </summary>
	public string ConnectionString { get; set; }

	/// <summary>
	/// Gets or sets the wait time before terminating the attempt to execute SQL command and generating an error. Default is 5 seconds.
	/// A value of 0 indicates no limit (an attempt to execute a command will wait indefinitely).
	/// </summary>
	public int SqlCommandTimeoutSeconds { get; set; } = 5;

	/// <summary>
	/// Is a lock time-out value in milliseconds. The default value is the same as the value returned by @@LOCK_TIMEOUT.
	/// To indicate that a lock request should return a Return Code of -1 instead of wait for the lock when the request cannot be granted immediately, specify 0.
	/// Default is -1. 
	/// Possible values: -1 means wait forever and never expire, 0 means without wait, more than 0 is time out interval in ms.
	/// I recommend set SqlCommandTimeoutSeconds bigger than LockTimeoutMs, because it is more clean, when lock timeout is runned by lockTimeoutMs and not by sql command timeout.
	/// Recommended values:
	/// LockTimeoutMs = -1 and SqlCommandTimeoutSeconds = 0: neverending lock timeout
	/// LockTimeoutMs = 0 and SqlCommandTimeoutSeconds = 10: lock timeout immediatelly 
	/// LockTimeoutMs = 60000 and SqlCommandTimeoutSeconds = 70: lock timeout is 60 sec
	/// </summary>
	public int LockTimeoutMs { get; set; } = -1;
}
