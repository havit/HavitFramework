using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Havit.Diagnostics.Contracts
{
	/// <summary>
	/// Výjimka vyhazovaná třídou Contract v případě nespnění podmínky contractu.
	/// </summary>
	public sealed class ContractException : Exception
	{
		#region Constructors
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

		private ContractException() : base()
		{
		}

		private ContractException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
		#endregion

	}
}
