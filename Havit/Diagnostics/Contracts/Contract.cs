using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;

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
		/// <remarks>
		/// Vyhozená výjimka je typu ContractException. Obsahuje zprávu ve formátu:<br />
		/// <code>
		/// Contract failed: Message<br />
		/// Member Name: Main<br />
		///	Source File Path: D:\Dev\MySolution\MyProject\Program.cs<br />
		/// Source Line Number: 123<br />
		/// </code>
		/// </remarks>
		[DebuggerStepThrough]
		[ContractAnnotation("condition:false => halt")]
		public static void Requires(bool condition, string userMessage = null, [CallerMemberName] string memberName = null, [CallerFilePath] string sourceFilePath = null, [CallerLineNumber] int sourceLineNumber = Int32.MinValue)
		{
			if (!condition)
			{
				ThrowException<ContractException>(ConstructMessage(userMessage, memberName, sourceFilePath, sourceLineNumber));
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
		[ContractAnnotation("condition:false => halt")]
		public static void Requires<TException>(bool condition, string userMessage = null, [CallerMemberName] string memberName = null, [CallerFilePath] string sourceFilePath = null, [CallerLineNumber] int sourceLineNumber = Int32.MinValue)
			where TException : Exception
		{
			if (!condition)
			{
				ThrowException<TException>(ConstructMessage(userMessage, memberName, sourceFilePath, sourceLineNumber));
			}
		}

		/// <summary>
		/// Pokud není podmínka condition splněna (hodnota je false), vyhodí výjimku ContractException.
		/// </summary>
		/// Vyhozená výjimka je typu ContractException. Obsahuje zprávu ve formátu:<br />
		/// <code>
		/// Contract failed: Message<br />
		/// Member Name: Main<br />
		///	Source File Path: D:\Dev\MySolution\MyProject\Program.cs<br />
		/// Source Line Number: 123<br />
		/// </code>
		[DebuggerStepThrough]
		[ContractAnnotation("condition:false => halt")]
		public static void Assert(bool condition, string userMessage = null, [CallerMemberName] string memberName = null, [CallerFilePath] string sourceFilePath = null, [CallerLineNumber] int sourceLineNumber = Int32.MinValue)
		{
			if (!condition)
			{
				ThrowException<ContractException>(ConstructMessage(userMessage, memberName, sourceFilePath, sourceLineNumber));
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
		[ContractAnnotation("condition:false => halt")]
		public static void Assert<TException>(bool condition, string userMessage = null, [CallerMemberName] string memberName = null, [CallerFilePath] string sourceFilePath = null, [CallerLineNumber] int sourceLineNumber = Int32.MinValue)
			where TException : Exception
		{
			if (!condition)
			{
				ThrowException<TException>(ConstructMessage(userMessage, memberName, sourceFilePath, sourceLineNumber));
			}
		}

		[DebuggerStepThrough]		
		private static void ThrowException<TException>(string message)
			where TException : Exception
		{
			Exception resultException;
			try
			{
				resultException = (Exception)Activator.CreateInstance(typeof(TException), message);
			}
			catch
			{
				resultException = new ContractException(message);
			}

			throw resultException;
		}

		/// <summary>
		/// Obalí zprávu textem "Contract failed".
		/// Určeno pro volání v konstruktoru.
		/// </summary>
		private static string ConstructMessage(string message, string memberName, string sourceFilePath, int sourceLineNumber)
		{
			string contractFailedMessage = String.IsNullOrEmpty(message) 
				? "Contract failed."
				: "Contract failed: " + message;

			return  $@"{contractFailedMessage}
Member Name: {memberName}
Source File Path: {sourceFilePath}
Source Line Number: {sourceLineNumber}";
		}

	}
}
