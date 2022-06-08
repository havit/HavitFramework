using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Hangfire.Extensions.Helpers;

/// <summary>
/// Helper methods to format job name.
/// </summary>
public static class JobNameHelper
{
	/// <summary>
	/// If the type is an interface returns the name of the type without the leading I.
	/// </summary>
	/// <example>For an interface type IMyService returns MyService.</example>
	public static bool TryGetSimpleNameFromInterfaceName(Type type, out string result)
	{
		if ((type != null) && type.IsInterface && (type.Name.StartsWith("I", System.StringComparison.Ordinal)))
		{
			result = type.Name.Substring(1);
			return true;
		}

		result = null;
		return false;
	}
}