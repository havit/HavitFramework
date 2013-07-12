using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Havit.Diagnostics.Contracts
{
	/// <summary>
	/// Contracts. Metody pro zajištění podmínek v kódu za běhu aplikace.
	/// Nahrazuje standardní CodeContracts, jejichž rewriter trvá velmi dlouho.
	/// Na rozdíl od CodeContracs se z popisu chyby nedozvíme, o jakou chybu skutečně jde (není znění podmínky).
	/// </summary>
	public static class Contract
	{
		#region Requires
		/// <summary>
		/// Pokud není podmínka condition splněna (hodnota je false), vyhodí výjimku ContractException.
		/// </summary>
		[DebuggerStepThrough]
		public static void Requires(bool condition, string userMessage = null)
		{
			if (!condition)
			{
				ThrowException<ContractException>(userMessage);
			}
		}

		/// <summary>
		/// Pokud není podmínka condition splněna (hodnota je false), vyhodí výjimku TException.
		/// </summary>
		/// <typeparam name="TException">Typ výjimky, která je v případě nesplnění podmínky vyhozena.</typeparam>
		[DebuggerStepThrough]
		public static void Requires<TException>(bool condition, string userMessage = null)
			where TException : Exception
		{
			if (!condition)
			{
				ThrowException<TException>(userMessage);
			}
		}
		#endregion

		#region Assert
		/// <summary>
		/// Pokud není podmínka condition splněna (hodnota je false), vyhodí výjimku ContractException.
		/// </summary>
		[DebuggerStepThrough]
		public static void Assert(bool condition, string userMessage = null)
		{
			if (!condition)
			{
				ThrowException<ContractException>(userMessage);
			}
		}
		#endregion

		#region ThrowException
		[DebuggerStepThrough]
		private static void ThrowException<TException>(string message)
			where TException : Exception
		{
			string exceptionMessage = String.IsNullOrEmpty(message) ? "Contract failed." : "Contract failed: " + message;
			Exception resultException;
			try
			{
				resultException = (Exception)Activator.CreateInstance(typeof(TException), exceptionMessage);
			}
			catch
			{
				resultException = new ContractException(exceptionMessage);
			}

			throw resultException;
		}
		#endregion
	}
}
