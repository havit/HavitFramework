using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Threading
{
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
        /// </summary>
        public int SqlCommandTimeoutSeconds { get; set; } = 5;

        /// <summary>
        /// Is a lock time-out value in milliseconds. The default value is the same as the value returned by @@LOCK_TIMEOUT.
        /// To indicate that a lock request should return a Return Code of -1 instead of wait for the lock when the request cannot be granted immediately, specify 0.
        /// Default is -1.
        /// </summary>
        public int LockTimeoutMs { get; set; } = -1;
    }
}
