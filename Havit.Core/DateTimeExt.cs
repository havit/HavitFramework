using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit;

/// <summary>
/// Extension methods for working with dates <see cref="DateTime"/>.	
/// </summary>
public static class DateTimeExt
{
	/// <summary>
	/// Returns the smallest (earliest) of the specified dates.
	/// </summary>
	public static DateTime Min(params DateTime[] values)
	{
		return values.Min();
	}

	/// <summary>
	/// Returns the largest (latest) of the specified dates.
	/// </summary>
	public static DateTime Max(params DateTime[] values)
	{
		return values.Max();
	}
}
