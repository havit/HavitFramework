using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
		/// <summary>
		/// Pokud není podmínka condition splněna (hodnota je false), vyhodí výjimku ContractException.
		/// </summary>
		[DebuggerStepThrough]
#if NET6_0_OR_GREATER
		public static void Requires([DoesNotReturnIf(false)] bool condition, [CallerArgumentExpression("condition")] string userMessage = null)
#else
		[JetBrains.Annotations.ContractAnnotation("condition:false => halt")]
		public static void Requires(bool condition, string userMessage = null)
#endif
		{			
			if (!condition)
			{
				ThrowException<ContractException>(ConstructMessage(userMessage));
			}
		}

		/// <summary>
		/// Pokud není podmínka condition splněna (hodnota je false), vyhodí výjimku TException.
		/// </summary>
		/// <typeparam name="TException">Typ výjimky, která je v případě nesplnění podmínky vyhozena.</typeparam>
		[DebuggerStepThrough]
#if NET6_0_OR_GREATER
		public static void Requires<TException>([DoesNotReturnIf(false)] bool condition, [CallerArgumentExpression("condition")] string userMessage = null)
#else
		[JetBrains.Annotations.ContractAnnotation("condition:false => halt")]
		public static void Requires<TException>(bool condition, string userMessage = null)
#endif
			where TException : Exception
		{
			if (!condition)
			{
				ThrowException<TException>(userMessage);
			}
		}

		/// <summary>
		/// Pokud není podmínka condition splněna (hodnota je false), vyhodí výjimku ContractException.
		/// </summary>
		[DebuggerStepThrough]
#if NET6_0_OR_GREATER
		public static void Assert([DoesNotReturnIf(false)] bool condition, [CallerArgumentExpression("condition")] string userMessage = null)
#else
		[JetBrains.Annotations.ContractAnnotation("condition:false => halt")]
		public static void Assert(bool condition, string userMessage = null)
#endif
		{
			if (!condition)
			{
				ThrowException<ContractException>(ConstructMessage(userMessage));
			}
		}

		/// <summary>
		/// Pokud není podmínka condition splněna (hodnota je false), vyhodí výjimku TException.
		/// </summary>
		/// <remarks>
		/// Vyhozená výjimka obsahuje zprávu ve formátu:<br />
		/// <code>
		/// Contract failed: Message<br />
		/// Member Name: Main<br />
		///	Source File Path: D:\Dev\MySolution\MyProject\Program.cs<br />
		/// Source Line Number: 123<br />
		/// </code>
		/// </remarks>
		/// <typeparam name="TException">Typ výjimky, která je v případě nesplnění podmínky vyhozena.</typeparam>
		[DebuggerStepThrough]
#if NET6_0_OR_GREATER
		public static void Assert<TException>([DoesNotReturnIf(false)] bool condition, [CallerArgumentExpression("condition")] string userMessage = null)
#else
		[JetBrains.Annotations.ContractAnnotation("condition:false => halt")]
		public static void Assert<TException>(bool condition, string userMessage = null)
#endif
			where TException : Exception
		{
			if (!condition)
			{
				ThrowException<TException>(userMessage);
			}
		}

		[DebuggerStepThrough]		
		private static void ThrowException<TException>(string message)
			where TException : Exception
		{
			Exception resultException;
			try
			{
				if (typeof(TException) == typeof(ArgumentNullException))
				{
					resultException = new ArgumentNullException(null, message);
				}
				else
				{
					resultException = (Exception)Activator.CreateInstance(typeof(TException), message);
				}

			}
			catch
			{
				resultException = new ContractException(message);
			}

			throw resultException;
		}

		/// <summary>
		/// Obalí zprávu textem "Contract failed".
		/// </summary>
		private static string ConstructMessage(string message)
		{
			return String.IsNullOrEmpty(message) 
				? "Contract failed."
				: "Contract failed: " + message;
		}
	}
}
