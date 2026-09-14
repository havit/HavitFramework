// Polyfill of the trimming annotations for the target frameworks whose BCL does not contain them (net48, netstandard2.0).
// The attributes are pure markers - they are consumed by the trimmer/ILLink analyzer only when the library is used
// from a .NET 6.0+ application, so the net48/netstandard2.0 build just needs the types to exist
// (the same approach as CallerArgumentExpressionAttribute or IsExternalInit).
// Must NOT be compiled for net6.0+ - there the types from the BCL are used (the analyzer recognizes only those).

#if !NET6_0_OR_GREATER
namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Specifies the types of members that are dynamically accessed.
/// </summary>
[Flags]
internal enum DynamicallyAccessedMemberTypes
{
	None = 0,
	PublicParameterlessConstructor = 0x0001,
	PublicConstructors = 0x0003,
	NonPublicConstructors = 0x0004,
	PublicMethods = 0x0008,
	NonPublicMethods = 0x0010,
	PublicFields = 0x0020,
	NonPublicFields = 0x0040,
	PublicNestedTypes = 0x0080,
	NonPublicNestedTypes = 0x0100,
	PublicProperties = 0x0200,
	NonPublicProperties = 0x0400,
	PublicEvents = 0x0800,
	NonPublicEvents = 0x1000,
	Interfaces = 0x2000,
	All = ~None
}

/// <summary>
/// Indicates that certain members on a specified <see cref="Type" /> are accessed dynamically (e.g. through reflection).
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, Inherited = false)]
internal sealed class DynamicallyAccessedMembersAttribute : Attribute
{
	public DynamicallyAccessedMembersAttribute(DynamicallyAccessedMemberTypes memberTypes)
	{
		MemberTypes = memberTypes;
	}

	public DynamicallyAccessedMemberTypes MemberTypes { get; }
}

/// <summary>
/// Indicates that the specified method requires dynamic access to code that is not referenced statically (e.g. through reflection).
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class, Inherited = false)]
internal sealed class RequiresUnreferencedCodeAttribute : Attribute
{
	public RequiresUnreferencedCodeAttribute(string message)
	{
		Message = message;
	}

	public string Message { get; }

	public string Url { get; set; }
}

/// <summary>
/// Suppresses reporting of a specific code analysis rule violation, allowing multiple suppressions on a single code artifact.
/// </summary>
[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
internal sealed class UnconditionalSuppressMessageAttribute : Attribute
{
	public UnconditionalSuppressMessageAttribute(string category, string checkId)
	{
		Category = category;
		CheckId = checkId;
	}

	public string Category { get; }

	public string CheckId { get; }

	public string Scope { get; set; }

	public string Target { get; set; }

	public string MessageId { get; set; }

	public string Justification { get; set; }
}
#endif