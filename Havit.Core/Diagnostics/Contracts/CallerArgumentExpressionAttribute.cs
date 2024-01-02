// For supporting caller argument expression without .NET 6.0.
// See https://stackoverflow.com/questions/70034586/how-can-i-use-callerargumentexpression-with-visual-studio-2022-and-net-standard

#if !NET6_0_OR_GREATER
namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
internal sealed class CallerArgumentExpressionAttribute : Attribute
{
	public CallerArgumentExpressionAttribute(string parameterName)
	{
		ParameterName = parameterName;
	}

	public string ParameterName { get; }
}
#endif
