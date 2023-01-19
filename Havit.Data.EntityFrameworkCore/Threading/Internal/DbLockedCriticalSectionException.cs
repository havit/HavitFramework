using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Threading.Internal
{
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

		/// <summary>
		/// Constructor.
		/// </summary>
		protected DbLockedCriticalSectionException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext) : base(serializationInfo, streamingContext)
		{
		}
	}
}