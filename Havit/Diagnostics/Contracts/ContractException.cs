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
		public ContractException(string message) : base(WrapMessage(message))
		{
		}

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public ContractException(string message, Exception innerException) : base(WrapMessage(message), innerException)
		{
		}

		private ContractException()
		{
		}

		private ContractException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
		#endregion

		#region WrapMessage
		/// <summary>
		/// Obalí zprávu textem "Contract failed".
		/// Určeno pro volání v konstruktoru.
		/// </summary>
		private static string WrapMessage(string message)
		{
			if (String.IsNullOrEmpty(message))
			{
				return "Contract failed.";
			}
			else
			{
				return "Contract failed: " + message;
			}
		}
		#endregion
	}
}
