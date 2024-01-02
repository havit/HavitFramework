using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Havit.Diagnostics.Contracts;

/// <summary>
/// Contracts. Methods for ensuring conditions in the code during application runtime.
/// Replaces standard CodeContracts, whose rewriter takes a very long time.
/// Unlike CodeContracts, we do not learn from the error description what the actual error is (the condition is not worded).
/// </summary>
public static class Contract
{
	/// <summary>
	/// If the condition is not met (the value is false), throws a ContractException.
	/// </summary>
	[DebuggerStepThrough]
#if NET6_0_OR_GREATER
	public static void Requires([DoesNotReturnIf(false)] bool condition, [CallerArgumentExpression("condition")] string userMessage = null)
#else
    [JetBrains.Annotations.ContractAnnotation("condition:false => halt")]
    public static void Requires(bool condition, [CallerArgumentExpression("condition")] string userMessage = null)
#endif
	{
		if (!condition)
		{
			ThrowException<ContractException>(ConstructMessage(userMessage));
		}
	}

	/// <summary>
	/// If the condition is not met (the value is false), throws an exception of type TException.
	/// </summary>
	/// <typeparam name="TException">The type of exception that is thrown if the condition is not met.</typeparam>
	[DebuggerStepThrough]
#if NET6_0_OR_GREATER
	public static void Requires<TException>([DoesNotReturnIf(false)] bool condition, [CallerArgumentExpression("condition")] string userMessage = null)
#else
    [JetBrains.Annotations.ContractAnnotation("condition:false => halt")]
    public static void Requires<TException>(bool condition, [CallerArgumentExpression("condition")] string userMessage = null)
#endif
		where TException : Exception
	{
		if (!condition)
		{
			ThrowException<TException>(userMessage);
		}
	}

	/// <summary>
	/// If the condition is not met (the value is false), throws a ContractException.
	/// </summary>
	[DebuggerStepThrough]
#if NET6_0_OR_GREATER
	public static void Assert([DoesNotReturnIf(false)] bool condition, [CallerArgumentExpression("condition")] string userMessage = null)
#else
    [JetBrains.Annotations.ContractAnnotation("condition:false => halt")]
    public static void Assert(bool condition, [CallerArgumentExpression("condition")] string userMessage = null)
#endif
	{
		if (!condition)
		{
			ThrowException<ContractException>(ConstructMessage(userMessage));
		}
	}

	/// <summary>
	/// If the condition is not met (the value is false), throws an exception of type TException.
	/// </summary>
	/// <remarks>
	/// The thrown exception contains a message in the format:<br />
	/// <code>
	/// Contract failed: Message<br />
	/// Member Name: Main<br />
	/// Source File Path: D:\Dev\MySolution\MyProject\Program.cs<br />
	/// Source Line Number: 123<br />
	/// </code>
	/// </remarks>
	/// <typeparam name="TException">The type of exception that is thrown if the condition is not met.</typeparam>
	[DebuggerStepThrough]
#if NET6_0_OR_GREATER
	public static void Assert<TException>([DoesNotReturnIf(false)] bool condition, [CallerArgumentExpression("condition")] string userMessage = null)
#else
    [JetBrains.Annotations.ContractAnnotation("condition:false => halt")]
    public static void Assert<TException>(bool condition, [CallerArgumentExpression("condition")] string userMessage = null)
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
	/// Wraps the message with the text "Contract failed".
	/// </summary>
	private static string ConstructMessage(string message)
	{
		return String.IsNullOrEmpty(message)
			? "Contract failed."
			: "Contract failed: " + message;
	}
}
