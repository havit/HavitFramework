using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

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
	public static void Requires<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TException>([DoesNotReturnIf(false)] bool condition, [CallerArgumentExpression("condition")] string userMessage = null)
#else
	[JetBrains.Annotations.ContractAnnotation("condition:false => halt")]
	public static void Requires<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TException>(bool condition, [CallerArgumentExpression("condition")] string userMessage = null)
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
	public static void Assert<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TException>([DoesNotReturnIf(false)] bool condition, [CallerArgumentExpression("condition")] string userMessage = null)
#else
	[JetBrains.Annotations.ContractAnnotation("condition:false => halt")]
	public static void Assert<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TException>(bool condition, [CallerArgumentExpression("condition")] string userMessage = null)
#endif
		where TException : Exception
	{
		if (!condition)
		{
			ThrowException<TException>(userMessage);
		}
	}

	// [DynamicallyAccessedMembers(PublicConstructors)] keeps the constructors of TException when trimming.
	// The exception is constructed here through reflection (Activator.CreateInstance below) and nowhere else statically,
	// so the trimmer would remove its (string) constructor. The resulting MissingMethodException would be swallowed by the catch below
	// and the caller would silently get a ContractException instead of the expected TException.
	// The annotation must be repeated on Requires<TException>/Assert<TException> - the requirement has to flow through the whole call chain.
	[DebuggerStepThrough]
	private static void ThrowException<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TException>(string message)
		where TException : Exception
	{
		Exception resultException;
		try
		{
			if (typeof(TException) == typeof(ArgumentNullException))
			{
				resultException = new ArgumentNullException(paramName: null, message);
			}
			else if (typeof(TException) == typeof(ArgumentOutOfRangeException))
			{
				// the single-string constructor is (string paramName), the message would end up as ParamName
				resultException = new ArgumentOutOfRangeException(paramName: null, message);
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
