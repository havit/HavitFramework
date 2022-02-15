using System.ComponentModel;

namespace System.Runtime.CompilerServices;

/// <summary>
/// Allows using init properties in netstandard2.0.
/// Reserved to be used by the compiler for tracking metadata.
/// This class should not be used by developers in source code.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
internal static class IsExternalInit
{
}